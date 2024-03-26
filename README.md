# Rewritten
A rewrite of the entire Bloons TD 5 game using the Godot game engine

# Goal
The main priority on this repository is to create a mostly faithful re-creation of Bloons TD 5

## Roadmap
This section will serve as a list of things that need to be done before the project is completed. If anything is missing, please add it in a pull request!

- ✅️ Completed
- ☑️ Completed, but may require more work
- 🔄 In progress
- ❌️ Incomplete

```
🔄 Asset Loading
    ✅️ Jet file importing
    ✅️ Sprite Sheet parsing
    ✅️ Texture Loading
        ✅️ JPNG file support
        ✅️ Track Thumbnail loading
    ✅️ CompoundSprite loading
    🔄 Factory initialization
        ☑️ Base Factory implementation
        🔄 Tower Factory
            ☑️ Tower Definition loading
            ☑️ Tower Instantiation
        ❌️ Bloon Factory
        ❌️ Other factories
🔄 Gameplay
    🔄 Screens and Menus
        ✅️ BloonsBaseScreen
        ✅️ DGSplashScreen
        🔄 MainMenuScreen
        🔄 PopupScreenBase
        🔄 GameSelectScreen
        ❌️ SettingsScreen
        🔄 MapSelectionScreen
            🔄 Map list
            ✅️ Left/Right buttons
            ❌️ Side panel
            ❌️ Mastery Icon
        🔄 GameHudScreen
            ✅️ Background textures
            ❌️ Special agent select
            ❌️ Game mode info
        🔄 InGameTowerSelectScreen
            ❌️ Cash display
            ❌️ Lives display
            🔄 Tower selection
                ✅️ Tower icons
                ☑️ Scroll background
                🔄 Scroll up/down buttons
                ❌️ Utility icons (pineapple/spikes)
        🔄 InGamePauseScreen
            🔄 Buttons
                ✅️ Home
                ❌️ Resume
                ❌️ Every other button
            ❌️ Menu title
            ❌️ Placement options
```


# FAQ
Below are some questions I think most people would have about this

## How will it work for a user?
In the ideal situation, the resulting exe from Godot will serve as a drop-in alternative to `BTD5-Win.exe` or `BTD5-Kong.exe`.

## Is there mod support?
This repository will not have any more modding support than the base game. Once this project is in a complete enough state, a fork of this repository will be made for modding purposes.

## Will this have bug fixes or performance improvements?
At the moment, most likely not. I don't think it's feasible to make a faithful re-creation of the game and include fixing bugs or issues with the base game without a team backing this project.

# Build/Setup
To build BTD5-Rewritten,
1. Download [Godot](https://godotengine.org/). **Make sure to download the version with .NET/Mono/C# support!**
2. Clone this project using git
3. Open Godot and Import the project
4. Once Godot is open, locate the "FileSystem" window
5. Find the `asset_importer_config.gd` file, it is insde the `Godot/Scripts/` folder.
6. Find the `_ready()` function, and edit the string that sets the `game_dir` variable
7. Replace the string with the path to your own installation of BTD5 ***NOTE: Replace any `\` with a `/`!!***
8. You can now run the game directly from the Godot editor

