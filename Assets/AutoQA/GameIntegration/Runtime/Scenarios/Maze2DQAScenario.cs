using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoQA.Maze2D
{
    public static class Maze2DQAScenario
    {
        public static IEnumerator Run(Action<bool, string> completed)
        {
            SceneManager.LoadScene(GameCore.GameConstants.MainMenuScene);
            yield return null;
            var bootstrap = Maze2DQABootstrap.Instance;
            if (!bootstrap) bootstrap = new GameObject("AutoQA Test Session").AddComponent<Maze2DQABootstrap>();
            var runtime = bootstrap.Runtime;
            runtime.BeginSession();
            foreach (var key in new[] { "menu.play", "lobby.width", "lobby.height", "lobby.exits", "lobby.play" })
            {
                Observation o = null;
                var deadline = Time.realtimeSinceStartup + 15;
                do { o = runtime.CaptureObservation(); if (o.readiness.status == "ready") break; yield return null; }
                while (Time.realtimeSinceStartup < deadline);
                if (o.readiness.status != "ready") { completed(false, "Readiness timeout: " + o.readiness.reason); yield break; }
                var action = o.availableActions.SingleOrDefault(x => x.semanticKey == key);
                if (action == null) { completed(false, "Missing action: " + key); yield break; }
                var result = runtime.Execute(new ActionCommand { commandId = Guid.NewGuid().ToString("N"), sessionId = o.sessionId,
                    observationId = o.observationId, revision = o.revision, actionId = action.actionId });
                if (result.status != "completed") { completed(false, result.code + ": " + result.description); yield break; }
                yield return null;
            }
            var end = Time.realtimeSinceStartup + 15;
            while (Time.realtimeSinceStartup < end)
            {
                var o = runtime.CaptureObservation();
                if (o.readiness.status == "ready" && o.checks.Any(x => x.key == "configuredGameplay" && x.status == "passed"))
                { completed(true, "Scenario passed without QAContext descriptors"); yield break; }
                yield return null;
            }
            completed(false, "Gameplay oracle timeout");
        }
    }
}
