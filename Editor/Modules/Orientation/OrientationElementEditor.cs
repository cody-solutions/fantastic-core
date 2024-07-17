using FantasticCore.Runtime.Modules.Orientation.Elements;
using UnityEditor;
using UnityEngine;

namespace FantasticCore.Editor.Modules.Orientation
{
    [CustomEditor(typeof(OrientationElementBase), true)]
    [CanEditMultipleObjects]
    public class OrientationElementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Label("Current orientation: " + (OrientationElementBase.IsVertical ? "Vertical" : "Horizontal"));
            DrawDefaultInspector();

            Object[] controllers = targets;
            if (GUILayout.Button("Save values"))
                foreach(Object controller in controllers)
                    ((OrientationElementBase)controller).SaveCurrentState();
				
            if (GUILayout.Button("Put values"))
                foreach (Object controller in controllers)
                    ((OrientationElementBase)controller).PutCurrentState();
				
            serializedObject.ApplyModifiedProperties();
        }
    }
}