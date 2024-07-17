using FantasticNetShared.Data.Auth;
using FantasticNetShared.Data.User;
using FantasticCore.Runtime.Modules.Auth;

namespace FantasticCore.Runtime.Local
{
    public sealed class FantasticLocal
    {
        #region Properties

        public static FantasticLocal Current => FantasticInstance.Local;

        public LoggedState LoggedState { get; private set; }

        public long UserId => User.Id;

        public string Token => Auth?.IdToken;

        public string RefreshToken => Auth?.RefreshToken;

        internal long UserAuthRequestId => UserAuthRequest.Id;
        
        private UserDTO User { get; set; }

        private UserAuthRequestDTO UserAuthRequest { get; set; }

        private AuthResponseDTO Auth { get; set; }
        
        #endregion

        internal void SetLoggedState(LoggedState loggedState)
            => LoggedState = loggedState;
        
        internal void SetUser(UserDTO user)
            => User = user;

        internal void SetUserAuthRequest(UserAuthRequestDTO userAuthRequest)
            => UserAuthRequest = userAuthRequest;
        
        internal void SetAuthResponse(AuthResponseDTO authResponse)
            => Auth = authResponse;

        internal void UpdateToken(RefreshTokenResponseDTO refreshToken)
        {
            if (Auth == null)
            {
                return;
            }

            Auth.IdToken = refreshToken.IdToken;
            Auth.RefreshToken = refreshToken.RefreshToken;
            Auth.ExpiresIn = refreshToken.ExpiresIn;
        }

        internal void ResetAfterLogout()
        {
            SetLoggedState(LoggedState.NONE);
            User = null;
            UserAuthRequest = null;
            Auth = null;
        }
    }
}