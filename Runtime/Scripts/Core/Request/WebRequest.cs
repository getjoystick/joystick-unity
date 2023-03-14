using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace JoystickRemote.Core.Web
{
    [PublicAPI]
    public class WebRequest : WebRequestInternal
    {
        [PublicAPI]
        public class Configuration
        {
            public string Url { get; protected set; } = string.Empty;
            public Dictionary<string, string> Headers { get; protected set; } = new();
            public string RequestType { get; set; } = string.Empty;
            public bool AutoStart { get; set; } = true;
        }

        #region Get Request Configuration
        
        [PublicAPI]
        public class GetRequestConfiguration : Configuration
        {
            public GetRequestConfiguration(string url, Dictionary<string, string> headers)
            {
                Url = url;
                RequestType = UnityWebRequest.kHttpVerbGET;
                Headers = headers;
            }
        }
        
        #endregion

        #region Post Request Coniguration
        
        [PublicAPI]
        public class PostRequestConfiguration : Configuration
        {
            public string RequestBody { get; private set; }
            public string ContentType { get; private set; }

            public PostRequestConfiguration(string url, Dictionary<string, string> headers, string requestBody, string contentType = "application/json")
            {
                Url = url;
                RequestType = UnityWebRequest.kHttpVerbPOST;
                Headers = headers;
                RequestBody = requestBody;
                ContentType = contentType;
            }
        }
        
        #endregion

        public WebRequest(Configuration configuration) : base(configuration) { }
    }
}