#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace AutoQA.Maze2D.Editor
{
    [InitializeOnLoad]
    public sealed class AutoQATestRunner : ICallbacks
    {
        const string ActiveKey = "AutoQA.TestRunMode";
        static readonly TestRunnerApi Api;
        static AutoQATestRunner()
        {
            Api = ScriptableObject.CreateInstance<TestRunnerApi>();
            Api.RegisterCallbacks(new AutoQATestRunner());
        }
        [MenuItem("Tools/AutoQA/Tests/Run EditMode")]
        static void EditTests() => Run(TestMode.EditMode);
        [MenuItem("Tools/AutoQA/Tests/Run PlayMode")]
        static void PlayTests() => Run(TestMode.PlayMode);
        static void Run(TestMode mode)
        {
            SessionState.SetString(ActiveKey, mode.ToString());
            Api.Execute(new ExecutionSettings(new Filter { testMode = mode, groupNames = new[] { "^AutoQA\\." } }));
        }
        public void RunFinished(ITestResultAdaptor result)
        {
            var mode = SessionState.GetString(ActiveKey, "");
            if (string.IsNullOrEmpty(mode)) return;
            SessionState.EraseString(ActiveKey);
            Directory.CreateDirectory("Temp");
            TestRunnerApi.SaveResultToFile(result, "Temp/AutoQA-" + mode + "-Editor.xml");
        }
        public void RunStarted(ITestAdaptor testsToRun) { }
        public void TestStarted(ITestAdaptor test) { }
        public void TestFinished(ITestResultAdaptor result) { }
    }
}
#endif
