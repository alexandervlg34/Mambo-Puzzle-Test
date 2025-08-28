# This is a match-3 game created for Voodoo as a Unity Developer's Test Assessment.

I recommend studying the code from the "EntryPoint" script and moving on through function calls.

Recommened game resolution: 1920x1080

Unity editor version used: 2022.3.19f1

Architectural solutions used:
1. GameEntryPoint Pattern for convenient control over game initialization.
2. Separation of data and visual parts of match3 for flexible testing.
3. "Game mode" abstraction. Allows you to flexibly create different game modes. The game mode controls the logic of the mode, controls score, and has its own presentation. At the moment, only the "time mode" has been created, but in the future it may depend on the config.
4. Loading the config (stored in the StreamingAssets folder)

You can test the correctness of match-3 logic using EditMode tests. Open TestRunner (Window->General->TestRunner) and run the written tests.

You can test the match-3 visuals by pressing the "X" button during the game. This will start a cycle of moving random cells in a random direction for up to 10000 matches. Press "X" if you want to finish.

Packages used:
1. 2D package
2. TextMeshPro
3. Test Framework
4. FruitFace - animated UI Pack (https://assetstore.unity.com/packages/2d/gui/icons/fruitface-animated-ui-pack-58686)
5. 2D Casual UI HD (https://assetstore.unity.com/packages/2d/gui/icons/2d-casual-ui-hd-82080)

Please enjoy!