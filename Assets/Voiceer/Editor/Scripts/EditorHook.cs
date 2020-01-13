using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace Voiceer
{
    public class EditorHook
    {
        [InitializeOnLoadMethod]
        private static void InitializeEditorHookMethods()
        {
            //PlayModeが変わった時
            //シーン再生を開始した時
            //シーン再生を止めた時
            EditorApplication.playModeStateChanged += (mode) =>
            {
                //再生ボタンを押した時であること
                if (!EditorApplication.isPlayingOrWillChangePlaymode
                    && EditorApplication.isPlaying)
                    return;

                //SceneView が存在すること
                if (SceneView.sceneViews.Count == 0)
                    return;

                switch (mode)
                {
                    //Playモードに入れた時
                    case PlayModeStateChange.EnteredPlayMode:
                        SoundPlayer.PlaySound(Hook.OnEnteredPlayMode);
                        break;
                    //Playモードを終了した時
                    case PlayModeStateChange.EnteredEditMode:
                        SoundPlayer.PlaySound(Hook.OnExitingPlayMode);
                        break;
                }

                //エラーがあるのにPlayしようとした。
                EditorApplication.delayCall += () =>
                {
                    var content = typeof(EditorWindow)
                        .GetField("m_Notification", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(SceneView.sceneViews[0]) as GUIContent;

                    if (content != null && !string.IsNullOrEmpty(content.text))
                    {
                        SoundPlayer.PlaySound(Hook.OnPlayButHasError);
                    }
                };
            };

            ///シーンを保存する時
            EditorSceneManager.sceneSaved += (scene) => { SoundPlayer.PlaySound(Hook.OnSave); };
        }

        /// <summary>
        /// コンパイル終了時
        /// </summary>
        [InitializeOnLoad]
        public class CompileFinishHook
        {
            static CompileFinishHook()
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorApplication.delayCall += () => { SoundPlayer.PlaySound(Hook.OnCompileEnd); };
            }
        }

        /// <summary>
        /// ビルド前・後
        /// </summary>
        public class ProcessBuildHook : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            public int callbackOrder
            {
                get { return 0; }
            }

            /// <summary>
            /// ビルド前
            /// </summary>
            /// <param name="report"></param>
            public void OnPreprocessBuild(BuildReport report)
            {
                SoundPlayer.PlaySound(Hook.OnPreProcessBuild);
            }

            /// <summary>
            /// ビルド後
            /// </summary>
            /// <param name="report"></param>
            public void OnPostprocessBuild(BuildReport report)
            {
                if (report.summary.result == BuildResult.Failed || report.summary.result == BuildResult.Failed)
                {
                    SoundPlayer.PlaySound(Hook.OnPostProcessBuildFailed);
                }
                else
                {
                    SoundPlayer.PlaySound(Hook.OnPostProcessBuildSuccess);
                }
            }
        }

        /// <summary>
        /// ビルドターゲット変更時
        /// </summary>
        public class BuildTargetChangeHook : IActiveBuildTargetChanged
        {
            public int callbackOrder => int.MaxValue;

            public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
            {
                SoundPlayer.PlaySound(Hook.OnBuildTargetChanged);
            }
        }
    }
}