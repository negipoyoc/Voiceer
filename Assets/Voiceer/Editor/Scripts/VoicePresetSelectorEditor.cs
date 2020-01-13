using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public class VoicePresetSelectorEditor : EditorWindow
    {

        private const string TitleString = "Voiceer - Voice Preset Selector";

        private static VoicePresetSelector _selector = null;

        [MenuItem("Window/Voiceer/Voice Preset Selector")]
        private static void Open()
        {
            GetWindow<VoicePresetSelectorEditor>("Voiceer");
        }

        private void OnGUI()
        {
            VoiceerEditorUtility.UiTitleBox(TitleString, VoiceerEditorUtility.BackGroundImage);

            if (_selector == null)
            { 
                _selector = VoiceerEditorUtility.GetStorageSelector();
            }
            EditorGUI.BeginChangeCheck();
            {
                _selector.CurrentVoicePreset = EditorGUILayout.ObjectField("現在のボイスPreset:",
                    _selector.CurrentVoicePreset, typeof(VoicePreset), false) as VoicePreset;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_selector);
                AssetDatabase.SaveAssets();
                // エディタを最新の状態にする
                AssetDatabase.Refresh();
            }
            VoiceerEditorUtility.Hr(position.width);
            
            if (_selector.CurrentVoicePreset == null)
            {
                EditorGUILayout.LabelField("デフォルトボイスが設定されていません。");
                return;
            }
            
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.LabelField("URL:", VoiceerEditorUtility.Bold);
                var url = _selector.CurrentVoicePreset.metaData.url;
                if (VoiceerEditorUtility.IsUrl(url))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space (EditorGUI.indentLevel * 20);
                    if (GUILayout.Button(url, VoiceerEditorUtility.LinkStyle))
                    {
                        Application.OpenURL(url);
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField(url);
                }
            }
            EditorGUI.indentLevel--;


            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.LabelField("備考:", VoiceerEditorUtility.Bold);
                EditorGUILayout.LabelField(_selector.CurrentVoicePreset.metaData.memo);
            }
            EditorGUI.indentLevel--;
        }
    }
}