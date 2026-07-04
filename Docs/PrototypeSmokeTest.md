# Prototype Smoke Test

## Preconditions
1. Open the project in Unity `6000.5.1f1`.
2. Open the Console window (`Window > General > Console`).
3. Allow the project to compile and confirm **zero compile errors** in the Console.

## Steps
1. Click `Tools > Neighborhood > Build World (overwrites Neighborhood scene)`.
2. Open `Assets/Scenes/Neighborhood.unity`.
3. Press **Play**.
4. Basic controls — verify each one:
   - Hold **W** and confirm the player moves forward.
   - Move the mouse and confirm the camera pans horizontally and vertically.
   - Hold **Shift** while moving and confirm walk speed increases.
   - Press **Space** and confirm the player jumps.
   - Press **Esc** and confirm the cursor is released; left-click and confirm it is recaptured.
5. Phone light — press **F** once: the phone light should turn on. Press **F** again: it should turn off.
6. Walk to the abandoned house and enter the courtyard.
7. Locate the dark ball near the center drain. Stand within ~2 m and center the view on it.
8. Press **E** while looking at the ball. The ball should disappear from the scene.
9. A black capsule (woman-in-black placeholder) should appear near the doorway.
10. Either walk within ~3 m of the capsule or wait up to 6 seconds; the capsule should disappear.
11. Walk in and out of the abandoned house interior. The Console may show a harmless warning because the audio zone has no clip assigned yet; no exceptions should appear.

## Expected outcome
All steps complete without exceptions. The core loop (move → phone light → interact → encounter) is validated.
