using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JoystickRemote
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
        
        /// <summary>
        /// Fetches remote config content from Joystick server passing RequestContentConfig
        /// </summary>
        /// <param name="definition">A data containing a list of content data and extended data</param>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchConfigContent(RequestContentDefinition definition, Action<bool, string> callback, bool getFreshContent = false)
        {
            FetchConfigContent(definition.DefinitionData, callback, definition.RequestData, getFreshContent);
        }

        /// <summary>
        /// Fetches remote config content from Joystick server passing a list of ContentConfigData and ExtendedRequestData
        /// </summary>
        /// <param name="configList">A list data include multiple content name and content id</param>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="extendedRequestData">A data containing uniqueUserId, version and an array of attributes</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchConfigContent(List<ContentDefinitionData> configList, Action<bool, string> callback, ExtendedRequestData extendedRequestData = null, bool getFreshContent = false)
        {
            JoystickService.Instance.FetchConfigContent(configList, callback, extendedRequestData, getFreshContent);
        }
        
        /// <summary>
        /// Fetches remote catalog content from Joystick server
        /// </summary>
        /// <param name="callback">A callback when request is done</param>
        /// <param name="getFreshContent">A bool value to force updating the content when request</param>
        public static void FetchCatalogContent(Action<bool, string> callback, bool getFreshContent = false)
        {
            JoystickService.Instance.FetchCatalogContent(callback, getFreshContent);
        }
    }
}