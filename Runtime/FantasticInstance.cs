using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityObject = UnityEngine.Object;
using Unity.Services.Core;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Games.Configuration;
using FantasticCore.Runtime.API;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules;
using FantasticCore.Runtime.Local;
using FantasticCore.Runtime.Fast_Play;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Games.Loader;

[assembly: AlwaysLinkAssembly, Preserve]
[assembly:
    InternalsVisibleTo("AD_FantasticCoreEditor"),
    InternalsVisibleTo("AD_FantasticCoreEditorTests"),
    InternalsVisibleTo("AD_FantasticCorePlayModeTests")]
namespace FantasticCore.Runtime
{
    public static class FantasticInstance
    {
        #region Propertiess

        /// <summary>
        /// Determine if FantasticCore was initialized successfully
        /// </summary>
        public static bool IsInitialized { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public static FantasticPlatformType FantasticType => CoreConfig.PlatformType;

        public static IFantasticGameLoader GameLoader { get; private set; }

        internal static FantasticLocal Local { get; private set; }
        
        internal static FantasticCoreConfig CoreConfig { get; private set; }

        private static FantasticModulesHolder ModulesHolder { get; set; }

        #endregion

        /// <summary>
        /// Try find and return GameConfig
        /// </summary>
        /// <param name="gameConfigId"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static FantasticGameConfig GetGameConfig(int gameConfigId)
        {
            if (!CheckIfInitializedForAction())
            {
                return null;
            }

            foreach (FantasticGameConfig gameConfig in CoreConfig.Games)
            {
                if (gameConfig.GameConfigId != gameConfigId)
                {
                    continue;
                }
                return gameConfig;
            }

            FantasticDebug.Logger.LogMessage($"Can not find GameConfig with {gameConfigId} game name!");
            return null;
        }
        
        /// <summary>
        /// Initialize FantasticCore package. Initialize FantasticCore environment
        /// </summary>
        /// <param name="config">Core config to initialize FantasticCore</param>
        /// <param name="rootObject">Root FantasticCore gameObject</param>
        internal static async void InitializeCore([DisallowNull] FantasticCoreConfig config, GameObject rootObject = null)
        {
            if (IsInitialized)
            {
                FantasticDebug.Logger.LogMessage("Core is initialized yet!", FantasticLogType.WARN);
                return;
            }

            if (!config)
            {
                FantasticDebug.Logger.LogMessage("Config for initialization is not valid!", FantasticLogType.ERROR);
                return;
            }

            if (!rootObject)
            {
                rootObject = new GameObject($"[{FantasticProperties.PackageName}]");
            }
            
            CoreConfig = config;
            
            FantasticDebug.InitializeDebug(config);
#if UNITY_SERVER
            if (CoreConfig.PlatformType != FantasticPlatformType.SERVER)
            {
                CoreConfig.SetPlatformType(FantasticPlatformType.SERVER);
                FantasticDebug.Logger.LogMessage("Active platform DEDICATED_SERVER but FantasticPlatformType is not SERVER!" +
                                                 " Rebuild with SERVER type. For now it's auto fixed.", FantasticLogType.WARN);
            }
#endif
            
            bool initialized = await InitializeEnvironment(rootObject);
            if (!initialized)
            {
                throw new Exception("FantasticCore environment initialization failed!");
            }

            FantasticDebug.Logger.LogMessage("Core package initialized successfully!");
            IsInitialized = true;
        }

        /// <summary>
        /// Wait FantasticCore full initialization
        /// </summary>
        /// <param name="timeOut">Wait for initialization timeout</param>
        /// <returns>Return true if FantasticCore initialized successfully, else return false</returns>
        internal static async UniTask<bool> WaitCoreInitialization(float timeOut = 5.0f) =>
            !await UniTask.WaitUntil(() => IsInitialized).TimeoutWithoutException(TimeSpan.FromSeconds(timeOut));

#if UNITY_EDITOR
        /// <summary>
        /// Determine editor mode for FantasticCore package
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static async UniTask<bool> IsFantasticEditorClientMode()
        {
            UnityEditor.PackageManager.Requests.ListRequest packages = UnityEditor.PackageManager.Client.List(false, true);
            await UniTask.WaitUntil(() => packages.IsCompleted);
            if (packages.Status == UnityEditor.PackageManager.StatusCode.Failure)
            {
                throw new Exception("Failed get is client mode to check Fantastic dependencies");
            }
            
            return System.Linq.Enumerable.Any(packages.Result,
                package => package.name.Equals(FantasticProperties.FantasticCorePackageName));
        }
#endif

        /// <summary>
        /// Check if FantasticCore was initialized. Return false with log if wasn't
        /// </summary>
        /// <param name="printLogIfNotInitialized"></param>
        /// <returns></returns>
        private static bool CheckIfInitializedForAction(bool printLogIfNotInitialized = true)
        {
            if (IsInitialized)
            {
                return true;
            }

            if (printLogIfNotInitialized)
            {
                FantasticDebug.Logger.LogMessage("Initialize Core package first to execute this!");
            }
            
            return false;
        }

        /// <summary>
        /// Initialize whole FantasticCore environment
        /// </summary>
        /// <param name="rootObject"></param>
        private static async UniTask<bool> InitializeEnvironment(GameObject rootObject = null)
        {
            if (!CoreConfig.APIProfileConfig)
            {
                FantasticDebug.Logger.LogMessage("APIProfileConfig isn't valid! Check FantasticCoreConfig.");
                return false;
            }

            bool success = await InitializeUnityServices();
            if (!success)
            {
                return false;
            }

            Local = new FantasticLocal();
            GameLoader = new AddressableGameLoader();
            
            if (CoreConfig.EnableFastPlay)
            {
                FantasticFastPlay.InitializeFastPlay();
            }

            UnityObject.DontDestroyOnLoad(rootObject);
            FantasticAPIClient.InitializeClient(CoreConfig.APIProfileConfig.Profile.Url, CoreConfig.APIProfileConfig.Profile.Version);
            await CreateAndSetupModulesHolder(rootObject);
            return true;
        }

        /// <summary>
        /// Initialize Unity services with selected environment 
        /// </summary>
        /// <returns></returns>
        private static async UniTask<bool> InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception exception)
            {
                FantasticDebug.Logger.LogMessage($"Unity services initialization failed! Error: {exception}");
                return false;
            }
            return true;
        }

        #region Modules Holder

        /// <summary>
        /// Try get and return registered module from Fantastic ModulesHolder
        /// </summary>
        /// <param name="module"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetModule<T>(out T module) where T : class, IFantasticModule
        {
            if (CheckIfInitializedForAction())
            {
                return ModulesHolder.TryGetModule(out module);
            }

            module = null;
            return false;
        }

        /// <summary>
        /// Determine if concrete module registered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsModulesRegistered<T>() where T : class, IFantasticModule =>
            CheckIfInitializedForAction() && ModulesHolder.IsModuleRegistered(typeof(T));

        /// <summary>
        /// Setup Fantastic ModulesHolder
        /// </summary>
        /// <param name="rootObject"></param>
        private static async UniTask CreateAndSetupModulesHolder(GameObject rootObject) => 
            ModulesHolder = await rootObject.AddComponent<FantasticModulesHolder>()
                .SetupModules(CoreConfig);

        #endregion
    }
}