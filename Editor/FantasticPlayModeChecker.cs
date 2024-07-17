using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using FantasticCore.Runtime;
using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Editor
{
    /// <summary>
    /// Fantastic editor PlayMode checker
    /// </summary>
    [InitializeOnLoad]
    internal static class FantasticPlayModeChecker
    {
        #region Constructor

        /// <summary>
        /// Ensure that Fantastic game load from GameBootDev scene in develop environment
        /// </summary>
        static FantasticPlayModeChecker()
        {
            EditorSceneManager.playModeStartScene = null;
            FantasticCoreConfig coreConfig = FantasticCoreConfig.GetCurrent(false);
            if (!coreConfig)
            {
                return;
            }

            if (!NeedCheck())
            {
                return;
            }

            if (EditorBuildSettings.scenes.Length == 0)
            {
                FantasticDebug.Logger.LogMessage("Failed open boot game dev scene. Please check your scene setup!", FantasticLogType.ERROR);
                return;
            }

            if (SceneManager.GetActiveScene().buildIndex == FantasticProperties.BootSceneBuildIndex)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
                EditorBuildSettings.scenes[FantasticProperties.BootSceneBuildIndex].path);
            return;

            // Need check if FantasticCore initialization process enabled with play mode validation
            bool NeedCheck() => coreConfig.InitializeFantasticCore && coreConfig.ValidatePlayMode;
        }

        #endregion
    }
}