using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JoystickRemoteConfig.Core;
using JoystickRemoteConfig.Core.API;
using JoystickRemoteConfig.Core.Data;
using JoystickRemoteConfig.Core.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JoystickRemoteConfig
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
        public ExtendedRequestData GlobalExtendedRequestData { get; private set; }
        
        private bool _autoFetchContent;
        private bool _getFreshContent;
        private string _runtimeEnvironmentAPIKey;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Instance._runtimeEnvironmentAPIKey = string.Empty;
            
            Instance.GlobalExtendedRequestData = JoystickUtilities.GlobalExtendedRequestDefinition().GlobalExtendedRequestData;
            
            var generalDefinition = JoystickUtilities.GetJoystickGeneralDefinition();

            if (generalDefinition != null)
            {
                Instance._autoFetchContent = generalDefinition.IsRequestContentAtStartEnabled;
            }

            if (Instance._autoFetchContent)
            {
                string[] contentIds = generalDefinition.RequestContentDefinitionAtStart.ContentIds;
                Instance.GlobalExtendedRequestData = generalDefinition.RequestContentDefinitionAtStart.RequestData;
                Instance.FetchConfigContent(contentIds,
                    (isSucceed, responseJsonData) =>
                    {
                        Instance.OnAutoStartFetchContentCompleted?.Invoke(isSucceed, responseJsonData);
                    }, Instance.GlobalExtendedRequestData);
            }
        }

        public void FetchConfigContent(string[] contentIds, Action<bool, string> callback, ExtendedRequestData overrideExtendedRequestData = null, bool getFreshContent = false)
        {
            _getFreshContent = getFreshContent;

            (string url, string requestBody) = PrepareRequest(contentIds, overrideExtendedRequestData);

            var apiKey = string.IsNullOrWhiteSpace(_runtimeEnvironmentAPIKey) ? GetCurrentEnvironmentAPIKey() : _runtimeEnvironmentAPIKey;
            Dictionary<string, string> headers = new Dictionary<string, string> { { "x-api-key", apiKey } };
            WebRequest.PostRequestConfiguration postRequestConfiguration = new WebRequest.PostRequestConfiguration(url, headers, requestBody);
            WebRequest webRequest = new WebRequest(postRequestConfiguration);
            webRequest.OnRequestDone += response => HandleOnRequestDone(response, callback);
        }
       
        public void FetchCatalogContent(Action<bool, string> callback)
        {
            _getFreshContent = true;
            
            string url = JoystickUtilities.GetCatalogAPIUrl();
            var apiKey = string.IsNullOrWhiteSpace(_runtimeEnvironmentAPIKey) ? GetCurrentEnvironmentAPIKey() : _runtimeEnvironmentAPIKey;
            Dictionary<string, string> headers = new Dictionary<string, string> { { "x-api-key", apiKey } };
            WebRequest.GetRequestConfiguration getRequestConfiguration = new WebRequest.GetRequestConfiguration(url, headers);
            WebRequest webRequest = new WebRequest(getRequestConfiguration);
            webRequest.OnRequestDone += response => HandleOnRequestDone(response, callback);
        }

        public void SetRuntimeEnvironmentAPIKey(string apiKey)
        {
            _runtimeEnvironmentAPIKey = apiKey;
        }
        
        public void SetExtendedRequestData(ExtendedRequestData extendedRequestData)
        {
            GlobalExtendedRequestData = extendedRequestData ?? new ExtendedRequestData();

            CacheDataManager.SetCache(nameof(ExtendedRequestData), JsonConvert.SerializeObject(extendedRequestData));
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
                JoystickLogger.LogError($"Request failed: code: {responseData.ResponseCode} | error message: {responseData.ErrorMessage}");

                if (CacheDataManager.HasCache(responseData.RequestUrl))
                {
                    JoystickLogger.LogError($"Request failed will use cache data: {responseData.RequestUrl}");

                    callback?.Invoke(true, CacheDataManager.GetCache(responseData.RequestUrl));
                }
                else
                {
                    JoystickLogger.LogError($"Request failed don't have cache data: {responseData.RequestUrl}");

                    CacheDataManager.ClearCache(responseData.RequestUrl);
                    callback?.Invoke(false, responseData.TextData);
                }
            }
        }

        private (string, string) PrepareRequest(string[] contentIds, ExtendedRequestData overrideExtendedRequestData)
        {
            APIRequestData requestData;

            if (overrideExtendedRequestData != null)
            {
                GlobalExtendedRequestData = overrideExtendedRequestData;
                CacheDataManager.SetCache(nameof(ExtendedRequestData), JsonConvert.SerializeObject(overrideExtendedRequestData));
            }
            else
            {
                var cachedExtendedRequestData = CacheDataManager.GetCache(nameof(ExtendedRequestData));

                if (!string.IsNullOrWhiteSpace(cachedExtendedRequestData))
                {
                    GlobalExtendedRequestData = JsonConvert.DeserializeObject<ExtendedRequestData>(cachedExtendedRequestData);
                }
                else
                {
                    GlobalExtendedRequestData ??= new ExtendedRequestData();
                }
            }
            
            if (Regex.IsMatch(GlobalExtendedRequestData.version, @"\d+\.\d+\.\d+"))
            {
                requestData = new APIRequestVersionData
                {
                    u = string.IsNullOrWhiteSpace(GlobalExtendedRequestData.uniqueUserId) ? string.Empty : GlobalExtendedRequestData.uniqueUserId,
                    v = GlobalExtendedRequestData.version
                };
            }
            else
            {
                requestData = new APIRequestData
                {
                    u = string.IsNullOrWhiteSpace(GlobalExtendedRequestData.uniqueUserId) ? string.Empty : GlobalExtendedRequestData.uniqueUserId,
                };
            }
            
            requestData.p = GetAttributesJObject(GlobalExtendedRequestData.attributes);
            
            var url = JoystickUtilities.GetConfigContentAPIUrl(contentIds);
            var requestBody = JsonConvert.SerializeObject(requestData);
            
            return (url, requestBody);
        }

        private string GetCurrentEnvironmentAPIKey()
        {
            EnvironmentsDataDefinition environmentsDataDefinition = JoystickUtilities.GetEnvironmentsDataDefinition();

            if (environmentsDataDefinition == null)
            {
                throw new NullReferenceException("Please set environments data definition first! You can create it by opening Setup Window through Joystick menu.");
            }

            string name = environmentsDataDefinition.selectedEnvironment.Name;
            string apiKey = environmentsDataDefinition.selectedEnvironment.APIKey;

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