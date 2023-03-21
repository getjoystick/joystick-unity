using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace JoystickRemoteConfig.Core.Web
{
    [PublicAPI]
    public class WebRequestInternal
    {
        public event Action<WebRequestResponseData> OnRequestDone;
        public event Action<int> OnRequestWillRestart;

        private const int RequestTimeOut = 8;
        private int _requestAttemptsLimit = 3;

        public WebRequestState RequestState { get; private set; }
        
        private WebRequest.Configuration _configuration;
        private UnityWebRequest _request;

        private int _requestTimeOutDuration;
        private int _requestAttempts;
        
        public WebRequestInternal(WebRequest.Configuration configuration)
        {
            _configuration = configuration;

            if (SetUpRequestConfiguration(configuration))
            {
                if (configuration.AutoStart)
                {
                    StartRequest();
                }
            }
            else
            {
                HandleOnRequestInvalid();
            }
        }

        private void StartRequest()
        {
            if (RequestState == WebRequestState.None)
            {
                RequestState = WebRequestState.Pending;
                WebRequestManager.Instance.StartRequest(this, RequestRoutine());
            }
        }

        private void StopRequest()
        {
            if (RequestState == WebRequestState.Pending)
            {
                RequestState = WebRequestState.None;
                WebRequestManager.Instance.StopRequest(this);
                DisposeRequest(true);
            }
        }

        private void RestartRequest()
        {
            if (RequestState == WebRequestState.Timeout)
            {
                RequestState = WebRequestState.Pending;
                
                JoystickLogger.Log($"Request timed out and will restart. Restart count: {_requestAttempts}. Url: {_configuration.Url}");

                OnRequestWillRestart?.Invoke(_requestAttempts);

                SetUpRequestConfiguration(_configuration);
                WebRequestManager.Instance.RestartRequest(this, RequestRoutine());
            }
        }

        private bool SetUpRequestConfiguration(WebRequest.Configuration configuration)
        {
            bool configured = false;
            try
            {
                Uri uri = new Uri(configuration.Url);
                string url = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.ToString();

                switch (configuration.RequestType)
                {
                    case UnityWebRequest.kHttpVerbGET:
                        _request = UnityWebRequest.Get(url);
                        break;
                    case UnityWebRequest.kHttpVerbPOST:
                        WebRequest.PostRequestConfiguration postRequestConfiguration = (WebRequest.PostRequestConfiguration)configuration;

                        byte[] postData = new UTF8Encoding().GetBytes(postRequestConfiguration.RequestBody);
                        
                        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(postData);
                        uploadHandlerRaw.contentType = postRequestConfiguration.ContentType;
                        _request = new UnityWebRequest(url, "Post", new DownloadHandlerBuffer(), uploadHandlerRaw);
                        
                        break;
                }

                foreach (KeyValuePair<string, string> keyValuePair in configuration.Headers)
                {
                    _request.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                }

                configured = true;
            }
            catch (Exception e)
            {
                JoystickLogger.LogError($"Error Configuring Web Request For Url: {configuration.Url} | Exception: {e}");
            }

            return configured;
        }

        private IEnumerator RequestRoutine()
        {
            if (RequestState == WebRequestState.Pending)
            {
                _requestTimeOutDuration = RequestTimeOut + (RequestTimeOut / 2 * _requestAttempts);
                _requestAttempts++;

                _request.SendWebRequest();
            }

            float requestProgress = -1f;
            float requestStuckTime = 0f;

            while (!_request.isDone)
            {
                bool requestNotProgressing = Mathf.Approximately(requestProgress, _request.uploadProgress + _request.downloadProgress);

                if (requestNotProgressing)
                {
                    requestStuckTime += Time.deltaTime;

                    if (requestStuckTime >= _requestTimeOutDuration)
                    {
                        RequestState = WebRequestState.Timeout;
                        HandleOnRequestTimeOut();

                        yield break;
                    }
                }
                else
                {
                    requestStuckTime = 0f;
                    requestProgress = _request.uploadProgress + _request.downloadProgress;
                }

                yield return null;
            }

            RequestState = WebRequestState.Completed;

            if (_request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError)
            {
                JoystickLogger.LogError($"Request result: {_request.result} | ErrorInfo: {_request.error} | Url: {_request.url}");
            }
            else
            {
                JoystickLogger.Log($"Url: {_request.url} | Response Code:{_request.responseCode} | Response Data: {_request.downloadHandler.text}");
            }

            HandleOnRequestDone();
            DisposeRequest(true);
        }

        private void HandleOnRequestTimeOut()
        {
            if (_requestAttempts < _requestAttemptsLimit)
            {
                DisposeRequest(false);
                RestartRequest();
            }
            else
            {
                HandleOnRequestDone();
                DisposeRequest(true);
            }
        }

        private void HandleOnRequestDone()
        {
            WebRequestResponseData responseData = new WebRequestResponseData
            {
                ResponseCode = _request.responseCode,
                RequestUrl = _configuration.Url,
                HasError = !string.IsNullOrWhiteSpace(_request.error),
                ErrorMessage = _request.error,
                ResponseHeaders = _request.GetResponseHeaders() ?? new Dictionary<string, string>(),
                UriData = _request.uri,
                TextData = _request.downloadHandler.text,
                WebRequest = _request,
            };

            OnRequestDone?.Invoke(responseData);
            _configuration = null;
        }
        
        private void HandleOnRequestInvalid()
        {
            WebRequestResponseData responseData = new WebRequestResponseData
            {
                ResponseCode = -1,
                RequestUrl = string.IsNullOrWhiteSpace(_configuration.Url) ? string.Empty : _configuration.Url,
                HasError = true,
                ErrorMessage = string.Empty,
                ResponseHeaders = new Dictionary<string, string>(),
                UriData = string.IsNullOrWhiteSpace(_configuration.Url) ? new Uri("https://getjoystick.com") : new Uri(_configuration.Url),
                TextData = string.Empty,
                WebRequest = _request,
            };

            OnRequestDone?.Invoke(responseData);
            _configuration = null;
        }

        private void DisposeRequest(bool disposeUploadHandler)
        {
            _request.disposeUploadHandlerOnDispose = disposeUploadHandler;
            _request.Dispose();
            _request = null;
        }
    }
}