using UnityEngine;

namespace JoystickRemote.Core.Data
{
    public class EnvironmentsDataDefinition : ScriptableObject
    {
        public EnvironmentData[] environments;
        [HideInInspector] public EnvironmentType environmentType;
        
        private bool _updateEnvironment;
    }
}