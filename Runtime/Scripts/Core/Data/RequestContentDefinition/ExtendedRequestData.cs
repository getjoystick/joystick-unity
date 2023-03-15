using System;

namespace JoystickRemote
{
    [Serializable]
    public class ExtendedRequestData
    {
        public string uniqueUserId = string.Empty;
        public string version = string.Empty;
        public AttributesData[] attributes = Array.Empty<AttributesData>();
    }
}