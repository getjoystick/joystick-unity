using UnityEngine;

namespace JoystickRemoteConfig
{
    [CreateAssetMenu(fileName = "RequestContentDefinition", menuName = "Joystick/GlobalExtendedRequestDefinition")]
    public class GlobalExtendedRequestDefinition : ScriptableObject
    {
        [SerializeField] private ExtendedRequestData _globalExtendedRequestData;

        public ExtendedRequestData GlobalExtendedRequestData => _globalExtendedRequestData;
    }
}