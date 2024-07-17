using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.User.Data;

namespace FantasticCore.Runtime.Modules.User
{
    internal sealed class FantasticUser : IFantasticUser
    {
        #region Properties

        public IEnumerable<Type> ModulesDependencies => null;

        #endregion

        #region Update

        async UniTask IFantasticModule.InitializeModule()
        {
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            await UniTask.CompletedTask;
        }

        #endregion

        public OperationHandleData<string> GetUserName(string userId)
        {
            throw new NotImplementedException();
        }

        public OperationHandleData<UserData> GetUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}