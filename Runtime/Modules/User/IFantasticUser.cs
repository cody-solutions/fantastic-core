using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.User.Data;

namespace FantasticCore.Runtime.Modules.User
{
    public interface IFantasticUser : IFantasticModule
    {
        public OperationHandleData<string> GetUserName(string userId);

        public OperationHandleData<UserData> GetUser(string userId);
    }
}