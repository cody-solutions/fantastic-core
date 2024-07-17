using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using JetBrains.Annotations;
using FantasticCore.Runtime.Utils;
using FantasticNetShared.Data.Error;

namespace FantasticCore.Runtime.API.Data
{
    [Serializable]
    public class APIProfileData
    {
        #region Properties

        [field: SerializeField, Tooltip("Base API url")] 
        public string Url { get; private set; } = "http://localhost";

        [field: SerializeField, Tooltip("Target API version")]
        public string Version { get; private set; } = "1.0";

        #endregion
    }
    
    public class APIRequestData
    {
        #region Properties

        [CanBeNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public string OverrideBaseUrl { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public RequestType RequestType { get; set; }

        /// <summary>
        /// Additional API url
        /// </summary>
        public string Url { get; private set; }
        
        /// <summary>
        /// Web request body
        /// </summary>
        public List<IMultipartFormSection> Form { get; private set; }

        /// <summary>
        /// Web request timeout in seconds
        /// </summary>
        public float TimeOut { get; set; } = 60.0f;

        public bool AuthorizeRequest { get; set; } = true;

        /// <summary>
        /// Amount time when API will try send web requests while response will not Exception 
        /// </summary>
        public int MaxRetry { get; set; } = 1;

        public CancellationToken CancellationToken { get; set; }

        public event Action<APIResponseData> APISuccessEvent;

        public event Action<APIResponseData> APIFailedEvent;

        #endregion

        public void SetRequestDetails(string url, List<IMultipartFormSection> form = null,
            RequestType requestType = RequestType.GET, string overrideBaseUrl = null, bool authorize = true)
        {
            Url = url;
            Form = form;
            RequestType = requestType;
            OverrideBaseUrl = overrideBaseUrl;
            AuthorizeRequest = authorize;
        }

        internal void TriggerAPIEvent(bool success, APIResponseData responseData)
        {
            if (success)
            {
                APISuccessEvent?.Invoke(responseData);
                return;
            }
            
            APIFailedEvent?.Invoke(responseData);
        }
    }

    public enum RequestType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public struct APIResponseData
    {
        #region Properties

        /// <summary>
        /// Determine API request status
        /// </summary>
        public bool Success { get; private set; }
        
        /// <summary>
        /// Clear response message
        /// </summary>
        public string Response { get; private set; }
        
        public bool IsTimeOut { get; private set; }

        public int RetryAmount { get; private set; }

        /// <summary>
        /// Error data. Exist if API request status was failed
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ErrorData Error { get; private set; }

        #endregion

        #region Constructor

        public APIResponseData(bool success, string response, bool isTimeOut = false, int retryAmount = 1)
        {
            Success = success;
            Response = response;
            IsTimeOut = isTimeOut;
            RetryAmount = retryAmount;
            Error = null;
            if (!success)
            {
                Error = FantasticJson.ParseTo<ErrorData>(response, false) ?? ErrorData.Create(response);
            }
        }

        #endregion
    }

    public struct APIResponseDataT<T> where T : class
    {
        #region Properties

        public bool APISuccess => APIResponse.Success;
        
        public APIResponseData APIResponse { get; private set; }

        public T Result { get; private set; }

        #endregion

        public static APIResponseDataT<T> Create(APIResponseData responseData)
        {
            var responseDataT = new APIResponseDataT<T>
            {
                APIResponse = responseData,
            };

            if (responseData.Success)
            {
                responseDataT.Result = FantasticJson.ParseTo<T>(responseData.Response);
            }

            return responseDataT;
        }
    }
}