using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoystickRemoteConfig.Core.Web
{
    public class WebRequestManager : MonoBehaviour
    {
        private const int ConcurrentRequestsCount = 6;

        private readonly Dictionary<WebRequestInternal, Coroutine> _pendingRequestsList = new();
        private readonly List<KeyValuePair<WebRequestInternal, IEnumerator>> _waitingRequestsList = new();
        
        private static WebRequestManager _instance;

        public static WebRequestManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(WebRequestManager)).AddComponent<WebRequestManager>();

                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        public void StartRequest(WebRequestInternal request, IEnumerator requestCoroutine)
        {
            if (_pendingRequestsList.Count < ConcurrentRequestsCount)
            {
                if (!_pendingRequestsList.ContainsKey(request))
                {
                    _pendingRequestsList.Add(request, StartCoroutine(requestCoroutine));
                    request.OnRequestDone += responseData => HandleOnRequestDone(request);
                }
                else
                {
                    JoystickLogger.LogError($"Already has the same request: {request.GetHashCode()} in pending requests list.");
                }
            }
            else
            {
                _waitingRequestsList.Add(new KeyValuePair<WebRequestInternal, IEnumerator>(request, requestCoroutine));
            }
        }

        public void StopRequest(WebRequestInternal request)
        {
            if (_pendingRequestsList.ContainsKey(request))
            {
                StopCoroutine(_pendingRequestsList[request]);
                _pendingRequestsList.Remove(request);
            }
            else
            {
                _waitingRequestsList.Remove(_waitingRequestsList.FirstOrDefault(keyValuePair => keyValuePair.Key == request));
            }
        }

        public void RestartRequest(WebRequestInternal request, IEnumerator requestCoroutine)
        {
            if (_pendingRequestsList.ContainsKey(request))
            {
                _pendingRequestsList.Remove(request);

                _pendingRequestsList.Add(request, StartCoroutine(requestCoroutine));
            }
            else
            {
                JoystickLogger.LogError($"Not has the request: {request.GetHashCode()} in pending list.");
            }
        }

        private void TryStartNextPendingRequest()
        {
            if (_waitingRequestsList.Count > 0)
            {
                StartRequest(_waitingRequestsList[0].Key, _waitingRequestsList[0].Value);
                _waitingRequestsList.RemoveAt(0);
            }
        }

        private void HandleOnRequestDone(WebRequestInternal request)
        {
            StopRequest(request);
            TryStartNextPendingRequest();
        }
    }
}