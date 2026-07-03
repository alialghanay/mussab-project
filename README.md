# 4th Day of Eid

A Fear-to-Fathom-style first-person psychological horror walking simulator set in a small Libyan residential neighborhood.

Developed by:
- Ali M. Alghanay (MR. BKST)
- Mohammed Albaroin (I.LXRD)
- Ahmed Almahjoub (SHANABO)

Under **Mussab Projects**.

## Engine

Unity `6000.5.1f1` (Unity 6).

## How to open the project

1. Install Unity `6000.5.1f1` with the Universal Render Pipeline module.
2. Open the project folder (`/home/rat/repo/mussab-project`) in Unity Hub.
3. Open the project.

## How to generate the world

1. In the Unity Editor, open **Tools > Neighborhood > Build World**.
2. This creates/regenerates `Assets/Scenes/Neighborhood.unity` from the greybox primitives, lights, and player rig defined in `Assets/Editor/NeighborhoodWorldBuilder.cs`.

## How to test

1. Open `Assets/Scenes/Neighborhood.unity`.
2. Press **Play** in the editor.
3. Use WASD to move, mouse to look, `Shift` to sprint, `Space` to jump, and `F` to toggle the flashlight.

## Shipping a build

`Neighborhood.unity` is not currently in the editor build list. Add it via **File > Build Settings** before making a player build.
