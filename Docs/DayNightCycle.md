# Day/Night Cycle — Design Spec (2026-07-04)

## Goal
Real-time day/night cycle: **10 real minutes = 24 game hours**, starting at **07:00**.
Sky blends smoothly through the Fantasy Skybox FREE FS002 panoramic set; lighting and
street lamps follow the time of day automatically.

## Components

### 1. `Assets/Art/Shaders/SkyboxBlend.shader` — `Skybox/PanoramicBlend`
Panoramic skybox shader with two equirectangular textures (`_TexA`, `_TexB`) and a
`_Blend` slider (0 = A, 1 = B), plus `_Tint`, `_Exposure`, `_Rotation`.
Used by `Assets/Art/SkyboxBlend_DayNight.mat`, which replaces the scene skybox.

### 2. `Assets/Scripts/DayNightCycle.cs` — on new root object `_DayNightCycle`
`[ExecuteAlways]` MonoBehaviour.

**Time:** `timeOfDay` (0–24 float, Inspector slider — scrub to preview),
`dayLengthMinutes` (default 10). Advances only in Play mode, wraps at 24.
Public API for GameDirector: `Hour`, `IsNight`.

**Sky schedule (blend windows use smoothstep):**

| Hours | Sky |
|-------|-----|
| 20:00–04:00 | Night |
| 04:00–06:00 | Night → Sunrise |
| 06:00–08:00 | Sunrise → Day |
| 08:00–16:00 | Day |
| 16:00–18:00 | Day → Sunset |
| 18:00–20:00 | Sunset → Night |

Textures come from the FS002 materials (Sunrise, Day, Sunset, Night).

**Sun/moon:** reuses the existing `Moon_Directional` light. X rotation =
`timeOfDay/24 × 360 − 90` (horizon at 6:00/18:00, overhead at noon). When the sun is
below the horizon the light flips to the moon position (opposite side) with the
current dim blue moonlight (color `(0.66, 0.73, 0.83)`, intensity ≤ 0.35). Daytime is
warm white up to ~1.1 intensity, orange near sunrise/sunset. Ambient intensity lerps
0.35 (night) → 1.0 (noon); `DynamicGI.UpdateEnvironment()` throttled to every 5 s.

**Street lights:** all `Light`s under `_StreetLights` fade on 18:00→18:30 and off
06:00→06:30 (uniform `lampIntensity`, lights disabled when fully off).

## Out of scope
No gameplay reactions to time (GameDirector only gets the read-only API), no lightmap
baking changes, no weather.
