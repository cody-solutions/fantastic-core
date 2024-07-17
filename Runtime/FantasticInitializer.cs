using UnityEngine;
using FantasticCore.Runtime.Configuration.Core;

namespace FantasticCore.Runtime
{
    /// <summary>
    /// Execute FantasticCore initialization in runtime
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    internal static class FantasticInitializer
    {
        /// <summary>
        /// Called before scene load. Load active FantasticCoreConfig from Resources and initialize FantasticInstance with that
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            FantasticCoreConfig coreConfig = FantasticCoreConfig.GetCurrent(false);
            if (!coreConfig)
            {
                return;
            }

            if (!coreConfig.InitializeFantasticCore)
            {
                return;
            }
            
            FantasticInstance.InitializeCore(coreConfig);
        }
    }
}