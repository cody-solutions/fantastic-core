using System.Collections.Generic;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using FantasticNetShared.Data.Auth;
using FantasticNetShared.Data.User;
using FantasticCore.Runtime.API;
using FantasticCore.Runtime.API.Data;
using FantasticCore.Runtime.API.Extensions;
using FantasticCore.Runtime.Modules.Auth.Data;

namespace FantasticCore.Runtime.Modules.Auth.API
{
    internal static class AuthAPIService
    {
        public static async UniTask<APIResponseDataT<UserAuthRequestDTO>> SignUp(AuthRequestData authRequest)
        {
            authRequest.SetRequestDetails("Auth/SignUp", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("email", authRequest.Request.Email),
                new MultipartFormDataSection("password", authRequest.Request.Password)
            }, RequestType.POST, authorize: false);
            return APIResponseDataT<UserAuthRequestDTO>.Create(await FantasticAPIClient.SendRequest(authRequest));
        }
        
        public static async UniTask<APIResponseDataT<UserDTO>> ConfirmSignUp(ConfirmSignUpRequestData confirmSignUpRequest)
        {
            confirmSignUpRequest.SetRequestDetails("Auth/ConfirmSignUp", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("authRequestId", confirmSignUpRequest.AuthRequestId.ToString()),
                new MultipartFormDataSection("login", confirmSignUpRequest.Login)
            }, RequestType.POST);
            
            confirmSignUpRequest.SetFormDataSectionIfValid("pfpConfigId", confirmSignUpRequest.PfpConfigId);
            return APIResponseDataT<UserDTO>.Create(await FantasticAPIClient.SendRequest(confirmSignUpRequest));
        }

        public static async UniTask<APIResponseDataT<AuthResponseRootDTO>> SignIn(AuthRequestData authRequest)
        {
            authRequest.SetRequestDetails("Auth/SignIn", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("email", authRequest.Request.Email),
                new MultipartFormDataSection("password", authRequest.Request.Password)
            }, RequestType.POST, authorize: false);
            return APIResponseDataT<AuthResponseRootDTO>.Create(await FantasticAPIClient.SendRequest(authRequest));
        }
        
        public static async UniTask<APIResponseDataT<AuthResponseRootDTO>> RefreshTokenFullResponse(RefreshTokenRequestData refreshTokenRequest)
        {
            refreshTokenRequest.SetRequestDetails("Auth/RefreshToken", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("refreshToken", refreshTokenRequest.RefreshToken),
                new MultipartFormDataSection("returnFullResponse", refreshTokenRequest.ReturnFullResponse.ToString())
            }, RequestType.POST, authorize: false);
            return APIResponseDataT<AuthResponseRootDTO>.Create(await FantasticAPIClient.SendRequest(refreshTokenRequest));
        }
        
        public static async UniTask<APIResponseDataT<RefreshTokenResponseDTO>> RefreshToken(RefreshTokenRequestData refreshTokenRequest)
        {
            refreshTokenRequest.SetRequestDetails("Auth/RefreshToken", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("refreshToken", refreshTokenRequest.RefreshToken),
                new MultipartFormDataSection("returnFullResponse", false.ToString())
            }, RequestType.POST, authorize: false);
            return APIResponseDataT<RefreshTokenResponseDTO>.Create(await FantasticAPIClient.SendRequest(refreshTokenRequest));
        }

    }
}