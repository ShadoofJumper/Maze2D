# Maze2D

The main menu Play button opens the lobby. The lobby configures maze width,
height and number of exits. Its Play button starts gameplay with those settings;
Back returns to the menu. Width must be 5–20, height 5–15 and exits 1–5.
In gameplay the player moves through the maze and wins by reaching an exit.
The completion popup shows steps and elapsed time. Closing it restarts the level.
The Exit button quits the application.

The initial QA scenario starts a maze with width 8, height 7 and 2 exits.
Its goal is to verify initialized gameplay with those parameters, not to solve
the maze. The scenario goal should be passed separately by the future Host.
