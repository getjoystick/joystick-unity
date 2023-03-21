using UnityEngine;

namespace JoystickRemoteConfig
{
    [CreateAssetMenu(fileName = "RequestContentDefinition", menuName = "Joystick/RequestContentDefinition")]
    public class RequestContentDefinition : ScriptableObject
    {
        [SerializeField] private string[] _contentIds;
        [SerializeField] private ExtendedRequestData _extendedRequestData;

        public string[] ContentIds => _contentIds;

        public ExtendedRequestData RequestData => _extendedRequestData;
    }
}