using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Modules.Socket.Data;

namespace FantasticCore.Runtime.Modules.Socket
{
    /// <summary>
    /// Fantastic Socket Module Implementation
    /// </summary>
    internal sealed class FantasticSocket : IFantasticSocket
    {
        #region Fields

        private SocketClient _client;
        private SocketPackageHandler _packageHandler;
        
        #region Properties

        public IEnumerable<Type> ModulesDependencies => null;

        #endregion

        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
        {
            _packageHandler = new SocketPackageHandler();
            _client = new SocketClient(_packageHandler.HandlePackage);
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            _client = null;
            _packageHandler.ResetHandler();
            _packageHandler = null;
            await UniTask.CompletedTask;
        }

        #endregion

        public void SendMessage(SocketPackageData packageData)
            => _client.SendMessage(packageData);
        
        public void BindListeningPackage<T>(bool state, Action<T> action) where T : SocketPackage
            => _packageHandler.BindListeningPackage(state, action);

        public void RegisterPackage<T>() where T : SocketPackage
            => _packageHandler.RegisterPackage<T>();
    }
}