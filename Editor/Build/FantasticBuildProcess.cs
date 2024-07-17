using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using FantasticCore.Runtime;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Games.Configuration;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Editor.Build
{
    /// <summary>
    /// Setup ProjectSettings with FantasticCore during Unity build process
    /// </summary>
    internal sealed class FantasticBuildProcess : IPreprocessBuildWithReport
    {
        #region Properties

        /// <summary>
        /// IPreprocessBuildWithReport callback order
        /// </summary>
        public int callbackOrder => 0;

        #endregion

        /// <summary>
        /// Called before build
        /// </summary>
        /// <param name="report"></param>
        public void OnPreprocessBuild(BuildReport report)
            => ExecuteFantasticBuild();

        private static void ExecuteFantasticBuild()
        {
            FantasticCoreConfig coreConfig = FantasticCoreConfig.GetCurrent();
            if (!coreConfig)
            {
                return;
            }

            if (!coreConfig.InitializeFantasticCore)
            {
                FantasticDebug.Logger.LogMessage("Skip FantasticCore build preprocess. Initialization disabled in FantasticCoreConfig.");
                return;
            }

            FantasticDebug.Logger.LogMessage("FantasticCore preprocess build started...");
            FantasticGameConfig gameConfig = FantasticCoreConfig.GetActiveGameConfig(coreConfig);
            if (!gameConfig)
            {
                FantasticDebug.Logger.LogMessage("Skip FantasticCore build preprocess. GameConfig is not valid!");
                return;
            }

            PlayerSettings.companyName = FantasticProperties.CompanyName;
            PlayerSettings.productName = gameConfig.Name;

            PlayerSettings.applicationIdentifier = gameConfig.OverrideBundleName.IsValid()
                ? gameConfig.OverrideBundleName
                : $"com.battle.{gameConfig.Name}";
            Debug.Log(PlayerSettings.applicationIdentifier);
            
            PlayerSettings.bundleVersion = gameConfig.Version;
            PlayerSettings.macOS.buildNumber = gameConfig.BuildNumber.ToString();
            PlayerSettings.Android.bundleVersionCode = gameConfig.BuildNumber;
            PlayerSettings.iOS.buildNumber = gameConfig.BuildNumber.ToString();
            PlayerSettings.forceSingleInstance = coreConfig.RuntimeType == FantasticRuntimeType.PRODUCTION;
            
            OnBuildProcessFinished();
        }

        private static void OnBuildProcessFinished()
            => FantasticDebug.Logger.LogMessage("FantasticCore preprocess build finished successfully!");
    }
}