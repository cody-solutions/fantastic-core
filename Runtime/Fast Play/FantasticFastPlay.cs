using Newtonsoft.Json;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Runtime.Fast_Play
{
    internal static class FantasticFastPlay
    {
        #region Fields

        public const string FastPlayDataEditorSaveKey = "FAST_PLAY_DATA";

        #region Properties

        public static FastPlayData FastPlayData { get; private set; }

        public static bool FastPlayEnabled => FastPlayData != null;
        
        #endregion

        #endregion

        internal static void InitializeFastPlay()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorPrefs.HasKey(FastPlayDataEditorSaveKey))
            {
                return;
            }

            string saveData = UnityEditor.EditorPrefs.GetString(FastPlayDataEditorSaveKey);
            UnityEditor.EditorPrefs.DeleteKey(FastPlayDataEditorSaveKey);
            if (saveData.IsNotValid())
            {
                FantasticDebug.Logger.LogMessage("FastPlayData is not valid. Skip FastPlay!", FantasticLogType.WARN);
                return;
            }

            SetFastPlayData(JsonConvert.DeserializeObject<FastPlayData>(saveData));
#endif
            
            // For example in build we can load data from command line arguments or get data from method
        }

        private static void SetFastPlayData(FastPlayData data)
        {
            FastPlayData = data;
        }
    }
}