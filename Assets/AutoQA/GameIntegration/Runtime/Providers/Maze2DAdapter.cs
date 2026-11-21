using System.Collections.Generic;
using System.Linq;
using AutoQA;
using GameCore;
using GameCore.Services;
using GameCore.Services.GameMechanicsService;
using Plugins.Core.UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using GameCore.Entity.Player;

namespace AutoQA.Maze2D
{
    public sealed class Maze2DAdapter : IGameContextProvider, IReadinessProvider, ITestOracle, ITestDataProvider, ISemanticProvider
    {
        public const int FixtureWidth = 8, FixtureHeight = 7, FixtureExits = 2;
        static T Find<T>() where T : UnityEngine.Object => UnityEngine.Object.FindFirstObjectByType<T>();
        static GameSetupService Setup => ProjectContext.HasInstance ? ProjectContext.Instance.Container.TryResolve<GameSetupService>() : null;
        static GameMechanicsService Mechanics
        {
            get
            {
                var scene = Find<SceneContext>();
                return scene ? scene.Container.TryResolve<GameMechanicsService>() : null;
            }
        }
        public void CollectContext(Observation observation)
        {
            observation.screen = Find<LobbyScreen>() ? "Lobby" : Find<MainMenuScreen>() ? "MainMenu" : "Gameplay";
            var setup = Setup;
            if (setup == null) return;
            var config = setup.MazeConfig;
            observation.gameContext.Add(new Fact("width", config.Width.ToString()));
            observation.gameContext.Add(new Fact("height", config.Height.ToString()));
            observation.gameContext.Add(new Fact("exits", config.NumberOfExits.ToString()));
        }
        public Readiness GetReadiness()
        {
            if (Setup == null || Setup.MazeConfig.Width <= 0) return Busy("Application initializing");
            // Game-specific heuristic for ScaleWindowAnimator, not a universal readiness signal.
            if (UnityEngine.Object.FindObjectsByType<UIElement>(FindObjectsSortMode.None)
                .Any(x => (x.transform.localScale - Vector3.one).sqrMagnitude > 0.000001f)) return Busy("UI scale transition (heuristic)");
            var scene = SceneManager.GetActiveScene().name;
            if (scene == GameConstants.GamePlayScene)
                return GameplayInitialized() ? Ready() : Busy("Gameplay initializing");
            if (scene == GameConstants.MainMenuScene && (Find<MainMenuScreen>() || Find<LobbyScreen>())) return Ready();
            return new Readiness { status = "unavailable", reason = "Unsupported scene or missing screen" };
        }
        static Readiness Ready() => new Readiness { status = "ready", reason = "Initialization complete" };
        static Readiness Busy(string reason) => new Readiness { status = "busy", reason = reason };
        static bool GameplayInitialized()
        {
            var mechanics = Mechanics;
            return mechanics != null && GameFieldReader.Read<PlayerEntityController>(mechanics, "_player") != null
                && Find<PlayerEntityView>() && Camera.main;
        }
        public void Evaluate(Observation observation)
        {
            var config = Setup?.MazeConfig;
            var success = observation.scene == GameConstants.GamePlayScene && GameplayInitialized() && config != null
                && config.Value.Width == FixtureWidth && config.Value.Height == FixtureHeight && config.Value.NumberOfExits == FixtureExits;
            observation.checks.Add(new CheckResult { key = "configuredGameplay", status = success ? "passed" : "pending",
                description = "Gameplay initialized with width=8, height=7, exits=2" });
        }
        public string GetKey(Component target)
        {
            var menu = target.GetComponentInParent<MainMenuScreen>();
            if (menu) return target == GameFieldReader.Read<Component>(menu, "_playButton") ? "menu.play" : target == GameFieldReader.Read<Component>(menu, "_exitGame") ? "menu.exit" : null;
            var lobby = target.GetComponentInParent<LobbyScreen>();
            if (!lobby) return null;
            if (target == GameFieldReader.Read<Component>(lobby, "_playButton")) return "lobby.play";
            if (target == GameFieldReader.Read<Component>(lobby, "_exitButton")) return "lobby.back";
            if (target == GameFieldReader.Read<Component>(lobby, "_widthInput")) return "lobby.width";
            if (target == GameFieldReader.Read<Component>(lobby, "_heightInput")) return "lobby.height";
            if (target == GameFieldReader.Read<Component>(lobby, "_exitNumberInput")) return "lobby.exits";
            return null;
        }
        public bool IsAllowed(Component target) => GetKey(target) != "menu.exit";
        public IEnumerable<string> GetValues(Component target)
        {
            switch (GetKey(target))
            {
                case "lobby.width": return new[] { FixtureWidth.ToString() };
                case "lobby.height": return new[] { FixtureHeight.ToString() };
                case "lobby.exits": return new[] { FixtureExits.ToString() };
                default: return System.Array.Empty<string>();
            }
        }
    }
}
