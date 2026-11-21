# Maze2D AutoQA integration

Runtime package: Packages/com.maze2d.autoqa. Game-specific providers and fixtures
are under GameIntegration/Runtime. No QAContext descriptors are required or added
to the game scenes. The shared game description is Config/GAME_DESCRIPTION.md;
loading it into a model is deferred to the future Host.

## Run the first scenario

1. Let Unity import/compile the package.
2. Open Assets/Scenes/MainMenuScene.unity and enter Play Mode.
3. Choose Tools > AutoQA > Run Maze2D Scenario (Play Mode).
4. The harness reloads MainMenuScene, creates an explicitly opted-in persistent
   bootstrap, opens Lobby, sets width=8/height=7/exits=2 and starts gameplay.
5. Console reports success only after the game initialization signal and the
   configured-parameter oracle pass. Every wait has a 15-second deadline.

Alternatively run AutoQA.Maze2D.Tests in the Test Runner PlayMode tab, or use
Tools > AutoQA > Tests > Run PlayMode to run all AutoQA PlayMode tests. This test
changes the active scene. The bootstrap is not automatically enabled in normal
play or production. An explicitly created instance exposes Runtime for manual
capture/execution and survives scene changes until destroyed or Play Mode ends.
Do not launch multiple scenario runs concurrently.

Game integration reads existing private UI references through a cached,
type-checked GameFieldReader; no QA public accessors are needed in game views.
Missing or renamed fields fail explicitly through Observer diagnostics. The
adapter uses a game-specific scale-animation heuristic for UI readiness, and
checks the existing player controller, player view and camera for gameplay
startup. These checks are not a universal initialization guarantee.
link.xml preserves the reflected fields for Player stripping; an actual IL2CPP
build has not been verified. It obtains game services from their
existing Zenject containers without introducing Zenject into the package.
The current oracle verifies gameplay startup, not solving the maze.

## Verification in this implementation session

Compiled Core, UGUI, the modified UI assembly, the complete game runtime sources,
and package test assemblies with Unity's bundled Roslyn compiler against this
project's Unity/reference assemblies. No compilation errors; existing game
warnings remain. Runtime sources were compiled without UNITY_EDITOR.

Unity batch mode refused to open the project because it is already open in the
Editor. Tests were subsequently launched inside that Editor through its Test
Runner API. The initial seven EditMode tests passed, and the real Maze2D
end-to-end PlayMode scenario passed without descriptors. The separate UGUI test
initially failed because it also matched controls from the loaded game scene;
its assertions now explicitly select the fixture targets. That correction and
two additional EditMode cases compile, but their final Editor rerun remains
pending. The recorded initial results are in Temp/AutoQA-EditMode-Editor.xml
and Temp/AutoQA-PlayMode-Editor.xml. A full Player build has not been run.

Use Tools > AutoQA > Tests to rerun the final tests. The helper writes XML to
the paths above and does not start tests automatically.
