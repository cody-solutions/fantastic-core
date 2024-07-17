using UnityEditor;
using UnityEngine;
using FantasticCore.Runtime.Configuration.Core;

namespace FantasticCore.Editor.Configuration.Core
{
    [CustomEditor(typeof(FantasticCoreConfig))]
    public class FantasticCoreConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var coreConfig = (FantasticCoreConfig)target;
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Changes"))
            {
                coreConfig.ApplyChanges();
            }
        }
    }
}