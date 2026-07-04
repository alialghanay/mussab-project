# City Expansion & Beautification — Design Spec (2026-07-04)

## Goal
Double the neighborhood, fill it with life (grass, trees, roses, props, NPCs, ambient
sound), and upgrade the graphics — using only assets already in the project plus
procedural content. Everything rebuildable and isolated from the existing scene content.

## Components

### 1. `Assets/Editor/CityExpansionBuilder.cs` — menu: Tools → Neighborhood → Build City Expansion
Builds everything under a single `_CityExpansion` root. Rebuild deletes and recreates
only that root — the original neighborhood is never touched.

Contents:
- **Outer ring:** 2–3 new streets around the existing blocks using the StarsandShells
  road tiles (materials converted to URP), ~15–18 new flat-roof houses in the existing
  Libyan greybox style (reuses `Art/Materials/Greybox` palette: Plaster*, Door*,
  CinderBlock, etc.).
- **Park/square:** in the expansion area — low wall, WoodenParkBench prefabs, dense
  grass, several trees, rose beds.
- **Vegetation:** grass tufts = crossed quads with `Texture_Grass_Diffuse` (URP Lit
  cutout, double-sided), scattered along road edges/house walls, dense in park.
  Trees = cylinder trunk + 2–3 crown spheres (Crown/DeadCrown materials). Rose bush =
  small green sphere-bush + red/pink dots.
- **Props:** parked cars, crates, trash bins, AC window units, satellite dishes —
  primitives with existing greybox materials, matching the current world.
- **NPC waypoints:** `_CityExpansion/NpcWaypoints` — empty transforms on sidewalks
  across BOTH old and new streets.
- **Dark figure NPC prefab** saved to `Assets/Art/Prefabs/NpcFigure.prefab`
  (~1.7 m, primitives, dark muted clothing colors).

### 2. Runtime scripts (`Assets/Scripts/`)
- **`NpcWanderer.cs`** — walks between waypoints (simple transform movement + turn
  toward target, slight bob), random idle pauses. No navmesh needed.
- **`NpcManager.cs`** — holds waypoint root + NPC prefab; keeps 8–12 NPCs wandering
  between 07:00–19:00 (`DayNightCycle.Hour`), fades them out after dark. Streets are
  EMPTY at night.
- **`AmbienceController.cs`** — procedurally generates looping AudioClips at Start
  (no audio assets exist in the project): wind (filtered noise, constant, stronger at
  night), crickets (chirp pattern, night only), sparse bird chirps (day only).
  Crossfades layers using `DayNightCycle.Hour`. 2D sound (spatialBlend 0).

### 3. Graphics upgrade
- Global post-processing Volume: bloom (lamp glow), vignette (~0.25), slight color
  grading (contrast/saturation), film grain (subtle).
- `DayNightCycle.cs` gains fog control: `RenderSettings.fog` (exponential), color and
  density lerped by hour — pale/thin at day, dark/thick at night.
- Longer URP shadow distance; fix any pink (Built-In) materials used by the road pack.

## Verification
Compile clean; screenshots at 10:00 (living city: NPCs, grass, park) and 22:00
(empty foggy streets, glowing lamps); brief play-mode check for NPC movement and audio.

## Out of scope
MarpaStudio interior props (later phase: furnishing interiors), story/scripted events,
character animation rigs.
