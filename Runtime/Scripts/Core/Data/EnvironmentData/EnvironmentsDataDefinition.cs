using UnityEngine;

namespace JoystickRemoteConfig.Core.Data
{
    public class EnvironmentsDataDefinition : ScriptableObject
    {
        public EnvironmentData[] environments;
        [HideInInspector] public EnvironmentData selectedEnvironment = new();
        private bool _updateEnvironment;
    }
}