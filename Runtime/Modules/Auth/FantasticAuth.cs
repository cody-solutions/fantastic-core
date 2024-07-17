using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FantasticNetShared.Data.Error;
using FantasticNetShared.Data.Auth;
using FantasticNetShared.Data.User;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.API.Data;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Local;
using FantasticCore.Runtime.Modules.User;
using FantasticCore.Runtime.Modules.Auth.API;
using FantasticCore.Runtime.Modules.Auth.Data;

namespace FantasticCore.Runtime.Modules.Auth
{
    /// <summary>
    /// Fantastic Auth Module Implementation
    /// </summary>
    internal sealed class FantasticAuth : IFantasticAuth
    {
        #region Fields

        private const string RefreshTokenSaveKey = "FANTASTIC_REFRESH";

        #region Properties

        public IEnumerable<Type> ModulesDependencies => new[] { typeof(IFantasticUser) };

        private static LoggedState LoggedState
        {
            get => FantasticLocal.Current.LoggedState;
            set => FantasticLocal.Current.SetLoggedState(value);
        }

        #endregion
        
        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
            => await UniTask.CompletedTask;

        async UniTask IFantasticModule.ResetModule()
            => await UniTask.CompletedTask;

        #endregion

        #region Sign-In

        public OperationHandleData<AuthResponseRootDTO> SignIn(AuthRequestData authRequestData, bool saveRefreshToken = true)
        {
            var operation = new OperationHandleData<AuthResponseRootDTO>();
            HandleOperation();
            return operation;

            async void HandleOperation()
            {
                if (LoggedState != LoggedState.NONE)
                {
                    operation.SetFailed(ErrorData.Create("Already logged in!"));
                    return;
                }

                APIResponseDataT<AuthResponseRootDTO> authResponse = await AuthAPIService.SignIn(authRequestData);
                if (!authResponse.APISuccess)
                {
                    operation.SetFailed(authResponse.APIResponse.Error);
                    return;
                }

                HandleSignInData(authResponse, operation, saveRefreshToken);
            }
        }

        public OperationHandleData<AuthResponseRootDTO> TrySignInWithSavedRefreshToken(bool saveRefreshToken = true)
        {
            var operation = new OperationHandleData<AuthResponseRootDTO>();
            HandleOperation();
            return operation;
            
            async void HandleOperation()
            {
                string refreshToken = GetSavedRefreshToken();
                if (refreshToken.IsNotValid())
                {
                    operation.SetFailed(ErrorData.Create("Failed get saved refresh token."));
                    return;
                }
                
                APIResponseDataT<AuthResponseRootDTO> authResponse = await AuthAPIService.RefreshTokenFullResponse(
                    new RefreshTokenRequestData(refreshToken, true));
                if (!authResponse.APISuccess) 
                {
                    operation.SetFailed(authResponse.APIResponse.Error);
                    return;
                }

                HandleSignInData(authResponse, operation, saveRefreshToken);
            }
        }

        private void HandleSignInData(APIResponseDataT<AuthResponseRootDTO> authResponse,
            OperationHandleData<AuthResponseRootDTO> operation, bool saveRefreshToken)
        {
            if (!authResponse.Result.IsSetupState)
            {
                FinishSignIn(authResponse.Result, saveRefreshToken);
            }
            else
            {
                FinishVerifySignIn(authResponse.Result, saveRefreshToken);
            }

            operation.SetComplete(authResponse.Result);
        }

        public async UniTask WaitForFullyLogged(CancellationToken cancellationToken = default)
            => await UniTask.WaitUntil(() => LoggedState == LoggedState.FULLY_LOGGED, cancellationToken: cancellationToken);

        private void FinishSignIn(AuthResponseRootDTO authResponse, bool saveRefreshToken = false)
        {
            FantasticLocal.Current.SetUser(authResponse.User);
            FantasticLocal.Current.SetAuthResponse(authResponse.Auth);

            LoggedState = LoggedState.FULLY_LOGGED;
            if (saveRefreshToken)
            {
                SaveRefreshToken(authResponse.Auth.RefreshToken);
            }
        }

