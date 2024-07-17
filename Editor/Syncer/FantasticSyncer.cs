using FantasticCore.Editor.Package;
using UnityEditor;

namespace FantasticCore.Editor.Syncer
{
    internal static class FantasticSyncer
    {
        #region Fields

        private const string SessionSyncerCheckedKey = "FANTASTIC_CORE_SYNCER";

        #endregion
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            bool @checked = EditorPrefs.GetBool(SessionSyncerCheckedKey);
            if (@checked)
            {
                return;
            }

            EditorPrefs.SetBool(SessionSyncerCheckedKey, true);
            FantasticDependenciesSyncer.SyncDependencies();
            FantasticSettingsSyncer.SyncProjectSettings();
        }
    }
}