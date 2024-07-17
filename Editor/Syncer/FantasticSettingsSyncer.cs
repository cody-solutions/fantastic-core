using FantasticCore.Runtime;
using FantasticCore.Runtime.Debug;
using UnityEditor;

namespace FantasticCore.Editor.Syncer
{
    internal static class FantasticSettingsSyncer
    {
        [MenuItem("FantasticCore/Sync Project Settings")]
        public static void SyncProjectSettings()
        {
            EditorUtility.DisplayProgressBar(FantasticProperties.PackageName,
                "Sync project settings...", 0.5f);
            EditorUtility.ClearProgressBar();
            FantasticDebug.Logger.LogMessage("Fantastic project settings was synced successfully!");
        }
    }
}