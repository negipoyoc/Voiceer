using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;

namespace Voiceer
{
    /// <summary>
    /// VoiceerEditor拡張の便利メソッド、プロパティ
    /// </summary>
    public static class VoiceerEditorUtility
    {
        /// <summary>
        /// HorizontalLine
        /// </summary>
        /// <param name="width"></param>
        public static void Hr(float width)
        {
            GUILayout.Box("", GUILayout.Width(width), GUILayout.Height(1));
        }

        /// <summary>
        /// 背景画像付きのタイトルボックス
        /// </summary>
        /// <param name="title"></param>
        /// <param name="bgIcon"></param>
        public static void UiTitleBox(string title, Texture2D bgIcon)
        {
            var bgStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                normal = {background = bgIcon}, fontSize = 22, fontStyle = FontStyle.Bold
            };

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            var scale = GUILayoutUtility.GetLastRect();
            scale.height = 30;

            GUI.Label(scale, title, bgStyle);
            GUILayout.Space(scale.height + 5);
        }


        private static GUIStyle _bold;

        /// <summary>
        /// 太字にするGUISｔｙle
        /// </summary>
        public static GUIStyle Bold => _bold != null
            ? _bold
            : _bold = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                fontStyle = FontStyle.Bold
            };


        private static GUIStyle _red;

        /// <summary>
        /// 赤文字にするGUIStyle
        /// </summary>
        public static GUIStyle Red => _red != null
            ? _red
            : _red = new GUIStyle
            {
                normal = {textColor = Color.red}
            };

        private static GUIStyle _linkStyle;

        /// <summary>
        /// リンクっぽい見た目にするGUIStyle
        /// </summary>
        public static GUIStyle LinkStyle => _linkStyle != null
            ? _linkStyle
            : _linkStyle = new GUIStyle
            {
                wordWrap = false,
                normal = {textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f)},
                stretchWidth = false
            };


        /// <summary>
        /// 指定されている文字列がURLかチェック
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsUrl(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            return Regex.IsMatch(
                line,
                @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$"
            );
        }

        private static Texture2D _backgroundImage { get; set; }

        /// <summary>
        /// 背景画像のロード
        /// </summary>
        public static Texture2D BackGroundImage => _backgroundImage
            ? _backgroundImage
            : (_backgroundImage =
                AssetDatabase.LoadAssetAtPath("Assets/Voiceer/Editor/Res/bg.png",
                    typeof(Texture2D)) as Texture2D);


        /// <summary>
        /// Triggerに合わせて正しいラベルを取得する
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static string GetDescriptionLabel(Hook trigger)
        {
            switch (trigger)
            {
                case Hook.OnCompileBegin:
                    return "コンパイルが開始した時";
                case Hook.OnCompileEnd:
                    return "コンパイルが終了した時";
                case Hook.OnEnteredPlayMode:
                    return "正常に再生できた時";
                case Hook.OnExitingPlayMode:
                    return "再生モードを終了した時";
                case Hook.OnPlayButHasError:
                    return "再生しようとしたがエラーがあって再生できなかった時";
                case Hook.OnPreProcessBuild:
                    return "ビルドプロセスに入る直前";
                case Hook.OnPostProcessBuildSuccess:
                    return "正常にビルドが完了した時";
                case Hook.OnPostProcessBuildFailed:
                    return "ビルドが失敗した時";
                case Hook.OnSave:
                    return "プロジェクトをセーブした時";
                case Hook.OnBuildTargetChanged:
                    return "ビルドターゲットを変更した時";
                default:
                    //上記で未定義なTriggerはそのまま出力
                    return trigger.ToString();
            }
        }

        /// <summary>
        /// 直接パス指定で開いてダメなら検索して開く
        /// </summary>
        /// <returns></returns>
        public static VoicePresetSelector GetStorageSelector()
        {
            var selector = AssetDatabase.LoadAssetAtPath<VoicePresetSelector>(
                "Assets/Voiceer/ScriptableObject/VoicePresetSeledctor.asset");
            if (selector == null)
            {
                var guid = AssetDatabase.FindAssets($"t:VoicePresetSelector").FirstOrDefault();
                var path = AssetDatabase.GUIDToAssetPath(guid);
                selector = AssetDatabase.LoadAssetAtPath<VoicePresetSelector>(path);
            }

            return selector;
        }

        /// <summary>
        /// 安全にフォルダを作成する
        /// </summary>
        /// <param name="path"></param>
        public static void SafeCreateDirectory(string path)
        {
            var currentPath = "";
            var splitChar = new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

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

        public static void ExportPackage(VoicePreset preset)
        {
            var assetList = new List<string>();
            assetList.Add(AssetDatabase.GetAssetPath(preset));

            foreach (var voiceClip in preset.voiceSetList.SelectMany(set => set.sound.voiceClips))
            {
                if (voiceClip != null)
                {
                    assetList.Add(AssetDatabase.GetAssetPath(voiceClip));
                }
            }

            AssetDatabase.ExportPackage(
                assetList.ToArray()
                , $"{preset.name}.unitypackage"
                , ExportPackageOptions.Interactive );
        }
    }
}