using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using FantasticCore.Runtime.Fast_Play;

namespace FantasticCore.Editor.Fast_Play
{
    public class FastPlatEditor : EditorWindow
    {
        #region Fields

        private const string EmailSaveKey = "FAST_PLAY_EMAIL";
        private const string PasswordSaveKey = "FAST_PLAY_PASSWORD";

        private FastPlayData _fastPlayData;

        #endregion

        [MenuItem("FantasticCore/Fast Play")]
        public static void ShowWindow()
            => GetWindow<FastPlatEditor>("Fantastic Fast Play");

        private void OnEnable()
            => LoadData();

        private void OnGUI()
        {
            GUILayout.Label("Fast Play Data", EditorStyles.boldLabel);

            _fastPlayData.Email = EditorGUILayout.TextField("Email", _fastPlayData.Email);
            _fastPlayData.Password = EditorGUILayout.PasswordField("Password", _fastPlayData.Password);

            GUILayout.Space(30.0f);
            if (GUILayout.Button("Save Data"))
            {
                SaveData();
            }

            if (GUILayout.Button("Load Data"))
            {
                LoadData();
            }
            
            if (GUILayout.Button("Clear Data"))
            {
                ClearData();
            }
        }

        // private void Start()
        // {
        //     if (EditorApplication.isPlaying)
        //     {
        //         EditorApplication.ExitPlaymode();
        //     }
        //
        //     EditorPrefs.SetString(FantasticFastPlay.FastPlayDataEditorSaveKey, JsonConvert.SerializeObject(_fastPlayData));
        //     EditorApplication.EnterPlaymode();
        // }

        private void SaveData()
        {
            EditorPrefs.SetString(EmailSaveKey, _fastPlayData.Email);
            EditorPrefs.SetString(PasswordSaveKey, _fastPlayData.Password);
            SaveCurrentDataToEditor();
        }

        private void LoadData()
        {
            _fastPlayData = new FastPlayData
            {
                Email = EditorPrefs.GetString(EmailSaveKey, "email"),
                Password = EditorPrefs.GetString(PasswordSaveKey)
            };
            SaveCurrentDataToEditor();
        }

        private void ClearData()
        {
            _fastPlayData = new FastPlayData();
            DeleteCurrentDataFromEditor();
        }

        private void SaveCurrentDataToEditor()
        {
            EditorPrefs.SetString(FantasticFastPlay.FastPlayDataEditorSaveKey, JsonConvert.SerializeObject(_fastPlayData));
            Repaint();
        }

        private void DeleteCurrentDataFromEditor()
        {
            EditorPrefs.DeleteKey(FantasticFastPlay.FastPlayDataEditorSaveKey);
            Repaint();
        }
    }
}