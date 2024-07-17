using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Assets_Provider.Loader;

namespace FantasticCore.Runtime.Modules.Assets_Provider
{
    /// <summary>
    /// Fantastic Assets Provider Module Implementation
    /// </summary>
    internal sealed class FantasticAssetsProvider : IFantasticAssetsProvider
    {
        #region Fields

        private Dictionary<Type, object> _registeredLoaders;

        #region Properties

        public IEnumerable<Type> ModulesDependencies => null;

        #endregion
        
        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
        {
            _registeredLoaders = new Dictionary<Type, object>();
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            _registeredLoaders = null;
            await UniTask.CompletedTask;
        }

        #endregion

        public void RegisterAssetLoader<T>() where T : IBaseAssetLoader, new()
        {
            if (IsLoaderRegistered<T>())
            {
                FantasticDebug.Logger.LogMessage($"{typeof(T).Name} loader already registered!",
                    FantasticLogType.WARN);
                return;
            }

            _registeredLoaders.Add(typeof(T), new T());
        }

        public void UnRegisterAssetLoader<T>() where T : IBaseAssetLoader
        {
            if (!IsLoaderRegistered<T>())
            {
                return;
            }

            _registeredLoaders.Remove(typeof(T));
        }

        public bool TryGetAssetLoader<T>(out T loader) where T : class, IBaseAssetLoader
        {
            if (!IsLoaderRegistered<T>(true))
            {
                loader = null;
                return false;
            }

            loader = _registeredLoaders[typeof(T)] as T;
            return true;
        }

        public bool IsLoaderRegistered<T>(bool logErrorIfNot = false) where T : IBaseAssetLoader
        {
            bool registered = _registeredLoaders.ContainsKey(typeof(T));
            if (!registered && logErrorIfNot)
            {
                FantasticDebug.Logger.LogMessage($"{typeof(T).Name} loader is not registered!",
                    FantasticLogType.ERROR);
            }
            return registered;
        }
    }
}