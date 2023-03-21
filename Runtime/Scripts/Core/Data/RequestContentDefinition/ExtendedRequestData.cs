using System;

namespace JoystickRemoteConfig
{
    [Serializable]
    public class ExtendedRequestData
    {
        public string uniqueUserId = string.Empty;
        public string version = string.Empty;
        public AttributesData[] attributes = Array.Empty<AttributesData>();

        public string SetUniqueUserId(string uniqueUserId)
        {
            return this.uniqueUserId = uniqueUserId;
        }
        
        public string SetVersion(string version)
        {
            return this.version = version;
        }
        
        public AttributesData[] SetAttributes(AttributesData[] attributes)
        {
            return this.attributes = attributes;
        }
    }
}