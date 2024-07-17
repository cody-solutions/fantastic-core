using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using FantasticCore.Runtime;

namespace FantasticCore.Editor
{
    internal static class FantasticEditorUtilities
    {
        [MenuItem("FantasticCore/Tools/Reload Domain &#r")]
        private static void ReloadDomain()
        {
            EditorUtility.RequestScriptReload();
        }

        [MenuItem("FantasticCore/Tools/Clear cache")]
        private static void ClearCache()
        {
            Caching.ClearCache();
        }

        [MenuItem("FantasticCore/Tools/Clear Player Prefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("FantasticCore/Tools/Clear Console &#c")]
        private static void ClearConsole()
        {
            var logEntries = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            MethodInfo clearMethod = logEntries!.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod!.Invoke(null,null);
        }
    
        [MenuItem("FantasticCore/Tools/Toggle Inspector Lock %l")]
        private static void SelectLockableInspector()
        {
            EditorWindow inspectorToBeLocked = EditorWindow.focusedWindow;
            if (inspectorToBeLocked == null || inspectorToBeLocked.GetType().Name != "InspectorWindow")
            {
                return;
            }

            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
            PropertyInfo propertyInfo = type.GetProperty("isLocked");
            bool value = (bool)propertyInfo!.GetValue(inspectorToBeLocked, null);
            propertyInfo.SetValue(inspectorToBeLocked, !value, null);
            inspectorToBeLocked.Repaint();
        }
    
        [MenuItem("FantasticCore/Tools/Toggle Inspector Mode %m")]
        private static void ToggleInspectorDebug()
        {
            EditorWindow targetInspector = EditorWindow.focusedWindow;
            if (targetInspector == null || targetInspector.GetType().Name != "InspectorWindow")
            {
                return;
            }

            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
            FieldInfo field = type.GetField("m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            var mode = (InspectorMode)field!.GetValue(targetInspector);                     
            mode = (mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);            
                
            MethodInfo method = type.GetMethod("SetMode", BindingFlags.NonPublic | BindingFlags.Instance);          
            method!.Invoke(targetInspector, new object[] {mode});                                                    
            targetInspector.Repaint();
        }

        // [MenuItem("FantasticCore/Open Boot Dev Scene")]
        // ReSharper disable once UnusedMember.Local
        private static void OpenBootDevScene()
        {
            if (Application.isPlaying)
            {
                return;
            }

            try
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[FantasticProperties.BootSceneBuildIndex];
                EditorSceneManager.OpenScene(scene.path);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed open boot game dev scene. Please check your scene setup! Details: {exception.Message}");
            }
        }
    }
}