using UnityEngine;

namespace JoystickRemote.Core
{
    public class JoystickGeneralDefinition : ScriptableObject
    {
        public bool IsRequestContentAtStartEnabled;
        public RequestContentDefinition RequestContentDefinitionAtStart;
        
        public bool IsSerializedResponseEnabled;
        public bool IsDebugModeEnabled;
    }
}