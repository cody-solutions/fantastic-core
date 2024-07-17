using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace FantasticCore.Runtime.Utils
{
    public static class WebRequestUtils
    {
        private class WebRequestsMonoBehaviour : MonoBehaviour { }

        private static WebRequestsMonoBehaviour _webRequestsMonoBehaviour;

        private static void Init()
        {
            if (_webRequestsMonoBehaviour != null) return;
            
            var gameObject = new GameObject("WebRequests");
            _webRequestsMonoBehaviour = gameObject.AddComponent<WebRequestsMonoBehaviour>();
        }

        public static void Get(string url, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, null, onError, onSuccess));
        }

        public static void Get(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, setHeaderAction, onError, onSuccess));
        }

        private static IEnumerator GetCoroutine(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = UnityWebRequest.Get(url);
            setHeaderAction?.Invoke(unityWebRequest);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }

        public static void Post(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, formFields, onError, onSuccess));
        }

        public static void Post(string url, string postData, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, postData, onError, onSuccess));
        }

        public static void PostJson(string url, string jsonData, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePostJson(url, null, jsonData, onError, onSuccess));
        }

        public static void PostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePostJson(url, setHeaderAction, jsonData, onError, onSuccess));
        }

        private static IEnumerator GetCoroutinePost(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = UnityWebRequest.Post(url, formFields);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }

        private static IEnumerator GetCoroutinePost(string url, string postData, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = UnityWebRequest.PostWwwForm(url, postData);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }

        private static IEnumerator GetCoroutinePostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            setHeaderAction?.Invoke(unityWebRequest);

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }

        public static void Put(string url, string bodyData, Action<string> onError, Action<string> onSuccess) {
            Init();
            _webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePut(url, bodyData, onError, onSuccess));
        }

        private static IEnumerator GetCoroutinePut(string url, string bodyData, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = UnityWebRequest.Put(url, bodyData);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError or UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }
}