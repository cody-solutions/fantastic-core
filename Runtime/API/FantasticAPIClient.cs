using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.API.Extensions;
using FantasticCore.Runtime.API.Data;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Local;

namespace FantasticCore.Runtime.API
{
    public static class FantasticAPIClient
    {
        #region Fields

        private const string APIVersionHeader = "api-version";

        #region Properties

        /// <summary>
        /// Determine if API client was successfully initialized
        /// </summary>
        private static bool Initialize { get; set; }

        /// <summary>
        /// Target API url
        /// </summary>
        private static string APIUrl { get; set; }

        /// <summary>
        /// Target API version
        /// </summary>
        private static string APIVersion { get; set; } = "1.0";

        #endregion
        
        #endregion

        /// <summary>
        /// Initialize client for API interactions
        /// </summary>
        /// <param name="apiUrl">Target API url, client will use it for sending web requests</param>
        /// <param name="apiVersion">Target API version, client will use it for sending web requests</param>
        public static void InitializeClient(string apiUrl, string apiVersion)
        {
            if (Initialize)
            {
                FantasticDebug.Logger.LogMessage("APIClient is already setup!", FantasticLogType.WARN);
                return;
            }

            if (apiUrl.IsNotValid(true, "APIUrl is not valid!"))
            {
                return;
            }

            APIUrl = apiUrl;
            if (apiVersion.IsValid())
            {
                APIVersion = apiVersion;
            }

            Initialize = true;
        }

        /// <summary>
        /// Base method for sending web requests to API. Will return APIResponseData with response details 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>APIResponseData with response details</returns>
        /// <exception cref="Exception">When not initialized yet or <see cref="requestData"/> isn't valid</exception>
        public static async UniTask<APIResponseData> SendRequest([DisallowNull] APIRequestData requestData)
        {
            if (!Initialize)
            {
                throw new Exception("APIClient is not setup yet. Setup it first!");
            }

            if (requestData == null)
            {
                throw new Exception("RequestData isn't valid!");
            }

            // If passed CancellationToken is default (none), set it with default app exit token 
            if (requestData.CancellationToken == default || requestData.CancellationToken == CancellationToken.None)
            {
                requestData.CancellationToken = Application.exitCancellationToken;
            }

            UnityWebRequest request = null;
            // Default WebRequest timeout is 0 and no timeout is applied. Handle it with UniTask.Timeout()
            // request.timeout = (int)requestData.TimeOut;
            string response;
            int retry = 0;
            int maxRetry = requestData.MaxRetry;
            
            // Try send web requests while MaxRetry > 0 or API response will be success
            while (true)
            {
                retry++;
                try
                {
                    request = GenerateRequest(requestData);
                    await request.SendWebRequest()
                        .WithCancellation(requestData.CancellationToken)
                        .Timeout(TimeSpan.FromSeconds(requestData.TimeOut));
                    break;
                }
                catch (Exception exception)
                {
                    request!.Abort();
                    await UniTask.WaitUntil(() => request.isDone);
                    
                    maxRetry--;
                    if (maxRetry > 0)
                    {
                        continue;
                    }

                    response = request.GetResponseText();
                    return HandleError(exception is TimeoutException, retry);
                }                
            }

            response = request.GetResponseText();
            return request.result == UnityWebRequest.Result.Success
                ? HandleSuccess()
                : HandleError(false, retry);
            
            APIResponseData HandleSuccess()
                => CreateResponseData(true);

            APIResponseData HandleError(bool isTimeOut = false, int tryAmount = 1)
            {
                FantasticDebug.Logger.LogMessage(
                    $"[API-CLIENT] API {APIUrl}/{requestData.Url} failed!" +
                    $" IsTimeout: {isTimeOut}. Error: {response}. Status: {request.result}.", FantasticLogType.ERROR);
                return CreateResponseData(false, isTimeOut, tryAmount);
            }

            // Dispose current web request. Trigger callbacks, create and return APIResponseData with API response details
            APIResponseData CreateResponseData(bool success, bool isTimeOut = false, int tryAmount = 1)
            {
                request.Dispose();
                var responseData = new APIResponseData(success, response, isTimeOut, tryAmount);
                requestData.TriggerAPIEvent(success, responseData);
                return responseData;
            }
        }

        // TODO: Request pooling
        private static UnityWebRequest GenerateRequest(APIRequestData requestData)
        {
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            var request = new UnityWebRequest(
                $"{requestData.OverrideBaseUrl ?? APIUrl}/{requestData.Url}",
                requestData.RequestType.ToString());
            
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader(APIVersionHeader, APIVersion);
            if (requestData.AuthorizeRequest && FantasticLocal.Current.Token.IsValid())
            {
                request.SetRequestHeader("Authorization", $"Bearer {FantasticLocal.Current.Token}");
            }

            if (requestData.Form == null || requestData.Form.Count == 0)
            {
                return request;
            }

            byte[] data = UnityWebRequest.SerializeFormSections(requestData.Form, boundary);
            var uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
            uploadHandler.contentType = GenerateContentType(boundary);
            request.uploadHandler = uploadHandler;
            return request;
        }

        private static string GenerateContentType(byte[] boundary)
            => "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary,
                0, boundary.Length);
    }
}