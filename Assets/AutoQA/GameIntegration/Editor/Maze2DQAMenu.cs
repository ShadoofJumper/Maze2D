#if UNITY_EDITOR
using AutoQA.Maze2D;
using UnityEditor;
using UnityEngine;

namespace AutoQA.Maze2D.Editor
{
    public static class Maze2DQAMenu
    {
        [MenuItem("Tools/AutoQA/Run Maze2D Scenario (Play Mode)")]
        static void Run()
        {
            if (!Application.isPlaying) { Debug.LogError("Enter Play Mode in MainMenuScene first."); return; }
            var bootstrap = Maze2DQABootstrap.Instance;
            if (!bootstrap) bootstrap = new GameObject("AutoQA Test Session").AddComponent<Maze2DQABootstrap>();
            bootstrap.StartCoroutine(Maze2DQAScenario.Run((passed, message) => {
                if (passed) Debug.Log("AutoQA: " + message); else Debug.LogError("AutoQA: " + message);
            }));
        }
    }
}
#endif
