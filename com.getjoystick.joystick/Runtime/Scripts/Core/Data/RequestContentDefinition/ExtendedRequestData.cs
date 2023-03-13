using JoystickRemote.Core.Data;

namespace JoystickRemote
{
    [System.Serializable]
    public class ExtendedRequestData
    {
        public string uniqueUserId;
        public string version;
        public AttributesData[] attributes;
    }
}