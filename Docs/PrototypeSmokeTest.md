# Prototype Smoke Test

## Steps
1. Open the project in Unity 6000.5.1f1.
2. Click `Tools > Neighborhood > Build World (overwrites Neighborhood scene)`.
3. Open `Assets/Scenes/Neighborhood.unity`.
4. Press Play.
5. Verify WASD movement, mouse look, Shift sprint, Space jump, Esc cursor release, left-click recapture.
6. Press `F` and verify the phone light toggles.
7. Walk to the abandoned house and enter the courtyard.
8. Find the ball, look at it, and press `E`. The ball should disappear.
9. A black capsule should appear near the doorway (the woman-in-black placeholder).
10. Walk toward or wait 6 seconds; the capsule should disappear.
11. Walk in/out of the house interior audio zone; no errors should occur (no clip assigned yet).

## Expected outcome
All steps complete without exceptions. The core loop is validated.
