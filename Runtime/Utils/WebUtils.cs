using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Modules.Orchestration.Data;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Runtime.Utils
{
    public static class WebUtils
    {
        // TODO: @Danil refactor for base usage
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public static async UniTask<string> GetAccessToken(string projectId, KeyData keyData, string environmentId)
        {
            byte[] keyByteArray = Encoding.UTF8.GetBytes(keyData.KeyID + ":" + keyData.KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);
            
            string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={projectId}&environmentId={environmentId}";
            string jsonRequestBody = FantasticJson.Parse(new TokenExchangeRequest
            {
                Scopes = new[] { "multiplay.allocations.create", "multiplay.allocations.list", "multiplay.allocations.get" },
            });

            TokenExchangeResponse tokenExchangeResponse = null;
            WebRequestUtils.PostJson(url,
                unityWebRequest =>
                {
                    unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
                },
                jsonRequestBody,
                error =>
                {
                    FantasticDebug.Logger.LogMessage("Error: " + error, FantasticLogType.ERROR);
                    tokenExchangeResponse = new TokenExchangeResponse
                    {
                        AccessToken = "Error"
                    };
                },
                json =>
                {
                    FantasticDebug.Logger.LogMessage("Success: " + json);
                    tokenExchangeResponse = FantasticJson.ParseTo<TokenExchangeResponse>(json);
                }
            );

            await UniTask.WaitUntil(() => tokenExchangeResponse != null);
            return tokenExchangeResponse.AccessToken == "Error" ? "" : tokenExchangeResponse.AccessToken;
        }
        
        [Serializable]
        private class TokenExchangeRequest
        {
            #region Properties

            [JsonProperty("scopes")]
            public string[] Scopes { get; set; }

            #endregion
        }

        [Serializable]
        private class TokenExchangeResponse
        {
            #region Properties

            [JsonProperty("accessToken")]
            public string AccessToken { get; set; }

            #endregion
        }
    }
}