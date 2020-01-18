#if UNITY_2019_2_OR_NEWER
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Voiceer
{
    /// <summary>
    /// ユニットテストの実行・完了イベントをフックして音声を再生します
    /// Required: Unity 2019.2 + Test Framework 1.1 or newer
    /// see: https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-icallbacks.html
    /// </summary>
    public class TestHook
    {
        [InitializeOnLoadMethod]
        private static void SetupListeners()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new TestCallbacks());
        }

        private class TestCallbacks : IErrorCallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                SoundPlayer.PlaySound(Hook.OnTestRunStarted);
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                if (result.FailCount == 0)
                {
                    SoundPlayer.PlaySound(Hook.OnTestRunSuccessfully);
                }
                else
                {
                    SoundPlayer.PlaySound(Hook.OnTestRunFailed);
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
                // テストノード単位の開始契機なので音声再生はしない
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                // テストノード単位の終了契機なので音声再生はしない
            }

            public void OnError(string message)
            {
                SoundPlayer.PlaySound(Hook.OnTestError);
            }
        }
    }
}
#endif