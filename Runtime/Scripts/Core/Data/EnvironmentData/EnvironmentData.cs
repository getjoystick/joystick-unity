using System;
using UnityEngine;

namespace JoystickRemote.Core.Data
{
    [Serializable]
    public class EnvironmentData
    {
        [SerializeField] private string _name;
        [SerializeField] private string _apiKey;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string APIKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }
    }
}
