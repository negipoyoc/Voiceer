using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public class SoundStorageEditor : ScriptableWizard
    {
        Vector2 _scrollPos = Vector2.zero;
        private const string TitleString = "Voiceer - Voice Preset Editor";

        private static string _presetName = "NewPreset";
        private static string _outputDirectory = "Assets/Voiceer/ScriptableObject/";
        private static readonly string _outputSuffix = ".asset";

        private VoicePreset _loadedPreset;

        [MenuItem("Window/Voiceer/Voice Preset Generator")]
        private static void Open()
        {
            DisplayWizard<SoundStorageEditor>("Voiceer");
        }

        private void OnGUI()
        {
            VoiceerEditorUtility.UiTitleBox(TitleString, VoiceerEditorUtility.BackGroundImage);

            if (_loadedPreset == null)
            {
                LoadPresetHeader();
                return;
            }

            EditPresetHeader();
            
            _loadedPreset.hideFlags = HideFlags.NotEditable;
            _loadedPreset.metaData.url = EditorGUILayout.TextField("URL:", _loadedPreset.metaData.url);
            _loadedPreset.metaData.memo = EditorGUILayout.TextField("その他:", _loadedPreset.metaData.memo);

            GUILayout.Space(20);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                EditorGUI.BeginChangeCheck();
                {
                    // 負の値を考慮してEnum型を昇順に列挙するための処理
                    // note: Enum.GetValues(Type)は列挙定数のバイナリ値(符号なしの大きさ)によって並べ替えらるため不適
                    // https://docs.microsoft.com/ja-jp/dotnet/api/system.enum.getvalues?view=netframework-4.8
                    var min = Enum.GetValues(typeof(Hook)).Cast<int>().Min();
                    var max = Enum.GetValues(typeof(Hook)).Cast<int>().Max();
                    for (var index = min; index < max; ++index)
                    {
                        if (!Enum.IsDefined(typeof(Hook), index))
                            continue;
                        var trigger = (Hook)index;

                        if (_loadedPreset.GetVoiceSet(trigger) == null)
                        {
                            _loadedPreset.voiceSetList.Add(new VoicePreset.Set(trigger, new Sound()));
                        }

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(VoiceerEditorUtility.GetDescriptionLabel(trigger),
                            VoiceerEditorUtility.Bold);

                        var clips = _loadedPreset.GetVoiceSet(trigger).voiceClips;

                        if (_loadedPreset.GetVoiceSet(trigger).voiceClips.Count != 0)
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
                        for (int i = 0; i < _loadedPreset.GetVoiceSet(trigger).voiceClips.Count; i++)
                        {
                            _loadedPreset.GetVoiceSet(trigger).voiceClips[i] =
                                (AudioClip) EditorGUILayout.ObjectField(
                                    _loadedPreset.GetVoiceSet(trigger).voiceClips[i],
                                    typeof(AudioClip), false);
                        }

                        EditorGUI.indentLevel--;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_loadedPreset);
                    AssetDatabase.SaveAssets();
                    // エディタを最新の状態にする
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            VoiceerEditorUtility.Hr(position.width);
            if (GUILayout.Button("Preset選択モードに戻る"))
            {
                _loadedPreset = null;
            }
            GUILayout.Space(10);
            if (GUILayout.Button("パッケージを出力する"))
            {
                VoiceerEditorUtility.ExportPackage(_loadedPreset);
            }
        }

        /// <summary>
        /// Presetが存在する時の編集画面のヘッダー
        /// </summary>
        private void EditPresetHeader()
        {
            EditorGUILayout.LabelField("各+ボタンを押して、任意のAudioClipを追加してください。", VoiceerEditorUtility.Bold);
            EditorGUILayout.LabelField("(変更内容はオートセーブされます。)",VoiceerEditorUtility.Red);
            VoiceerEditorUtility.Hr(this.position.width);
            _loadedPreset =
                (VoicePreset) EditorGUILayout.ObjectField("現在のPreset", _loadedPreset, typeof(VoicePreset), false);
            GUILayout.Space(10);
        }

        /// <summary>
        /// Presetが存在しない時のロード画面のヘッダー
        /// </summary>
        void LoadPresetHeader()
        {
            EditorGUILayout.LabelField("VoicePresetをロード", VoiceerEditorUtility.Bold);
            EditorGUI.indentLevel++;
            {
                _loadedPreset =
                    (VoicePreset) EditorGUILayout.ObjectField("既存のPresetをロード", _loadedPreset, typeof(VoicePreset),
                        false);
            }
            EditorGUI.indentLevel--;
            VoiceerEditorUtility.Hr(position.width);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("または");
            GUILayout.Space(10);
            VoiceerEditorUtility.Hr(position.width);
            EditorGUILayout.LabelField("VoicePresetの新規作成", VoiceerEditorUtility.Bold);

            EditorGUI.indentLevel++;
            {
                _outputDirectory = EditorGUILayout.TextField("出力フォルダ:", _outputDirectory);
                _presetName = EditorGUILayout.TextField("ファイル名:", _presetName);

                EditorGUILayout.LabelField("出力先：" + Path.Combine(_outputDirectory, _presetName + _outputSuffix),
                    VoiceerEditorUtility.Bold);

                if (GUILayout.Button("新規作成"))
                {
                    var exportedPath = CreateNewPreset();
                    _loadedPreset = AssetDatabase.LoadAssetAtPath(exportedPath, typeof(VoicePreset)) as VoicePreset;
                }
            }
            EditorGUI.indentLevel--;
            VoiceerEditorUtility.Hr(position.width);

        }

        /// <summary>
        /// Presetを作成する。出力先Pathを返す
        /// </summary>
        /// <returns></returns>
        private string CreateNewPreset()
        {
           　VoiceerEditorUtility.SafeCreateDirectory(_outputDirectory);
            var createdObject = CreateInstance(nameof(VoicePreset));
            var outputPath = Path.Combine(_outputDirectory, _presetName + _outputSuffix);
            var uniqueOutputPath = AssetDatabase.GenerateUniqueAssetPath(outputPath);
            AssetDatabase.CreateAsset(createdObject, uniqueOutputPath);
            AssetDatabase.Refresh();
            return uniqueOutputPath;
        }
    }
}