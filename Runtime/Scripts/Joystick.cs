using System;
using JetBrains.Annotations;

namespace JoystickRemoteConfig
{
    [PublicAPI]
    public class Joystick
    {
        /// <summary>
        /// An event will be triggered when toggle on "Request Data at Start" option in Joystick editor window and the data fetching complete.
        /// </summary>
        public static event Action<bool, string> OnAutoStartFetchContentCompleted
        {
            add => JoystickService.Instance.OnAutoStartFetchContentCompleted += value;
            remove => JoystickService.Instance.OnAutoStartFetchContentCompleted -= value;
        }

        /// <summary>
        /// Response Json data
        /// </summary>
        public static string ResponseJsonData => JoystickService.Instance.ResponseJsonData;

        public static ExtendedRequestData GlobalExtendedRequestData => JoystickService.Instance.GlobalExtendedRequestData;

        /// <summary>
        /// Fetches remote config content from Joystick server passing RequestContentConfig
        /// </summary>
        /// <param name="definition">A data containing a list of content data and extended data</param>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchConfigContent(RequestContentDefinition definition, Action<bool, string> callback, bool getFreshContent = false)
        {
            FetchConfigContent(definition.ContentIds, callback, definition.RequestData, getFreshContent);
        }

        /// <summary>
        /// Fetches remote config content from Joystick server passing a list of ContentConfigData and ExtendedRequestData
        /// </summary>
        /// <param name="contentIds">A array of string data include multiple content id</param>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="overrideExtendedRequestData">A data to override existing ExtendedRequestData which contains uniqueUserId, version and an array of attributes</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchConfigContent(string[] contentIds, Action<bool, string> callback, ExtendedRequestData overrideExtendedRequestData = null, bool getFreshContent = false)
        {
            JoystickService.Instance.FetchConfigContent(contentIds, callback, overrideExtendedRequestData, getFreshContent);
        }
        
        /// <summary>
        /// Fetches remote catalog content from Joystick server
        /// </summary>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchCatalogContent(Action<bool, string> callback)
        {
            JoystickService.Instance.FetchCatalogContent(callback);
        }

        /// <summary>
        /// Set up a runtime apikey for Joystick
        /// </summary>
        /// <param name="apiKey">A string value of api key</param>
        public static void SetRuntimeEnvironmentAPIKey(string apiKey)
        {
            JoystickService.Instance.SetRuntimeEnvironmentAPIKey(apiKey);
        }
        
        /// <summary>
        /// Set up extended request data for Joystick
        /// </summary>
        /// <param name="extendedRequestData">An ExtendedRequestData contains uniqueUserId, version and an array of attributes</param>
        public static void SetExtendedRequestData(ExtendedRequestData extendedRequestData)
        {
            JoystickService.Instance.SetExtendedRequestData(extendedRequestData);
        }
    }
}