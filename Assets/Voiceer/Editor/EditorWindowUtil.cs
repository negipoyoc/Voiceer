using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Voiceer
{
    public class VEUtil
    {
        public static void hr(float width)
        {
            GUILayout.Box("", GUILayout.Width(width), GUILayout.Height(1));
        }

        public static void UiTitleBox(string title, Texture2D bgIcon)
        {
            GUIStyle bgStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            bgStyle.normal.background = bgIcon;
            bgStyle.fontSize = 22;
            bgStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect scale = GUILayoutUtility.GetLastRect();
            scale.height = 30;

            GUI.Label(scale, title, bgStyle);
            GUILayout.Space(scale.height + 5);
        } 

        public static GUIStyle Bold
        {
            get
            {
                GUIStyle bgStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                bgStyle.fontStyle = FontStyle.Bold;
                return bgStyle;
            }
        }

        public static string GetDescription(Trigger trigger)
        {
            switch (trigger)
            {
                case Trigger.OnCompileEnd:
                    return "コンパイルが終了した時";
                case Trigger.OnEnteredPlayMode:
                    return "正常に再生できた時";
                case Trigger.OnExitingPlayMode:
                    return "再生モードを終了した時";
                case Trigger.OnPlayButHasError:
                    return "再生しようとしたがエラーがあって再生できなかった時";
                case Trigger.OnPreProcessBuild:
                    return "ビルドプロセスに入る直前";
                case Trigger.OnPostProcessBuild_Success:
                    return "正常にビルドが完了した時";
                case Trigger.OnPostProcessBuild_Failed:
                    return "ビルドが失敗した時";
                case Trigger.OnSave:
                    return "プロジェクトをセーブした時";
                case Trigger.OnBuildTargetChanged:
                    return "ビルドターゲットを変更した時";
                default:
                    return trigger.ToString();
            }
        }
    }
}