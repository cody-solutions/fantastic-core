using FantasticCore.Runtime.API.Data;

namespace FantasticCore.Runtime.Modules.Auth.Data
{
    public sealed class AuthRequestData : APIRequestData
    {
        #region Properties

        public SignUpRequestData Request { get; set; }
        
        #endregion
    }

    public sealed class SignUpRequestData
    {
        #region Properties

        public string Email { get; }

        public string Password { get; }

        #endregion

        #region Constructor

        public SignUpRequestData(string email, string password)
        {
            Email = email;
            Password = password;
        }

        #endregion
    }

    internal sealed class RefreshTokenRequestData : APIRequestData
    {
        #region Properties

        public string RefreshToken { get; }

        public bool ReturnFullResponse { get; }

        #endregion

        #region Constructor

        public RefreshTokenRequestData(string refreshToken, bool returnFullResponse = false)
        {
            RefreshToken = refreshToken;
            ReturnFullResponse = returnFullResponse;
        }

        #endregion
    }

    internal sealed class ConfirmSignUpRequestData : APIRequestData
    {
        #region Properties

        public long AuthRequestId { get; }
        
        public string Login { get; }
        
        public int? PfpConfigId { get; }

        #endregion

        #region Constructor

        public ConfirmSignUpRequestData(long authRequestId, string login, int? pfpConfigId)
        {
            AuthRequestId = authRequestId;
            Login = login;
            PfpConfigId = pfpConfigId;
        }

        #endregion
    }
}