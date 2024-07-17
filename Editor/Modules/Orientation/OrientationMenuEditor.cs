using System.IO;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Orientation;
using FantasticCore.Runtime.Modules.Orientation.Data;
using FantasticCore.Runtime.Modules.Orientation.Elements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FantasticCore.Editor.Modules.Orientation
{
    [InitializeOnLoad]
    public static class OrientationMenuEditor
    {
        private static void Initialize()
        {
            EditorSceneManager.sceneOpened += OnSceneLoaded;
            EditorApplication.update += UpdateOrientation;

            SetOrientation(OrientationUtils.GetSettings().DefaultOrientation);
        }

        [MenuItem("FantasticCore/Modules/Orientation/Invert Orientation")]
        private static void InvertOrientation()
        {
            if (EditorApplication.isPlaying)
            {
                FantasticDebug.Logger.LogMessage("Not available in play mode!", FantasticLogType.ERROR);
                return;
            }
            
            SetOrientation(!OrientationElementBase.IsVertical);
        }
        
        private static async void OnSceneLoaded(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Additive)
            {
                return;
            }

            if (!OrientationUtils.GetSettings().RefreshOnSceneLoad)
            {
                return;
            }

            await UniTask.NextFrame();

            int height = Screen.height;
            int width = Screen.width;
            
            FantasticDebug.Logger.LogMessage($"Vertical orientation is {OrientationElementBase.IsVertical}. " +
                                             $"Height: {height}," +
                                             $" Width: {width}.");
            
            SetCurrentOrientation();
        }

        private static void UpdateOrientation()
        {
            if (OrientationUtils.GetSettings().EnabledAutomaticOnEditor) SetCurrentOrientation();
        }

        [MenuItem("FantasticCore/Modules/Orientation/Generate Config")]
        private static async void GenerateConfig()
        {
            const string path = "Assets/Resources/SO_GeneratedOrientationConfig.asset";
            const string pathResources = "Assets/Resources";

            if (!Directory.Exists(pathResources))
            {
                Directory.CreateDirectory(pathResources);
            }

            await UniTask.NextFrame();

            var config = ScriptableObject.CreateInstance<OrientationModuleConfig>();
            
            config.SetSetting(OrientationUtils.GetDefaultSettings());
            
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;
            
            AssetDatabase.Refresh();
        }

        private static async void SetCurrentOrientation()
        {
            await UniTask.NextFrame();
            
            bool vertical = Screen.height > Screen.width;
            if (vertical != OrientationElementBase.IsVertical)
            {
                SetOrientation(vertical);
            }
        }

        private static void SetOrientation(bool isVertical)
        {
            FantasticDebug.Logger.LogMessage($"Set Orientation - {isVertical}");
            OrientationElementBase.FireOrientationChanged(null, isVertical);
        }

        static OrientationMenuEditor() => Initialize();
    }
}
