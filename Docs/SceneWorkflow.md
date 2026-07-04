# Scene Workflow Guide

This document describes how to add new scenes to the project and wire them into the main menu and build.

## Adding a new scene

1. Create the scene file under `Assets/Scenes/`.
2. Add it to **File > Build Settings** so it gets a build index.
3. Ensure the scene file name matches what `SceneLoader.Load(name)` expects (no `.unity` extension).

## Wiring a scene to the main menu

1. Open `Assets/Scenes/MainMenu.unity`.
2. Select the `MainMenuDirector` GameObject.
3. Add a new button or reuse an existing one.
4. Point the button's `OnClick()` event to `MainMenuDirector` and call the appropriate method (usually `OnStartDemo()` or a new method that calls `SceneLoader.Load("YourSceneName")`).

## Localization

- Add new text keys to the `LocalizationSwitcher` on the `LocalizationManager` GameObject.
- Assign the key to each `LocalizedText` component on UI labels.
- Update both English and Arabic values.
- Keep `LocalizationManager` as a dedicated root GameObject; do not attach `LocalizationSwitcher` to `MainMenuDirector` because the switcher survives scene loads via `DontDestroyOnLoad`.

## Replacing placeholder UI

- The art/animation team can replace button sprites, fonts, colors, and layouts directly in `MainMenu.unity` without changing `MainMenuController.cs`.
- Avoid renaming GameObjects referenced by `MainMenuController` unless the Inspector references are updated.

## Testing a new scene

1. Open `MainMenu.unity`.
2. Press Play.
3. Click the button that loads the new scene.
4. Check the Console for `SceneLoader` errors if the scene is missing from Build Settings.

## Manual smoke test

Use these steps to verify the main menu in Play mode after wiring changes:

1. Open `Assets/Scenes/MainMenu.unity` and press **Play**.
2. Verify the title and five buttons are visible.
3. Click **Start Demo** and confirm `Neighborhood` loads.
4. Click **Language** and verify button texts switch to Arabic.
5. Click **Settings**, move the volume slider, and confirm no errors appear in the Console.
6. Click **Quit** and confirm the Console logs `"Quit requested"`.