        private void FinishVerifySignIn(AuthResponseRootDTO authResponse, bool saveRefreshToken = false)
        {
            FantasticLocal.Current.SetAuthResponse(authResponse.Auth);
            FantasticLocal.Current.SetUserAuthRequest(authResponse.UserAuthRequest);

            LoggedState = LoggedState.SETUP_STEP;
            if (saveRefreshToken)
            {
                SaveRefreshToken(authResponse.Auth.RefreshToken);
            }
        }

        #endregion
        
        #region Sign-Up

        public OperationHandleData<UserAuthRequestDTO> SignUp(AuthRequestData authRequestData)
        {
            var operation = new OperationHandleData<UserAuthRequestDTO>();
            HandleOperation();
            return operation;

            async void HandleOperation()
            {
                APIResponseDataT<UserAuthRequestDTO> authResponse = await AuthAPIService.SignUp(authRequestData);
                if (!authResponse.APISuccess)
                {
                    operation.SetFailed(authResponse.APIResponse.Error);
                    return;
                }

                FantasticLocal.Current.SetUserAuthRequest(authResponse.Result);
                operation.SetComplete(authResponse.Result);
            }
        }

        public OperationHandleData<UserDTO> SignUpConfirm(string login, int? pfpConfigId, bool autoRefreshToken = true)
        {
            var operation = new OperationHandleData<UserDTO>();
            HandleOperation();
            return operation;

            async void HandleOperation()
            {
                if (LoggedState != LoggedState.SETUP_STEP)
                {
                    operation.SetFailed(ErrorData.Create("LoggedState must be SETUP_USER to confirm sign-up!"));
                    return;
                }

                APIResponseDataT<UserDTO> authResponse = await AuthAPIService.ConfirmSignUp(
                    new ConfirmSignUpRequestData(FantasticLocal.Current.UserAuthRequestId, login, pfpConfigId));
                if (!authResponse.APISuccess)
                {
                    operation.SetFailed(authResponse.APIResponse.Error);
                    return;
                }

                FantasticLocal.Current.SetUser(authResponse.Result);
                if (autoRefreshToken)
                {
                    OperationHandleData<RefreshTokenResponseDTO> refreshToken = UpdateToken();
                    await refreshToken.Task;
                    
                    if (refreshToken.Status != OperationHandleStatus.SUCCESS)
                    {
                        operation.SetFailed(refreshToken.Error);
                        return;
                    }
                }

                LoggedState = LoggedState.FULLY_LOGGED;
                operation.SetComplete(authResponse.Result);
            }
        }

        public OperationHandleData<RefreshTokenResponseDTO> UpdateToken()
        {
            var operation = new OperationHandleData<RefreshTokenResponseDTO>();
            HandleOperation();
            return operation;

            async void HandleOperation()
            {
                APIResponseDataT<RefreshTokenResponseDTO> refreshToken = await AuthAPIService.RefreshToken(
                    new RefreshTokenRequestData(FantasticLocal.Current.RefreshToken));
                if (!refreshToken.APISuccess) 
                {
                    operation.SetFailed(refreshToken.APIResponse.Error);
                    return;
                }

                FantasticLocal.Current.UpdateToken(refreshToken.Result);
                operation.SetComplete(refreshToken.Result);
            }
        }
        
        #endregion

        public void Logout()
        {
            if (LoggedState == LoggedState.NONE)
            {
                return;
            }

            FantasticLocal.Current.ResetAfterLogout();
            ClearSavedRefreshToken();
        }

        #region Save

        public void SaveRefreshToken(string refreshToken)
            => SaveKey(RefreshTokenSaveKey, refreshToken);

        public string GetSavedRefreshToken()
            => PlayerPrefs.GetString(RefreshTokenSaveKey);

        private static void ClearSavedRefreshToken()
        {
            PlayerPrefs.DeleteKey(RefreshTokenSaveKey);
            PlayerPrefs.Save();
        }

        private static void SaveKey(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        #endregion
    }
    
    public enum LoggedState
    {
        NONE,
        SETUP_STEP,
        FULLY_LOGGED
    }
}