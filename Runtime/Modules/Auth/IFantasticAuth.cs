using System.Threading;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.Auth.Data;
using FantasticNetShared.Data.Auth;
using FantasticNetShared.Data.User;

namespace FantasticCore.Runtime.Modules.Auth
{
    /// <summary>
    /// Fantastic Auth Module
    /// </summary>
    public interface IFantasticAuth : IFantasticModule
    {
        /// <summary>
        /// Sign-in to Fantastic
        /// </summary>
        /// <param name="authRequest"></param>
        /// <param name="saveRefreshToken"></param>
        /// <returns></returns>
        public OperationHandleData<AuthResponseRootDTO> SignIn(AuthRequestData authRequest, bool saveRefreshToken = true);

        /// <summary>
        /// Try sign-in to Fantastic with saved refresh token
        /// </summary>
        /// <returns></returns>
        public OperationHandleData<AuthResponseRootDTO> TrySignInWithSavedRefreshToken(bool saveRefreshToken = true);
        
        /// <summary>
        /// Task for Fantastic successfully authentication
        /// </summary>
        /// <returns></returns>
        public UniTask WaitForFullyLogged(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sign-up in Fantastic
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        public OperationHandleData<UserAuthRequestDTO> SignUp(AuthRequestData authRequest);

        /// <summary>
        /// Confirm sign-up in Fantastic
        /// </summary>
        /// <returns></returns>
        public OperationHandleData<UserDTO> SignUpConfirm(string login, int? pfpConfigId, bool autoRefreshToken = true);

        /// <summary>
        /// Refresh current token
        /// </summary>
        /// <returns></returns>
        public OperationHandleData<RefreshTokenResponseDTO> UpdateToken();

        /// <summary>
        /// End current session
        /// </summary>
        public void Logout();
        
        #region Save

        /// <summary>
        /// Save refresh token locally
        /// </summary>
        /// <param name="refreshToken"></param>
        public void SaveRefreshToken(string refreshToken);

        /// <summary>
        /// Load saved refresh token locally
        /// </summary>
        /// <returns></returns>
        public string GetSavedRefreshToken();

        #endregion
    }
}