using System.IO;
using Services.EnumGeneration;
using UnityEditor;
using UnityEngine;

namespace Services.AudioSystem.Editor
{
    [CustomEditor(typeof(SFXPlayer))]
    [CanEditMultipleObjects]
    public class SFXPlayerEditor : UnityEditor.Editor
    {
        static string path = "Assets/Resources/Configs";
        static string folderName = "SFX_Configs";
        static string fileName = "SFXPlayer.asset";


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Update Configs"))
            {
                SFXPlayer target = serializedObject.targetObject as SFXPlayer;

                if (target == null) return;

                for (int i = 0; i < target.SoundsConfigs.Length; i++)
                {
                    target.SoundsConfigs[i].SetupPlayer(target);
                }
            }

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        private void GenerateEffectsEnum()
        {
            SFXPlayer target = serializedObject.targetObject as SFXPlayer;

            if (target == null) return;
            string[] sounds = new string[target.SoundsConfigs.Length];

            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = target.SoundsConfigs[i].name;
            }


            EnumGenerator.GenerateEnum("SoundType", Path.Combine("Assets/Scripts/Services/AudioSystem/"), sounds);
        }

        [MenuItem("R-Quest Tools/Effects/Open SFX Player")]
        private static void CreatePresenter()
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(path, folderName)))
            {
                AssetDatabase.CreateFolder(path, folderName);
            }

            SFXPlayer asset = AssetDatabase.LoadAssetAtPath<SFXPlayer>(Path.Combine(path, folderName, fileName));
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<SFXPlayer>();
                AssetDatabase.CreateAsset(asset, Path.Combine(path, folderName, fileName));
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}