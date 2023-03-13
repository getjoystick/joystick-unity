using System;
using Newtonsoft.Json.Linq;

namespace JoystickRemote.Core.API
{
    [Serializable]
    public class APIRequestData
    {
        public string u;
        public JObject p;
    }
    
    [Serializable]
    public class APIRequestVersionData : APIRequestData
    {
        public string v;
    }
}