using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Modules.User;
using FantasticCore.Runtime.Modules.Auth;
using FantasticCore.Runtime.Modules.Orientation;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Assets_Provider;
using FantasticCore.Runtime.Modules.Matchmaking;
using FantasticCore.Runtime.Modules.Network;
using FantasticCore.Runtime.Modules.Orchestration;
using FantasticCore.Runtime.Modules.Socket;
using FantasticCore.Runtime.Utils;

namespace FantasticCore.Runtime.Modules
{
    [SelectionBase, DisallowMultipleComponent]
    [AddComponentMenu("FantasticCore/Modules/Modules Holder")]
    internal sealed class FantasticModulesHolder : MonoBehaviour
    {
        #region Fields

        private Dictionary<Type, IFantasticModule> _registeredModules = new();

        #region Properties

        private bool IsSetup { get; set; }

        #endregion
        
        #endregion

        #region MonoBehaviour

        private async void OnDestroy()
        {
            await ResetHolder();
        }

        #endregion
        
        internal async UniTask<FantasticModulesHolder> SetupModules(FantasticCoreConfig coreConfig)
        {
            if (IsSetup)
            {
                FantasticDebug.Logger.LogMessage("FantasticModuleHolder is already setup!", FantasticLogType.WARN);
                return this;
            }

            _registeredModules = new Dictionary<Type, IFantasticModule>();
            var tasks = new List<UniTask>();

            if (coreConfig.IncludeAssetsProviderModule)
                tasks.Add(HandleRegister<IFantasticAssetsProvider>(new FantasticAssetsProvider()));

            if (coreConfig.IncludeUserModule)
                tasks.Add(HandleRegister<IFantasticUser>(new FantasticUser()));

            if (coreConfig.IncludeAuthModule)
                tasks.Add(HandleRegister<IFantasticAuth>(new FantasticAuth()));

            if (coreConfig.IncludeNetworkModule)
                tasks.Add(HandleRegister<IFantasticNetwork>(new FantasticNetwork()));

            if (coreConfig.IncludeOrchestrationModule)
                tasks.Add(HandleRegister<IFantasticOrchestration>(new FantasticOrchestration()));

            if (coreConfig.IncludeMatchmakingModule)
                tasks.Add(HandleRegister<IFantasticMatchmaking>(new FantasticMatchmaking()));

            if (coreConfig.IncludeSocketModule)
                tasks.Add(HandleRegister<IFantasticSocket>(new FantasticSocket()));

            if (coreConfig.IncludeOrientationModule)
                tasks.Add(HandleRegister<IFantasticOrientation>(gameObject.AddComponent<FantasticOrientation>()));

            await UniTask.WhenAll(tasks);

            IsSetup = true;
            FantasticDebug.Logger.LogMessage("FantasticModulesHolder with registered modules are initialized successfully!");
            return this;

            async UniTask HandleRegister<T>(IFantasticModule instance) where T : class, IFantasticModule
            {
                if (IsModuleRegistered(typeof(T)) || !ValidDependencies())
                {
                    // TODO: Dispose/delete module instance or move this check to HandleRegister() params
                    return;
                }

                await RegisterModule<T>(instance);
                return;

                bool ValidDependencies()
                {
                    if (instance.ModulesDependencies == null)
                    {
                        return true;
                    }

                    if (instance.ModulesDependencies.All(IsModuleRegistered))
                    {
                        return true;
                    }

                    //TODO: Add to log only missing dependencies (now all dependencies will logged)
                    FantasticDebug.Logger.LogMessage(
                        $"Skip registering {typeof(T).Name} module." +
                        $" Missing dependencies: {FantasticJson.Parse(instance.ModulesDependencies)}",
                        FantasticLogType.ERROR);
                    return false;
                }
            }
        }
        
        public bool TryGetModule<T>(out T module) where T : class, IFantasticModule
        {
            if (!IsSetup)
            {
                FantasticDebug.Logger.LogMessage("FantasticModuleHolder is not setup yet. Please setup it first!",
                    FantasticLogType.WARN);
                module = null;
                return false;
            }

            Type type = typeof(T);
            if (_registeredModules.TryGetValue(type, out IFantasticModule foundModule))
            {
                module = (T)foundModule;
                return true;
            }

            FantasticDebug.Logger.LogMessage($"Can't find registered {typeof(T).Name} module. You need register it first!",
                FantasticLogType.ERROR);
            module = null;
            return false;
        }
        
        public bool IsModuleRegistered(Type type)
            => _registeredModules.ContainsKey(type);

        private async UniTask<bool> RegisterModule<T>(IFantasticModule instance) where T : class, IFantasticModule
        {
            await instance.InitializeModule();
            return _registeredModules.TryAdd(typeof(T), instance);
        }

        // ReSharper disable once UnusedMember.Local
        [Obsolete("Not implemented now", true)]
        private async UniTask<bool> UnRegisterModule(Type type)
        {
            if (!_registeredModules.TryGetValue(type, out IFantasticModule module))
            {
                return false;
            }

            await module.ResetModule();
            _registeredModules.Remove(type);
            return true;
        }

        private async UniTask ResetHolder()
        {
            if (!IsSetup)
            {
                return;
            }

            IsSetup = false;
            foreach (IFantasticModule module in _registeredModules.Values)
            {
                await module.ResetModule();
            }
        }
    }
}