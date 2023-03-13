using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace JoystickRemote.Core.Web
{
    [PublicAPI]
    public class WebRequestResponseData
    {
        public long ResponseCode;
        public string RequestUrl;
        public bool HasError;
        public string ErrorMessage;

        public Dictionary<string, string> ResponseHeaders = new();
        public Uri UriData;
        public string TextData;
        
        public UnityWebRequest WebRequest;
    }
}