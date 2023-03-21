using UnityEngine;

namespace JoystickRemoteConfig.Core
{
    public class JoystickLogger
    {
        private const string Tag = "<color=ff6f00>Joystick</color>";
        
        public static void Log(object message)
        {
            var enableDebugMode = JoystickUtilities.GetJoystickGeneralDefinition().IsDebugModeEnabled;

            if (enableDebugMode)
            {
                Debug.unityLogger.Log(Tag, message);
            }
        }

        public static void LogError(object message)
        {
            var enableDebugMode = JoystickUtilities.GetJoystickGeneralDefinition().IsDebugModeEnabled;

            if (enableDebugMode)
            {
                Debug.unityLogger.LogError(Tag, message);
            }
        }
    }
}