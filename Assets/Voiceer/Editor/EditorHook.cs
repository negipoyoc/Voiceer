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
        private static VoicePreset _storage;
        private static VoicePreset storage
        {
            get
            {
                if (_storage == null)
                {
                    var selector = AssetDatabase.LoadAssetAtPath<StorageSelector>("Assets/Voiceer/ScriptableObject/VoicePresetSelector.asset");
                    _storage = selector.CurrentVoicePreset;
                }
                return _storage;
            }
        }


        [InitializeOnLoadMethod]
        private static void InitializeEditorHookMethods()
        {
            ///PlayModeが変わった時
            ///シーン再生を開始した時
            ///シーン再生を止めた時
            EditorApplication.playModeStateChanged += (mode) =>
            {
                //再生ボタンを押した時であること
                if (!EditorApplication.isPlayingOrWillChangePlaymode
                     && EditorApplication.isPlaying)
                    return;

                //SceneView が存在すること
                if (SceneView.sceneViews.Count == 0)
                    return;

                //Playモードに入れた時
                if (mode == PlayModeStateChange.EnteredPlayMode)
                {
                    SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnEnteredPlayMode));
                }
                //Playモードを終了した時
                if (mode == PlayModeStateChange.ExitingPlayMode)
                {
                    SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnExitingPlayMode));
                }

                //エラーがあるのにPlayしようとした。
                EditorApplication.delayCall += () => {
                    var content = typeof(EditorWindow)
                        .GetField("m_Notification", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(SceneView.sceneViews[0]) as GUIContent;

                    if (content != null && !string.IsNullOrEmpty(content.text))
                    {
                        SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnPlayButHasError));
                    }
                };
            };

            ///シーンを保存する時
            EditorSceneManager.sceneSaved += (scene) =>
            {
                SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnSave));
            };
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

                EditorApplication.delayCall += () => { 
                    SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnCompileEnd));
                };
            }
        }

        /// <summary>
        /// ビルド前・後
        /// </summary>
        public class ProcessBuildHook : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            public int callbackOrder { get { return 0; } }

            /// <summary>
            /// ビルド前
            /// </summary>
            /// <param name="report"></param>
            public void OnPreprocessBuild(BuildReport report)
            {
                SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnPreProcessBuild));
            }

            /// <summary>
            /// ビルド後
            /// </summary>
            /// <param name="report"></param>
            public void OnPostprocessBuild(BuildReport report)
            {
                if (report.summary.result == BuildResult.Succeeded)
                {
                    SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnPostProcessBuild_Success));
                }
                else
                {
                    SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnPostProcessBuild_Failed));
                }
            }
        }

        /// <summary>
        /// ビルドターゲット変更時
        /// </summary>
        public class BuildTargetChangeHook : IActiveBuildTargetChanged
        {
            public int callbackOrder { get { return 0; } }

            public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
            {
                SoundPlayer.PlaySound(storage.GetRandomClip(Trigger.OnBuildTargetChanged));
            }
        }
    }
}