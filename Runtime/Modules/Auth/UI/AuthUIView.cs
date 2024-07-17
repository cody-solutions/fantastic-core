using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.UI;

namespace FantasticCore.Runtime.Modules.Auth.UI
{
    [SelectionBase, DisallowMultipleComponent]
    [AddComponentMenu("FantasticCore/Modules/Auth/Auth UI View")]
    public sealed class AuthUIView : BaseUIView
    {
        #region Fields

        [Header("Login UI")]
        [SerializeField] private GameObject _loginPanel;
        
        [Space(5.0f)]
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _openRegisterButton;

        [Space(5.0f)] 
        [SerializeField] private TMP_InputField _loginEmailInputField;
        [SerializeField] private TMP_InputField _loginPasswordInputField;
        
        [Header("Register UI")]
        [SerializeField] private GameObject _registerPanel;

        [Space(5.0f)]
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _openLoginButton;

        [Space(5.0f)]
        [SerializeField] private TMP_InputField _registerEmailInputField;
        [SerializeField] private TMP_InputField _registerNameInputField;
        [SerializeField] private TMP_InputField _registerPasswordInputField;

        [Header("Notification UI")]
        [SerializeField] private TMP_Text _notificationText;
        
        #region Properties

        public string LoginEmailInput => _loginEmailInputField.text;

        public string LoginPasswordInput => _loginPasswordInputField.text;
        
        public string RegisterEmailInput => _registerEmailInputField.text;

        public string RegisterNameInput => _registerNameInputField.text;

        public string RegisterPasswordInput => _registerPasswordInputField.text;

        #endregion
        
        #endregion

        #region Login

        public void BindLoginButtonClicked(bool bind, UnityAction action)
        {
            if (bind)
            {
                _loginButton.onClick.AddListener(action);
                return;
            }
            
            _loginButton.onClick.RemoveListener(action);
        }
        
        public void BindOpenLoginButtonClicked(bool bind, UnityAction action)
        {
            if (bind)
            {
                _openLoginButton.onClick.AddListener(action);
                return;
            }

            _openLoginButton.onClick.RemoveListener(action);
        }

        public void SetLoginEmailInput(string email)
        {
            _loginEmailInputField.text = email;
        }

        public void SetLoginPasswordInput(string password)
        {
            _loginPasswordInputField.text = password;
        }

        public void OpenLoginPanel()
        {
            _registerPanel.Deactivate();
            _loginPanel.Activate();
        }
        
        #endregion

        #region Register

        public void BindRegisterButtonClicked(bool bind, UnityAction action)
        {
            if (bind)
            {
                _registerButton.onClick.AddListener(action);
                return;
            }
            
            _registerButton.onClick.RemoveListener(action);
        }

        public void BindOpenRegisterButtonClicked(bool bind, UnityAction action)
        {
            if (bind)
            {
                _openRegisterButton.onClick.AddListener(action);
                return;
            }

            _registerButton.onClick.RemoveListener(action);
        }

        public void OpenRegisterPanel()
        {
            _loginPanel.Deactivate();
            _registerPanel.Activate();
        }

        #endregion

        public void SetNotificationText(string text, Color color)
        {
            _notificationText.color = color;
            _notificationText.SetText(text);
        }
    }
}