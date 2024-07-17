using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Debug.Logger;
using UnityEngine;

namespace FantasticCore.Runtime.Debug
{
    public static class FantasticDebug
    {
        #region Fields

        private const string DebugConsoleKey = "P_DebugConsole";

        #region Properties

        public static IFantasticLogger Logger { get; } = new FantasticLogger();

        #endregion
        
        #endregion

        internal static void InitializeDebug(FantasticCoreConfig coreConfig)
        {
            Logger.Setup();
            if (NeedCreateDebugConsole())
            {
                CreateDebugConsole();
            }

            return;
            bool NeedCreateDebugConsole()
            {
                return coreConfig.RuntimeType == FantasticRuntimeType.DEVELOPMENT && coreConfig.EnableDebugConsole;
            }
        }

        private static void CreateDebugConsole()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(DebugConsoleKey);
            request.completed += _ =>
            {
                if (!request.asset)
                {
                    Logger.LogMessage($"Failed to load debug console. Check {DebugConsoleKey} key in Resources folder!");
                    return;
                }

                var console = (GameObject)Object.Instantiate(request.asset);
                if (!console)
                {
                    Logger.LogMessage("Spawn debug console failed!");
                }
            };
        }
    }
}