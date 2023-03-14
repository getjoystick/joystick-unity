using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JoystickRemote.Core;
using JoystickRemote.Core.API;
using JoystickRemote.Core.Data;
using JoystickRemote.Core.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JoystickRemote
{
    public sealed class JoystickService
    {
        private static JoystickService _instance;
        private static readonly object padlock = new();
        
        public static JoystickService Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instance ??= new JoystickService();
                }
            }
        }

        public Action<bool, string> OnAutoStartFetchContentCompleted;
        public string ResponseJsonData { get; private set; }
        
        private bool _autoFetchContent;
        private bool _getFreshContent;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            CacheDataManager.ClearCache();

            var generalDefinition = JoystickUtilities.GetJoystickGeneralDefinition();

            if (generalDefinition != null)
            {
                Instance._autoFetchContent = generalDefinition.IsRequestContentAtStartEnabled;
            }

            if (Instance._autoFetchContent)
            {
                List<ContentDefinitionData> contentDefinitionDataList = generalDefinition.RequestContentDefinitionAtStart.DefinitionData;
                ExtendedRequestData extendedRequestData = generalDefinition.RequestContentDefinitionAtStart.RequestData;
                Instance.FetchConfigContent(contentDefinitionDataList,
                    (isSucceed, responseJsonData) =>
                    {
                        Instance.OnAutoStartFetchContentCompleted?.Invoke(isSucceed, responseJsonData);
                    }, extendedRequestData);
            }
        }

        public void FetchConfigContent(List<ContentDefinitionData> contentDefinitionDataList, Action<bool, string> callback, ExtendedRequestData extendedRequestData = null, bool getFreshContent = false)
        {
            _getFreshContent = getFreshContent;
            
            (string url, string requestBody) = PrepareRequest(contentDefinitionDataList, extendedRequestData);

            Dictionary<string, string> headers = new Dictionary<string, string> { { "x-api-key", GetCurrentEnvironmentAPIKey() } };
            WebRequest.PostRequestConfiguration postRequestConfiguration = new WebRequest.PostRequestConfiguration(url, headers, requestBody);
            WebRequest webRequest = new WebRequest(postRequestConfiguration);
            webRequest.OnRequestDone += response => HandleOnRequestDone(response, callback);
        }
       
        public void FetchCatalogContent(Action<bool, string> callback, bool getFreshContent = false)
        {
            _getFreshContent = getFreshContent;
            
            string url = JoystickUtilities.GetCatalogAPIUrl();
            Dictionary<string, string> headers = new Dictionary<string, string> { { "x-api-key", GetCurrentEnvironmentAPIKey() } };
            WebRequest.GetRequestConfiguration getRequestConfiguration = new WebRequest.GetRequestConfiguration(url, headers);
            WebRequest webRequest = new WebRequest(getRequestConfiguration);
            webRequest.OnRequestDone += response => HandleOnRequestDone(response, callback);
        }
        
        private void HandleOnRequestDone(WebRequestResponseData responseData, Action<bool, string> callback)
        {
            if (!responseData.HasError && responseData.ResponseCode == 200 && !string.IsNullOrWhiteSpace(responseData.TextData))
            {
                var responseDataText = responseData.TextData;
                ResponseJsonData = responseDataText;
                var cachedDataText = CacheDataManager.GetCache(responseData.RequestUrl);

                if (!string.IsNullOrWhiteSpace(cachedDataText) && cachedDataText.Equals(responseDataText) && !_getFreshContent)
                {
                    JoystickLogger.Log($"Use Cached data: {cachedDataText}");
                    callback.Invoke(true, cachedDataText);
                }
                else
                {
                    CacheDataManager.SetCache(responseData.RequestUrl, responseDataText);
                
                    JoystickLogger.Log($"Request succeed: {responseDataText}");
                    callback?.Invoke(true, responseDataText);
                }
            }
            else
            {
                JoystickLogger.LogError($"Request failed: {responseData.TextData} | Clear Cached Data");
                
                CacheDataManager.ClearCache(responseData.RequestUrl);
                
                callback?.Invoke(false, responseData.TextData);
            }
        }

        private (string, string) PrepareRequest(List<ContentDefinitionData> contentDefinitionDataList, ExtendedRequestData extendedRequestData)
        {
            APIRequestData requestData;

            if (Regex.IsMatch(extendedRequestData.version, @"\d+\.\d+\.\d+"))
            {
                requestData = new APIRequestVersionData
                {
                    u = string.IsNullOrWhiteSpace(extendedRequestData.uniqueUserId) ? string.Empty : extendedRequestData.uniqueUserId,
                    v = extendedRequestData.version
                };
            }
            else
            {
                requestData = new APIRequestData
                {
                    u = string.IsNullOrWhiteSpace(extendedRequestData.uniqueUserId) ? string.Empty : extendedRequestData.uniqueUserId,
                };
            }
            
            requestData.p = GetAttributesJObject(extendedRequestData.attributes);

            var url = JoystickUtilities.GetConfigContentAPIUrl(contentDefinitionDataList);
            var requestBody = JsonConvert.SerializeObject(requestData);
            
            return (url, requestBody);
        }

        private string GetCurrentEnvironmentAPIKey()
        {
            EnvironmentsDataDefinition environmentsDataDefinition = JoystickUtilities.GetEnvironmentDefinition();
            int index = (int)environmentsDataDefinition.environmentType;

            string name = environmentsDataDefinition.environments[index].Name;
            string apiKey = environmentsDataDefinition.environments[index].APIKey;

            JoystickLogger.Log($"Current Environment Name: {name} | API Key: {apiKey}");
            
            return apiKey;
        }

        private JObject GetAttributesJObject(AttributesData[] attributes)
        {
            JObject attributesObject = new JObject();

            if (attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    if (int.TryParse(attribute.value, out int attributeIntValue))
                    {
                        attributesObject.Add(attribute.key, attributeIntValue);
                    }
                    else if (bool.TryParse(attribute.value, out bool attributeBoolValue))
                    {
                        attributesObject.Add(attribute.key, attributeBoolValue);
                    }
                    else if (float.TryParse(attribute.value, out float attributeFloatValue))
                    {
                        attributesObject.Add(attribute.key, attributeFloatValue);
                    }
                    else
                    {
                        attributesObject.Add(attribute.key, attribute.value);
                    }
                }
            }

            return attributesObject;
        }
    }
}