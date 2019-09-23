using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public class SoundStorageEditor : ScriptableWizard
    {
        private bool foldout;
        protected Vector2 scrollPos = Vector2.zero;
        public Texture2D BackgroundImage { get; private set; }

        private const string titleString = "Voiceer - Voice Preset Editor";

        private static string _presetName = "NewVoicePreset";
        private static string _outputDirectory = "Assets/Voiceer/ScriptableObject/";
        private static readonly string _outputSuffix = ".asset";

        private VoicePreset loadedPreset;

        [MenuItem("Window/Voiceer")]
        private static void Open()
        {
            DisplayWizard<SoundStorageEditor>("Voiceer - Voice Preset Editor");

        }

        private void OnGUI()
        {
            if (BackgroundImage == null)
            {
                BackgroundImage = AssetDatabase.LoadAssetAtPath("Assets/Voiceer/Editor/Res/bg.png", typeof(Texture2D)) as Texture2D;
            }

            VEUtil.UiTitleBox(titleString, BackgroundImage);

            if (loadedPreset == null)
            {
                LoadScriptableObjectUIWhenPresetIsNull();
                return;
            }

            loadedPreset.hideFlags = HideFlags.NotEditable;

            LoadScriptableObjectUIWhenPresetIsNotNull();

            GUILayout.Space(20);

            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);


            EditorGUI.BeginChangeCheck();
            foreach (Trigger trigger in Enum.GetValues(typeof(Trigger)))
            {
                if (loadedPreset.GetVoiceSet(trigger) == null)
                {
                    loadedPreset.voiceSetList.Add(new VoicePreset.Set(trigger, new Sound()));
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(VEUtil.GetDescription(trigger), VEUtil.Bold);

                var clips = loadedPreset.GetVoiceSet(trigger).voiceClips;

                if (loadedPreset.GetVoiceSet(trigger).voiceClips.Count != 0)
                {
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        clips.RemoveRange(clips.Count - 1, 1);
                    }
                }

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    clips.Add(null);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                for (int i = 0; i < loadedPreset.GetVoiceSet(trigger).voiceClips.Count; i++)
                {
                    loadedPreset.GetVoiceSet(trigger).voiceClips[i] =
                        (AudioClip)EditorGUILayout.ObjectField(loadedPreset.GetVoiceSet(trigger).voiceClips[i], typeof(AudioClip), false);
                }

                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(loadedPreset);
                AssetDatabase.SaveAssets();
                // エディタを最新の状態にする
                AssetDatabase.Refresh();
            }

            GUILayout.EndScrollView();


            GUILayout.Space(10);

            if (GUILayout.Button("Preset選択画面に戻る"))
            {
                loadedPreset = null;
                loadedPreset?.ClearAll();
                loadedPreset = null;
            }
        }

        void LoadScriptableObjectUIWhenPresetIsNotNull()
        {
            EditorGUILayout.LabelField("各+ボタンを押して、任意のAudioClipを追加してください。", VEUtil.Bold);
            EditorGUILayout.LabelField("(変更内容はオートセーブされます。)");
            VEUtil.hr(this.position.width);
            loadedPreset = (VoicePreset)EditorGUILayout.ObjectField("現在のPreset", loadedPreset, typeof(VoicePreset), false);
            GUILayout.Space(10);
        }

        void LoadScriptableObjectUIWhenPresetIsNull()
        {
            EditorGUILayout.LabelField("VoicePresetをロードするか、新規作成してください。", VEUtil.Bold);


            _presetName = EditorGUILayout.TextField(_presetName);
            _outputDirectory = EditorGUILayout.TextField(_outputDirectory);

            EditorGUILayout.LabelField("出力先：" + Path.Combine(_outputDirectory, _presetName + _outputSuffix), VEUtil.Bold);

            if (GUILayout.Button("Create New"))
            {
                var path = CreateNewScriptable();
                loadedPreset = AssetDatabase.LoadAssetAtPath(path, typeof(VoicePreset)) as VoicePreset;
            }

            GUILayout.Space(10);
            VEUtil.hr(this.position.width);
            GUILayout.Space(10);

            loadedPreset = (VoicePreset)EditorGUILayout.ObjectField("または既存のPresetをロード", loadedPreset, typeof(VoicePreset), false);
        }

        private void SafeCreateDirectory(string path)
        {
            var currentPath = "";
            var splitChar = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

            foreach (var dir in path.Split(splitChar))
            {
                var parent = currentPath;
                currentPath = Path.Combine(currentPath, dir);
                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(parent, dir);
                }
            }
        }

        private string CreateNewScriptable()
        {
            SafeCreateDirectory(_outputDirectory);
            var createdObject = CreateInstance("VoicePreset");
            var outputPath = Path.Combine(_outputDirectory, _presetName + _outputSuffix);
            var uniqueOutputPath = AssetDatabase.GenerateUniqueAssetPath(outputPath);
            AssetDatabase.CreateAsset(createdObject, uniqueOutputPath);
            AssetDatabase.Refresh();
            return uniqueOutputPath;
        }
    }
}