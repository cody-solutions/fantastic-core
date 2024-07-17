using System;
using UnityEngine;
using FantasticCore.Runtime.API;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Games.Configuration;

namespace FantasticCore.Runtime.Configuration.Core
{
    [CreateAssetMenu(fileName = "SO_FantasticCoreConfig", menuName = "FantasticCore/Core Config", order = 0)]
    public sealed class FantasticCoreConfig : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// Determine if FantasticCore will initialize in runtime
        /// </summary>
        [field: Header("Main"), SerializeField]
        public bool InitializeFantasticCore { get; private set; } = true;

        [field: SerializeField]
        public FantasticPlatformType PlatformType { get; private set; } = FantasticPlatformType.CLIENT;

        [field: SerializeField]
        public FantasticRuntimeType RuntimeType { get; private set; } = FantasticRuntimeType.DEVELOPMENT;

        [field: Header("API"), SerializeField]
        public APIProfileConfig APIProfileConfig { get; private set; }

        [field: Header("Modules"), SerializeField]
        public bool IncludeAssetsProviderModule { get; private set; } = true;
        
        [field: SerializeField]
        public bool IncludeUserModule { get; private set; } = true;

        [field: SerializeField]
        public bool IncludeAuthModule { get; private set; } = true;

        [field: SerializeField]
        public bool IncludeNetworkModule { get; private set; } = true;

        [field: SerializeField]
        public bool IncludeOrchestrationModule { get; private set; } = true;
        [field: SerializeField]
        public bool IncludeMatchmakingModule { get; private set; } = true;

        [field: SerializeField]
        public bool IncludeSocketModule { get; private set; } = true;

        [field: SerializeField]
        public bool IncludeOrientationModule { get; private set; } = true;

        [field: Header("Games"), SerializeField, Tooltip("First GameConfig in array will be active game")]
        public FantasticGameConfig[] Games { get; private set; }

        [field: Header("Play Mode"), SerializeField]
        public bool ValidatePlayMode { get; private set; } = true;

        [field: Header("Fast Play"), SerializeField]
        public bool EnableFastPlay { get; private set; }

        [field: Header("Debug"), SerializeField]
        public bool EnableDebugConsole { get; private set; } = true;

        [field: Header("Configuration"), SerializeField, Tooltip("Determine if FantasticCore boot auth will be skipped")]
        public bool CustomAuthProvider { get; private set; }

        #endregion

        /// <summary>
        /// Try load and return active project FantasticCoreConfig from Resources
        /// </summary>
        /// <returns>Return FantasticCoreConfig if was found</returns>
        /// <exception cref="Exception">Throw if FantasticCoreConfig doesn't exist in Resources</exception>
        public static FantasticCoreConfig GetCurrent(bool throwExceptionIfFailed = true)
        {
            var coreConfig = Resources.Load<FantasticCoreConfig>(FantasticProperties.FantasticCoreConfigKey);
            if (coreConfig)
            {
                return coreConfig;
            }

            if (throwExceptionIfFailed)
            {
                throw new Exception("Can't load FantasticCoreConfig." +
                                    $" Check {FantasticProperties.FantasticCoreConfigKey} key in Resources!");
            }
            
            return null;
        }

        public static FantasticGameConfig GetActiveGameConfig(FantasticCoreConfig coreConfig = null)
        {
            if (!coreConfig)
            {
                coreConfig = GetCurrent();
            }

            if (coreConfig.Games.Length > 0 && coreConfig.Games[0])
            {
                return coreConfig.Games[0];
            }

            throw new Exception("Can't get active GameConfig!");
        }

        public void SetInitializeFantasticCore(bool state)
            => InitializeFantasticCore = state;

        // ReSharper disable once UnusedMember.Global
        public void SetPlatformType(FantasticPlatformType platformType)
            => PlatformType = platformType;

        // ReSharper disable once UnusedMember.Global
        public void SetRuntimeType(FantasticRuntimeType runtimeType)
            => RuntimeType = runtimeType;

#if UNITY_EDITOR
        [ContextMenu("Apply Changes")]
        public void ApplyChanges()
        {
            UnityEditor.EditorUtility.RequestScriptReload();
        }  
#endif
    }
}