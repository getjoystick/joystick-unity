using System.Collections.Generic;
using UnityEngine;

namespace JoystickRemote
{
    [CreateAssetMenu(fileName = "RequestContentDefinition", menuName = "Joystick/RequestContentDefinition")]
    public class RequestContentDefinition : ScriptableObject
    {
        [SerializeField] private List<ContentRequestSettings>_contentDefinitionData;
        [SerializeField] private ExtendedRequestData _extendedRequestData;

        public List<ContentRequestSettings> DefinitionData => _contentDefinitionData;

        public ExtendedRequestData RequestData => _extendedRequestData;
    }
}