using System;
using UnityEngine;
using FantasticNetShared.Data.Auth;
using FantasticNetShared.Data.User;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.Auth.Data;
using FantasticCore.Runtime.Fast_Play;
using FantasticCore.Runtime.UI;

namespace FantasticCore.Runtime.Modules.Auth.UI
{
    public sealed class AuthUIPresenter : BaseUIPresenter
    {
        #region Fields

        private readonly IFantasticAuth _auth;
        private readonly AuthUIView _view;
        
        private bool _authProcess;

        #endregion

        #region Constructor

        public AuthUIPresenter(IFantasticAuth auth, AuthUIView view)
        {
            if (auth == null || !view)
            {
                throw new Exception($"Not valid {GetType().Name} presenter constructor!");
            }

            _auth = auth;
            _view = view;
            _view.OpenLoginPanel();
            LoadAuthData();
        }

        #endregion
        
        public override void Enable()
        {
            _view.BindLoginButtonClicked(true, OnLoginButtonClicked);
            _view.BindOpenLoginButtonClicked(true, _view.OpenLoginPanel);
            _view.BindRegisterButtonClicked(true, OnRegisterButtonClicked);
            _view.BindOpenRegisterButtonClicked(true, _view.OpenRegisterPanel);
        }

        public override void Disable()
        {
            _view.BindLoginButtonClicked(false, OnLoginButtonClicked);
            _view.BindOpenLoginButtonClicked(false, _view.OpenLoginPanel);
            _view.BindRegisterButtonClicked(false, OnRegisterButtonClicked);
            _view.BindOpenRegisterButtonClicked(false, _view.OpenRegisterPanel);
        }

        #region SignIn

        private void OnLoginButtonClicked()
        {
            SignIn(_view.LoginEmailInput, _view.LoginPasswordInput);
        }

        private async void SignIn(string email, string password)
        {
            if (_authProcess)
            {
                return;
            }

            if (!IsInputDataValid())
            {
                return;
            }

            SetNotification("Sign-in process...");
            _authProcess = true;
            OperationHandleData<AuthResponseRootDTO> loginOperation = _auth.SignIn(new AuthRequestData
            {
                Request = new SignUpRequestData(email, password)
            });
            
            await loginOperation.Task;
            _authProcess = false;
            if (loginOperation.Status == OperationHandleStatus.SUCCESS)
            {
                SetNotification("Sign-in success! Finish process...");
                ConfirmSignUpIfNeed(loginOperation.Result);
                return;
            }

            SetNotificationError(loginOperation.Error.Error);
            return;

            bool IsInputDataValid()
            {
                return email.IsValid() && password.IsValid();
            }
        }

        private void ConfirmSignUpIfNeed(AuthResponseRootDTO authResponseRoot)
        {
            if (authResponseRoot.IsSetupState)
            {
                ConfirmSignUp(_view.RegisterNameInput, 0);
            }
        }

        #endregion

        #region SignUp

        private void OnRegisterButtonClicked()
            => SignUp(_view.RegisterEmailInput, _view.RegisterNameInput, _view.RegisterPasswordInput);
        
        private async void SignUp(string email, string userName, string password)
        {
            if (_authProcess)
            {
                return;
            }

            if (!IsInputDataValid())
            {
                return;
            }

            SetNotification("Sign-up process...");
            _authProcess = true;
            OperationHandleData<UserAuthRequestDTO> signUpOperation = _auth.SignUp(new AuthRequestData
            {
                Request = new SignUpRequestData(email, password)
            });
            
            await signUpOperation.Task;
            _authProcess = false;
            if (signUpOperation.Status == OperationHandleStatus.SUCCESS)
            {
                SetNotification("Verify email and sign-in to continue!");
                return;
            }

            SetNotificationError(signUpOperation.Error.Error);
            return;
            
            bool IsInputDataValid()
            {
                return email.IsValid() && userName.IsValid() && password.IsValid();
            }
        }

        private async void ConfirmSignUp(string login, int? pfpConfigId)
        {
            _authProcess = true;
            OperationHandleData<UserDTO> signUpConfirmOperation = _auth.SignUpConfirm(login, pfpConfigId);
            
            await signUpConfirmOperation.Task;
            _authProcess = false;
            
            if (signUpConfirmOperation.Status == OperationHandleStatus.SUCCESS)
            {
                return;
            }
            
            SetNotificationError(signUpConfirmOperation.Error.Error);
        }

        #endregion
        
        #region Save/Load Data

        private void LoadAuthData()
        {
            if (!FantasticFastPlay.FastPlayEnabled)
            {
                _auth.TrySignInWithSavedRefreshToken();
            }
        }

        #endregion
        
        #region Fast Play

        internal void SetFastPlayData(FastPlayData fastPlayData)
        {
            SetViewLoginData(fastPlayData.Email, fastPlayData.Password);
            OnLoginButtonClicked();
        }

        #endregion

        private void SetViewLoginData(string email, string password)
        {
            _view.SetLoginEmailInput(email);
            _view.SetLoginPasswordInput(password);
        }

        private void SetNotification(string value)
            => _view.SetNotificationText(value, Color.white);
        
        private void SetNotificationError(string value)
            => _view.SetNotificationText(value, Color.red);
    }
}