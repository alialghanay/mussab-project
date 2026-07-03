# World Design Document — "The Neighborhood" (working title)

First-person psychological horror. Environment-only design.
Setting: an old residential neighborhood (حي شعبي) on the edge of a Libyan city.
Reference tone: Fear to Fathom — ordinary places, low budget realism, dread from familiarity.

This document contains no story, characters, dialogue, mechanics, or code. It describes only the physical world so a level designer / 3D artist can build it in Unity (URP) or Blender.

---

## 1. World Overview

### 1.1 The core idea

The whole world is one small neighborhood you could walk across in four minutes. Nothing about it is fantastical. Every wall, wire, and doorway is something a Libyan player has seen a thousand times. The horror comes from three things only:

1. **Emptiness where there should be people.** These neighborhoods are loud and social. At night, in this game, they are silent. The player feels the absence of life more than the presence of anything.
2. **Light as territory.** Warm yellow pools around inhabited doors = safe. The gaps between streetlights = not safe. The abandoned house sits entirely outside the light network.
3. **Sightlines that almost show you things.** Alley mouths, rooftop edges, half-open doors, the dark gap behind a parked car. The world is built so the player is always looking *into* darkness from light, or *back* toward light from darkness.

### 1.2 Design pillars

- **Believable first, scary second.** Every prop must answer "would this actually be here?" If not, cut it.
- **Small and dense.** One connected map, no loading between exterior areas. Interior of the shop and abandoned house can be separate scenes if needed.
- **Wear, not ruin.** The neighborhood is old and dusty, not destroyed. Only the abandoned house is allowed real decay.
- **Vertical presence.** Rooftops are visible from almost everywhere. Water tanks and satellite dishes on the skyline make silhouettes the player's eye keeps checking.
- **Nothing glows.** No magical light sources. Every light has a fixture: streetlamp, doorway bulb, shop fluorescent, window light behind curtains, moon.

### 1.3 Scale reference (for blockout)

- Alley width: 3–4.5 m (one car barely passes)
- Main road width: 7–8 m including dirt shoulders
- House facade height: 3.5 m (one floor) / 6.5–7 m (unfinished two floors)
- Courtyard walls: 2.2–2.6 m, topped occasionally with broken glass or short rebar
- Streetlight pole height: 6–7 m, spacing 35–50 m (deliberately too far apart)
- Player eye height: 1.7 m

---

## 2. Map Layout

### 2.1 Connection diagram

```
                          [open dirt field]
                                 |
                        +----------------+
                        |  5. SCHOOL     |
                        |  (walled yard) |
                        +----------------+
                                 |
        ... city glow ...   MAIN ROAD (east–west)
   ------------------+======================+------------------
                     |          |           |
              [4. SHOP]         |     [side street, blocked
               (corner)         |      by parked cars/rubble]
                                |
                         SIDE STREET (north–south, ~60 m)
                                |
                     +----------+----------+
                     |   1. MAIN ALLEY     |
                     |   (player start)    |
                     |                     |
              [2. FAMILY HOUSE]            |
               (mid-alley, west side)      |
                     |                     |
                     +----- alley bends ---+
                                |
                        narrower dead-end branch (~2.5 m wide)
                                |
                     [6/7. ABANDONED HOUSE]
                      (alley terminates at its wall)
                      interior → 8. stairs → 9. rooftop → 10. locked room
```

### 2.2 Walking distances (night walk pacing)

- Family house door → alley mouth: ~40 m
- Alley mouth → shop along main road: ~120 m (the "bread walk" — long enough for tension, passes the school across the road)
- Family house → abandoned house: ~55 m, but through the dead-end branch that gets darker with every step
- School gate: visible across the main road for most of the shop walk; never on the direct path — the player looks at it, doesn't have to enter the grounds until the design calls for it

### 2.3 Layout rules

- The abandoned house is the **only** dead end in the map. Everywhere else has at least two exits. Approaching it must feel like the world funnels closed behind you.
- The family house and the abandoned house share the same alley system: the safe place and the wrong place are neighbors. From the family house roofline, the abandoned house's water tank is just visible.
- The shop is the brightest point on the map at night. The school is the largest dark mass. They face each other across the main road.
- Beyond the playable edge: silhouettes of more rooftops, a distant minaret with a green-lit crown, a faint orange city glow on the horizon, one far radio tower with a blinking red light. Block off streets naturally: parked cars, a rubble pile from someone's renovation, a chain-link gate, a dark unlit street that simply reads as "not this way."

---

## 3. Location-by-Location Breakdown

Each location: description → prop list → lighting → sound → what the player sees.

---

### Location 1 — The Main Alley (player start)

**Description.**
A residential alley 3.5–4 m wide, ~70 m long with one gentle bend so you can never see the whole thing at once. Ground: old asphalt worn to dirt in patches, a shallow concrete drainage channel along one edge, dust and fine sand collected against the walls. Facades are a rhythm of: painted cement render (cream, faded ochre, pale green — each house a slightly different tone, all sun-bleached), rusted metal double doors, small barred windows at 1.6–2 m height, and electric meter boxes bolted next to each door with wires stapled up the wall. Overhead, bundles of sagging cables cross the alley between houses; one dead cable hangs loose and stirs in the wind. Two or three houses have unfinished second floors: bare grey cinder block, no render, rebar sticking up from the corner columns like reeds.

**Props.**
- Metal house doors (double-leaf, ridged/paneled sheet steel, painted brown/green/blue, rust bleeding at the bottom edge and hinges)
- Electric meter boxes (grey/beige, some with cracked glass windows, hand-written numbers in marker on the wall beside them)
- Water tap stubs low on walls, hose coiled at one house
- Parked cars: an old Toyota-style pickup, a dusty 90s sedan (one with a sun-faded cover), parked half on the "sidewalk"
- Gas bottle (blue butane cylinder) beside one door
- Trash corner: black bags, a flattened cardboard box, an empty tomato-paste tin, drifted plastic bags caught on a wall spike
- Satellite dish mounted low on one wall bracket, cable stapled around a window
- Children's chalk marks low on one wall (faded hopscotch grid, scribbles); a deflated football wedged in the drainage channel
- A/C units on wall brackets, dripping stains on the render below them
- One doorstep with worn slippers left outside — the strongest "people live here" signal
- Broom leaned by a door; a low wooden stool
- Wall dish of dry cat food / tin of water near one door

**Lighting.**
- *Day:* hard white sunlight, deep clean-edged shadows across the alley, dust haze in the air. Everything looks poor but harmless.
- *Night:* one working streetlight at the alley mouth (warm sodium yellow, slight flicker never more than a subtle shimmer) and one at the bend. Between them, ~25 m of near-darkness broken only by two doorway bulbs (bare warm bulbs above doors, one behind a cracked plastic shade). Window light behind curtains in 2–3 houses, warm and dim. The dead-end branch toward the abandoned house has **no** light at all.

**Sound.**
- Day: distant traffic, a far generator, sparrows, a radio playing faintly behind a wall, a hand slapping dough or a spoon on a pot somewhere.
- Night: wind moving grit along the ground, the loose cable tapping the wall irregularly, a TV murmur behind one lit window, cat movement in the trash corner, the electric meter's faint tick near doors. Far away: one dog barking, answered by a farther one.

**Player visibility.**
From the start point the player sees: warm light at their own door behind them, the lit alley mouth ahead, the dark bend, and — only if they look — the unlit dead-end branch. The abandoned house is *not* visible from the start; only the black gap that leads to it.

---

### Location 2 — The Family House (exterior)

**Description.**
Mid-alley, west side. A single-story house with a low parapet roof, render painted a warm cream that's cleaner than the neighbors' — someone maintains this house. Metal front door painted dark green, repainted enough times that the ridges are soft; a small aluminum knocker; a doorbell button with a hand-drawn label. Above the door, a small entrance light in a simple frosted shade — the warmest, steadiest light in the whole map. Two windows on the facade with decorative iron bars (simple arc pattern), curtains behind glass, one window lit from inside at night. A short step up to the threshold, tiled with two rows of patterned cement tiles different from the street. Along the base of the wall: potted plants in cut-open paint buckets and old cooking-oil tins (basil, geranium — slightly dusty but alive). Rooftop line shows a black water tank, a satellite dish, and a laundry line with a few clothespins.

**Props.**
- Green metal door with knocker, keyhole plate, doorbell
- Frosted entrance lamp (warm 2700K)
- Barred windows with clean curtains; one A/C unit above a window, quietly dripping onto a dark patch
- Plant pots in repurposed tins/buckets, small watering can
- Doormat (worn, geometric pattern), slippers beside it
- Water tank + tank stand on roof edge, satellite dish, TV antenna
- Garden hose on a wall hook, coiled properly (this house is cared for)
- House number tile / hand-painted number beside the door
- Electric meter box, newer and intact compared to neighbors

**Lighting.**
- *Day:* the brightest, cleanest facade in the alley.
- *Night:* entrance lamp always on; one curtained window glowing warm. This is the visual anchor of "safe" — when the player turns around anywhere in the alley, this light should be findable.

**Sound.**
Close to the door: muffled interior life (TV, faint kitchen sounds, water in a pipe). The A/C's soft drone and drip. These sounds fade within a few meters — leaving the house means leaving its sound.

**Player visibility.**
The door and lamp are visible from most of the main alley. From the doorstep, the player can see the alley bend but not past it, and cannot see the abandoned house — home feels insulated from it by geometry.

---

### Location 3 — The Road to the Shop

**Description.**
Leaving the alley mouth, the player turns onto the main road: 7–8 m of patched asphalt with dirt shoulders, a broken curb line, houses set back behind low courtyard walls on both sides. The road runs straight ~120 m to the shop's corner. Streetlights are spaced too far apart — four poles on the route, one of them dead, so the walk is: light → long dark stretch → light → the dead pole's dark stretch → the shop's glow. Across the road, midway, the long blank wall of the school runs for 40 m, pale and featureless, with palm crowns black above it. Closed metal gates in courtyard walls face the street; behind one or two, a courtyard light leaks under the gate as a thin warm line. A speed bump. A pothole holding a little dark water that reflects the streetlight. At the far end, before the shop, an empty corner lot with rubble, a stack of hollow blocks, and dry weeds.

**Props.**
- Streetlight poles (concrete or galvanized steel, sodium heads; one dead, one with a dying flicker cycle — slow, irregular, never strobe-fast)
- Courtyard gates (sheet metal, various colors, padlocked hasps, painted phone numbers of plumbers/electricians stenciled on walls)
- Parked cars at irregular intervals; one car under a dusty tarp
- Dumpster / communal bins at one corner, bags around it, a cat that slips away when approached
- Utility pole with a knot of wires and an illegally spliced connection drooping to a house
- Speed bump, faded paint; pothole puddle
- Rubble lot: hollow cement blocks, sand pile, a wheelbarrow shell, dry weeds
- Old election/ad poster remnants peeling on a wall; a hand-painted "for sale" phone number on a gate
- Distant mosque minaret visible over the rooftops (green light at the crown)
- Bus-stop-like bench slab where old men sit in daytime — empty at night, one plastic chair left out

**Lighting.**
- *Day:* wide open, bleached, heat shimmer down the road, harsh and boring in a good way.
- *Night:* pools of sodium yellow with real darkness between them. The school wall opposite catches almost no light — it reads as a pale grey band under a black tree line. The shop's cool fluorescent glow is visible from ~80 m as the destination. Moonlight gives the asphalt a faint sheen; the dirt shoulders eat all light.

**Sound.**
- Night: the player's own footsteps change surface (asphalt → grit → asphalt). Wind gusts funneled down the road. Far dog barking, occasionally closer than expected. A single distant car passing on some other street — heard, never seen. Papery rattle of palm fronds from the school. The dying streetlight buzzes and clicks. Total absence of human voices.

**Player visibility.**
Sightline management: the player can always see the *next* light but never what's between the lights until they're in it. The school gate (Location 5) becomes visible across the road at the midpoint — the one place on the walk where the player has darkness on both sides. Looking back, the alley mouth light is small and far.

---

### Location 4 — The Shop / الدكان

**Exterior.**
A house corner converted into a shop: one wide opening with a roll-down metal shutter (day: half-raised even when open; night: raised, spilling light onto the pavement). Above it, a sun-bleached sign — hand-painted lettering plus a faded soft-drink logo, one corner of the sign bent. Outside: crates of empty glass bottles stacked against the wall, a chest freezer with a padlock, a bag of onions or a produce rack pulled halfway in, a hanging bunch of chip packets on a clip strip beside the door, and a plastic chair where the shopkeeper sits. A bug zapper or bare fluorescent tube under the eave. The shop's light is the only cool-white light in the neighborhood exterior — an island of fluorescent normality.

**Interior.**
Tiny: 4 × 5 m, shelves to the ceiling on three walls, a counter across the back corner defending the cigarette shelf and the cash drawer. Floor: worn terrazzo tile. Ceiling: one fluorescent twin-tube, one tube slightly pinker/older than the other. Shelves crowded but ordered: canned tomatoes, tuna tins, pasta, powdered milk, biscuits, cleaning products, a top shelf of dusty rarely-sold items. Bread arrives in big clear plastic bags stacked in a wire basket / on a low table near the door. A glass-front drink fridge with an aging compressor hums constantly — the loudest thing in the room. Behind the counter: cigarettes, phone-credit cards taped to the shelf edge, an old CRT or small TV on a bracket playing quietly, a wall calendar, a string of prayer beads on a nail, a tiny fan. A hanging scale. Fly strip near the light.

**Props (interior).**
Counter (laminate, worn edge, glass candy display on top) · wire bread basket + bread bags · drink fridge (branded, yellowed door seal) · chest freezer · metal/wood shelving, crowded product boxes · biscuit and chips cartons on the floor · hanging clip strips of chips/shampoo sachets · scale · calculator with tape · cash drawer · wall calendar · small TV/radio · fluorescent fixture + fly strip · broom behind the door · stacked egg trays · a kids' toy rack (cheap plastic toys in bags, slightly creepy only because everything else is closed for the night)

**Lighting.**
- *Day:* shutter half up, deep interior contrast — bright street, dim cave of goods.
- *Night:* fluorescent white flattens everything. No shadows to hide anything. This must be the most *normal*-feeling place in the game at night — the horror outside works because this room works as relief. The light spills a hard-edged pool onto the street; stepping out of it back into sodium-and-dark is the transition the whole location exists for.

**Sound.**
Fridge compressor hum + rattle cycle, fluorescent buzz, TV murmur, fly strip nothing, the shutter's bang if touched. Outside sounds vanish inside; when the player steps back out, the night's silence returns slightly louder than before.

**Player visibility.**
From inside, the doorway frames pure darkness across the road. Design the shelving so the player keeps their back to the door at the bread basket — the mirror above the counter corner (a small convex security mirror) shows the doorway behind them, distorted.

---

### Location 5 — The School

**Description.**
A government primary school on the north side of the main road: long two-story block, render in institutional two-tone (ochre band below, cream above), rows of identical barred windows, some panes broken and patched with cardboard. A walled yard with a sand/asphalt playground, one rusted goal frame without a net, a flagpole with a frayed rope that clinks against the pole in the wind. Front: a wide sheet-metal gate (blue, chained) with a smaller pedestrian door cut into it, sagging enough to leave a 20 cm gap at the bottom corner. A painted mural on the outer wall — sun, children's handprints, faded and peeling, benign by day and wrong at night. Palm trees and one big ficus inside the yard throw moving shadows over the wall. Behind the school, the open dirt field stretches away to the edge of the map.

**Props.**
- Metal gate + chain + padlock; pedestrian door with the corner gap
- Flagpole with a clinking halyard
- Goal frame, half-buried tire seats (painted tires used as playground seats), broken bench
- Rows of barred windows; a few open shutters upstairs that shouldn't be open
- Peeling mural, hand-painted school name plaque over the gate
- Palm trees, ficus, drifted leaves and litter against the wall
- A single external stair to the roof, visible above the wall line
- Water tank cluster on the school roof, one tank leaning
- Abandoned kiosk/guard hut by the gate, window dark

**Lighting.**
- *Day:* flat, institutional, empty (weekend/holiday feel), heat over the yard.
- *Night:* the school has **no working exterior lights**. It's lit only by spill from the road's nearest streetlight and moonlight — a pale wall under black trees. One upstairs window should be designed to catch a faint sourceless-seeming reflection (in reality: moonlight angle on glass) so it sometimes reads as lit when viewed from one specific spot on the road. That window and the gap under the pedestrian gate are the two "creepy angles": frames where the player's eye expects a figure. Keep them empty; the architecture does the work.

**Sound.**
Flagpole rope clinking (irregular, metallic, the signature sound of this location). Palm fronds. Wind crossing the open field behind — a broader, hollower wind than in the alleys. Occasionally the gate's pedestrian door shifts against its chain with a soft knock.

**Player visibility.**
The player passes the school across the road — 8+ m of distance, which makes it worse, not better: it's always in peripheral vision for ~40 m of walking, too far to inspect, too big to ignore. The upstairs window row is angled to face the shop walk. From the school corner, the open field reads as a void with the city glow far behind it.

---

### Location 6 — The Abandoned House (exterior)

**Description.**
At the end of the dead-end alley branch. Same construction as every other house — that's the point — but twenty years unmaintained. Single story plus an unfinished second floor of bare block. The render is cracked in map-like patterns and has fallen away in patches showing block and mortar. The courtyard wall (2.2 m) still stands but the courtyard's metal gate hangs on one hinge, wedged half open by drifted sand and trash — it hasn't closed in years and can't. Behind it, a small courtyard: dead planter bed, drifted plastic bags, a dry olive tree in the corner, broken roof tiles in a pile. The front door beyond the courtyard is metal like every other door in the neighborhood, but rusted to brown-black, dented low as if kicked, with a hasp and a padlock that someone has already broken — the lock hangs open on the hasp. The electric meter box beside the door is dead, glass smashed, wires cut and taped. No light fixture survives. Above, the unfinished second floor's window openings are raw block holes with no frames — black rectangles. Rebar reeds on the corner columns. On the parapet edge, the silhouette of a water tank and a dish leaning at a wrong angle.

**Signs of illegal visits (subtle, ground level only):**
- Cigarette butts collected in one sheltered corner of the courtyard, a crushed pack
- Two empty cans and a blackened spot where someone once made a small fire
- A scrawl of spray tag on the courtyard wall, plus one older, half-whitewashed scrawl under it
- Footpath: the drifted sand in the gateway has a worn channel through it — people step through the same gap
- A padlock replaced and broken more than once: two older broken locks in the dirt below the hasp

**Lighting.**
- *Day:* even in daylight it sits in the shade of the alley's end; the sun only reaches the courtyard at midday. Dust motes, hard contrast in the doorway.
- *Night:* zero artificial light. The nearest streetlight is around the bend, ~30 m back. The house is lit by moon and by memory of the light behind you. The doorway is a pure black rectangle; the second-floor window holes are blacker. If the player brings any light source, its beam should feel like trespass.

**Sound.**
The alley wind stops here — dead-end air is still. Instead: sand grit under the gate when it shifts (a low metal groan, rare), plastic bags whispering in the dry olive tree, and above, something on the roof level responding to wind — a loose sheet of zinc that lifts and settles. From inside the doorway: nothing, which after the walk's ambient sound feels like pressure.

**Player visibility.**
Approach is a straight 25 m with walls on both sides and the house filling the end of the frame — no way to see it obliquely, no way to circle it. The half-open gate reads from distance; the open padlock reads only up close. Looking up from the courtyard, the player sees the rooftop parapet and the top of a small rooftop-room structure (Location 10's exterior) — a box on the roof with one tiny shuttered window.

---

### Location 7 — The Abandoned House (interior, ground floor)

**Description.**
A traditional small Libyan house layout: the metal door opens into a short entry hall (دهليز) — deliberately angled so you cannot see into the house from the street — which opens into a small central hall with rooms off it: a sitting room, one bedroom, a kitchen at the back, a tiny bathroom. Ceilings 2.9 m, concrete slab with formwork lines visible where paint fell. Floors: patterned cement tile, dust-drowned so the pattern only shows in the swept channel of previous visitors' footsteps. Walls painted two-tone long ago (dark oil-paint lower band, light upper) — the classic old-house scheme — now flaking in curls. Ceiling wire stubs where light fittings were stripped. Every window barred, glass broken behind the bars, so the rooms get thin blades of outside light.

**Room by room:**
- **Entry hall:** empty except drifted sand and leaves near the door, a broken shoe rack, one child-sized sandal near the wall. Hooks on the wall.
- **Sitting room:** the skeleton of Libyan floor seating — a torn floor mattress (فراش عربي) against one wall, foam yellowed and burst, cushions gone or gutted. A carpet rolled and slumped in the corner, chewed. A wall niche shelf with a broken teapot and glass. Curtain rod hanging by one bracket, rag of curtain. Cigarette butts and a candle stub on a saucer near the mattress — visitor debris, not household debris.
- **Bedroom (ground):** an iron bed frame with no mattress, a wardrobe with one door off leaning against the wall beside it, empty hangers inside, a drawer pulled out on the floor. Newspaper sheets (Arabic print, sun-browned) covering part of a broken window pane.
- **Kitchen:** concrete counter with tiled top, tiles cracked; a hole and stains where the sink was stripped; a rusted two-burner stove; a dead refrigerator with the door removed (leaned against the wall — old safety habit); empty shelf brackets; a soot fan of black up the wall above the stove; scattered tins and a pot with a fossilized residue. Back door to the light-well, chained.
- **Bathroom:** tiny, tiled, drain hole, a bucket, a tap that (visibly) drips into a rust stain — the only "alive" thing in the house.
- **Central hall:** the stairwell door — a wooden door, half open, revealing the first steps up (Location 8). This is the composition's focal point: everything in the hall sightlines toward it.

**Props summary (interior ground floor).**
Torn floor mattresses · rolled carpet · iron bed frame · broken wardrobe + hangers · wall niches with broken teapot/glasses · curtain rags · newspaper on windows · dead fridge with door off · rusted stove · cracked tile counter · scattered pots/tins · candle stubs, saucers, cigarette butts, a lighter · child's sandal · dust with existing footprints (a worn path: door → sitting room → stairs; nothing wanders into the other rooms) · flaking two-tone paint · wire stubs from ceilings · broken switch plates

**Lighting.**
- *Day (if ever seen):* blades of white light through barred broken windows, dust hanging in them; interior otherwise brown-dark.
- *Night:* virtually black. Moonlight enters as faint grey panels on the floor below each window, cut by bar shadows. Any player-carried light gives small warm coverage that makes the darkness beyond it read darker. The stairwell door leaks a thin, slightly cooler light from above (moonlight falling down the stairwell from the roof level) — the house is dark but *up* is faintly lit, which is what pulls the eye.

**Sound.**
Interior silence with texture: the bathroom tap's slow drip (irregular period), grit under the player's own steps, paint flakes crunching, the zinc sheet on the roof shifting overhead — now *above* the player and felt in the ceiling. Wind is only audible as a low whistle at the broken windows. Every player-caused sound (a kicked tin, the wardrobe door) should ring loud against the silence.

**Player visibility.**
The angled entry hall guarantees a blind corner within the first three steps inside. Room doorways are staggered so no single position sees all rooms. The stair door is visible from the entrance hall's end — faint cool light in its gap — and everything about the ground floor should feel like a lobby for that door.

---

### Location 8 — The Stairs

**Description.**
A Libyan interior stair: narrow (80–90 cm), steep, cast concrete with terrazzo treads, switching back once at a mid landing. The stairwell is a tight shaft — walls close on both sides, ceiling low over the flights. The metal handrail (simple square-tube) is anchored loose; two of its floor bolts have pulled out so it shifts if touched. Treads: the third and ninth are chipped down to aggregate; one mid-flight tread has a crack across it and a spall missing at the nose. Dust on the treads carries the same single worn footpath line up the center. At the mid landing: a tiny slit window, glassless, letting in a slab of moon/streetlight — the only light in the shaft, and it lights the *landing*, not the flights, so the player climbs from dark into light into dark. Wall cracks run diagonally from the window corner. Paint on the stair walls is scribbled at child height near the bottom (crayon lines, faded) and bare above. At the top: a metal door to the roof, ajar, its lower corner scraping an arc worn into the concrete.

**Props.**
Loose square-tube handrail · chipped/cracked treads · slit window (no glass) · crayon marks low on the wall · a dropped and dusty single glove or rag on the mid landing · nail heads and a hook in the shaft wall · hanging wire stub from the shaft ceiling · the arc scrape under the roof door · drifted grit heavier near the top (blown in from the roof)

**Lighting.**
Night: one cold moonlight slab on the mid landing; darkness above and below it. When the player is on the lower flight, the landing light shows the *turn* but not what's around it. Approaching the top, the roof door's gap glows faint blue-grey with night sky. No warm light anywhere in the shaft — this is the coldest lighting in the game.

**Sound.**
The shaft is an amplifier: the player's steps double with a close slap-back echo that doesn't exist elsewhere. The handrail knocks against its loose anchor if brushed. Wind pulses at the slit window — a breathing sound with no rhythm. From above, the zinc sheet and a wire tapping the tank. Total absence of the ground floor's drip once past the landing; sound cues swap from "house below" to "sky above."

**Player visibility.**
Sightlines are designed to be hostile: on any flight, the player can see at most six steps ahead before the turn. Looking back down from the landing, the bottom of the stairs is a black pool. The stairwell is the one place in the game where the player can see neither an exit nor open space — maximum enclosure before maximum openness (the roof).

---

### Location 9 — The Rooftop / Unfinished Second Floor

**Description.**
The roof door opens onto the flat roof: a concrete slab, ~9 × 11 m, with a 1 m parapet on all sides. Half the roof was meant to become a second floor: two courses of block wall trace unbuilt rooms, corner columns with rebar reeds, a pallet of cement bags long since hardened into stone, a sand pile crusted over. The other half is normal Libyan roof life gone to seed: the black polyethylene water tank on a rusted steel stand (ladder welded to its side), a satellite dish knocked off-aim, a clothesline with two wooden pins and one petrified rag, an old foam mattress folded against the parapet (people once slept up here on hot nights), a broken white plastic chair on its side, a car seat someone hauled up, loose wires crossing the slab from a dead junction box, concrete blocks in a loose stack, the zinc sheet — the sound the player has heard all night — lying over a hole where a roof hatch was, lifting at one corner in gusts. Bird droppings and two or three pigeon feathers near the tank shadow.

And in the corner, built against the parapet: **the rooftop room** (غرفة السطح) — a small block structure, 2.5 × 3 m, rendered but unpainted, with a single metal door and one tiny high window, shuttered from inside. The door is the only locked thing in the entire house. The hasp is industrial, the padlock newer than everything around it, and the door's lower face carries old scratch marks (see Location 10).

**Props.**
Water tank + stand + float valve pipe · satellite dish (off-aim) + LNB cable snaking to a drilled hole · clothesline + pins + rag · folded rotten mattress · broken plastic chair · salvaged car seat · hardened cement bags · sand pile · block stacks · rebar column reeds · zinc sheet over hatch hole · dead junction box + loose wires · TV antenna fallen and leaning in a corner · a child's kite frame skeleton wedged under blocks (sticks and a rag of plastic) · pigeon droppings/feathers · rain stains fanning from parapet drain spouts

**Lighting.**
- *Night:* the shock of the roof is light: after the house and the stairs, the player steps into open moonlight and the whole sky. Cold blue-grey ambient across the slab, hard tank shadow, the parapet cutting a clean line against the sky. Over the parapet: the neighborhood at night — a scatter of warm sodium points, the family house's entrance lamp findable among them, the shop's cool glow at the far corner, the minaret's green light, the city's orange smear on the horizon. The rooftop room's corner is the exception: the parapet and tank shadow it, and its door sits in the one wedge of the roof that moonlight never reaches at any hour.

**Sound.**
Full wind, unbroken — the loudest ambient in the game, but *open* rather than threatening. The tank's water moves once in a while (a deep soft slosh). The zinc sheet's lift-and-settle is finally located and explained. The wire taps the tank stand. Distant dogs, now below the player. Near the rooftop room's corner, the parapet blocks the wind: a pocket of unnatural quiet three steps wide, so approaching the locked door means the wind dying mid-stride.

**Player visibility.**
The roof gives the game's only overview: the player can visually re-map everything they've walked — alley, road, shop light, school mass, field. This orientation is the trap's comfort. Turning from the view to the corner room reverses it: the closer the player gets to the door, the less of the neighborhood remains in frame, until at the door itself the parapet hides every light and the view is only block, metal, and scratches.

---

### Location 10 — The Locked Room (غرفة السطح)

**Description.**
2.5 × 3 m, 2.4 m ceiling. Sealed for years: when the door opens, the air should read as different — the game's stillest, dustiest air, motes hanging in whatever light enters. The tiny high window is shuttered from *inside* with a wooden shutter, nailed shut; thin lines of moonlight enter at its warped edges and lay two faint stripes on the opposite wall. The room was lived in — that's what's wrong with it. Not storage: lived in.

**Contents (composed, not scattered):**
- A narrow steel-frame bed against the long wall, thin mattress still on it, bedding long rotted to a grey compressed layer, the pillow with a stain-shadow at its center. The bed is *made* — roughly, but made.
- A small dusty cabinet (two doors, one warped and cracked open a finger's width). Inside: folded child-sized clothes gone stiff, a tin cup, an empty medicine strip, a comb.
- A wall mirror — broken, but present: the frame hangs level, most of the glass fallen and lying below it against the skirting in three large blades, one shard still in the frame's corner. The fallen pieces are dust-covered; the wall behind where the glass was shows the paint's original, un-faded color — a pale sharp rectangle the room's eye keeps returning to.
- An old cardboard/metal box under the bed, string-tied, dust on it disturbed in a way that says it was slid out and pushed back at least once, long ago.
- Torn photographs: a handful of black-and-white and 80s-color photo scraps in and around the box — faces missing (torn away, not scribbled), backgrounds of a courtyard and a beach remaining. A few scraps have drifted into the corner.
- Children's drawings, low on one wall: crayon and pencil directly on the paint — a house, a sun, stick figures — ordinary and cheerful, dense in one 1 m band at child height and then, further along the same wall, growing sparser and simpler until the last ones are only repeated vertical lines.
- The door's inner face: scratch marks concentrated around the handle-height and low near the floor — worn, old, varnished over by grime, unmistakably from the inside. Above the handle, the paint is rubbed to metal in one hand-sized patch.
- A plate and spoon on the floor near the door, both empty, both dusty.
- A nail in the wall with a small ring of thread/faded ribbon hanging on it.
- Ceiling: a single wire stub, no bulb. This room never gets artificial light.

**Lighting.**
Only the shutter-edge moonlines and whatever the player carries. The moonlines move very slowly (real-time believable, not animated spectacle). Player light here should render warm against cold — dust in the beam, the mirror blades throwing broken reflections of the beam onto the ceiling when the player's light crosses them: the only "light event" in the room, fully physical, deeply uncomfortable.

**Sound.**
Nothing. Design an active silence: interior wind and tank sounds fully muffled to a low felt pressure, the player's own breath-space becoming audible in the mix. The single exception: when the wind gusts hard outside, the nailed shutter strains once — a single wooden creak with a long silence after it. No sound in the game should be more restrained than this room.

**Player visibility.**
The room is small enough to see entirely in one sweep — and that's the design: nothing hides. The first sweep reads "old bedroom"; dread arrives only through the second look, when the details assemble (made bed, inside scratches, plate by the door, the drawings' decay along the wall). Nothing moves, nothing appears. The room's job is to be worse the longer it is looked at.

---

## 4. Global Lighting & Atmosphere

### Day (if the game includes any daytime)
- Sun: high, hard, slightly warm white (5600–6000K). Shadows short, opaque, sharp.
- Sky: pale blue washed toward white at the horizon, faint dust haze (subtle fog, warm grey, density rising with distance).
- Exposure: bright and slightly over — daylight should feel exposed, glare off render and dust.
- Palette effect: everything sun-bleached; colors read 20% desaturated from their painted values.

### Night (primary game state)
- Moon: the true key light. Cool grey-blue (~8000–10000K feel), low intensity, hard enough for readable shadows on rooftops and open road, negligible inside alleys.
- Sodium streetlights: warm orange-yellow (~2000–2200K), tight pools, visible falloff, slight downward haze cone in the dust. One flickering lamp maximum on the whole map (the dying one on the shop road) — flicker as slow irregular sag-and-recover, never horror-movie strobe.
- Domestic light: warm bulbs (2700K) at doorways and behind curtains. Curtained windows glow as soft rectangles; never show interiors.
- Shop: cool fluorescent (4000–5000K) — deliberately the only cool artificial light, so "cold white light" subconsciously equals "the safe shop," which the school's moonlit wall then quietly mis-echoes.
- Fog: very light global night fog, blue-grey, just enough to swallow the far ends of streets and put halos on distant lamps. Denser in the dead-end alley (still air), absent on the rooftop (wind).
- Abandoned house interior: no lighting at all beyond window moonlight panels; rely on the player's carried light and true black. Let blacks actually crush; do not lift shadows for readability inside this house.

### Atmosphere rules
- No colored lights except the minaret's distant green and the far radio tower's red blink.
- Dust: floating particulate visible only inside light beams and pools; wind-blown grit sheets at ground level on the open road during gusts.
- Weather: dry. No rain. Wind is the weather. Optional far dry-lightning flicker on the horizon once or twice a night, no thunder.

---

## 5. Color Palette

### Base world (day-bleached materials)
| Use | Color | Hex (albedo guide) |
|---|---|---|
| Render, cream | faded warm cream | `#D8C9A8` |
| Render, ochre | dusty ochre | `#C2A171` |
| Render, green | chalky pale green | `#A8B396` |
| Bare cinder block | grey-tan block | `#9E9484` |
| Concrete slab/road | worn grey | `#8A857C` |
| Dust/sand ground | pale dust | `#C7B594` |
| Metal doors | oxidized green / brown / blue | `#4E5D4A` / `#6B4A38` / `#3E566B` |
| Rust | rust bloom | `#7A4326` |
| Water tanks | aged black poly | `#2B2B29` |

### Night grade
| Zone | Cast |
|---|---|
| Safe pools (doors, streetlamps) | warm amber `#E8A84C` light on warm-grey shadows |
| Open night (moon) | desaturated blue-grey, shadows toward `#1C2026` |
| Shop | neutral-cool white `#E9F0EE`, minimal color cast |
| Abandoned house interior | near-black browns, `#141210` floor of the grade |
| Locked room | coldest and stillest: blue-grey moonlines over brown-black, player light warm `#F2C98C` |

Grading intent: warm = inhabited, cool = exposed, black = wrong. Keep global saturation low (–15 to –25%); let the only saturated pixels at night be the distant green minaret light and the red tower blink.

---

## 6. Sound Atmosphere Summary

| Location | Bed | Signature sounds | Silence usage |
|---|---|---|---|
| Main alley | low wind, far city | loose cable tap, TV murmur, cat in trash | gaps between houses |
| Family house | interior murmur, A/C drone | drip on concrete, doorbell hum | none — always alive |
| Road to shop | funneled wind, far traffic | dog volleys, dying lamp buzz, palm rattle | mid-walk dead-lamp stretch |
| Shop | fridge hum, fluorescent buzz | shutter bang, TV, bag rustle | outside sounds cut to zero |
| School | field wind (hollow) | flagpole clink, gate knock on chain | yard interior is soundless |
| Abandoned ext. | still air | zinc sheet lift, bags in dry tree, gate groan | wind stops at the dead end |
| Abandoned int. | near silence | bathroom drip, grit steps, paint flakes | rooms without the drip |
| Stairs | shaft echo of self | rail knock, window wind-pulse | above the landing |
| Rooftop | full open wind | tank slosh, wire tap, zinc explained | wind-shadow at locked door |
| Locked room | active silence / pressure | one shutter creak per strong gust | the room *is* silence |

General: no music as ambience; if score exists it stays out of this document. All fear-sounds must have physical sources the player can eventually find (zinc sheet, cable, flag rope, gate) — the world never cheats.

---

## 7. Asset List (Unity URP / Blender)

### Modular architecture kit
- Wall segments: rendered (3 paint tones × clean/cracked/patched), bare block, courtyard wall + coping, parapet
- Corner columns with rebar tops (2 lengths)
- Metal double door (3 paints × 3 wear levels), single metal door, rooftop-room door with hasp/padlock, wooden interior doors (2 wear levels), roll shutter (open/half/closed states)
- Window modules: barred window (2 grill patterns) × (curtained / broken+newspaper / shuttered), raw block opening
- Roof kit: slab edges, parapet drain spouts, roof access door, hatch hole + zinc sheet
- Stair kit: flight, mid landing, slit window, square-tube handrail (intact/loose)
- Ground: asphalt (patched), dirt/sand, drainage channel, curb, speed bump, pothole, terrazzo interior tile, patterned cement tile (2 patterns), rubble/sand piles, hollow block stacks

### Props (shared)
Electric meter box (intact/dead) · wire bundles + catenary cables + wall-stapled runs · streetlight pole (on/dead/flicker) · doorway lamp (2 types) · A/C unit + drip stain decal · water tank + stand · satellite dish (aimed/fallen) · TV antenna · clothesline + pins · gas bottle · plastic chairs (intact/broken) · wooden stool · plant pots in tins/buckets · hose + wall hook · doormat + slippers · trash bags, cardboard, tins, drifting plastic bags (cloth sim or vertex-anim) · dumpster · car set: 90s sedan (clean/dusty/tarped), old pickup · cat (ambient agent) · pigeons/feathers/droppings decals

### Props (location-specific)
- Shop: shelving units, product fillers (generic Arabic-label boxes/cans — invent brands, no real trademarks), bread bags, wire basket, drink fridge, chest freezer, counter + candy display, scale, clip strips, fly strip, small TV, calendar, convex mirror
- School: gate + chain, flagpole + rope, goal frame, tire seats, mural decal set, name plaque, guard kiosk
- Abandoned house: floor mattress (torn), rolled carpet, iron bed frame, broken wardrobe + hangers, dead fridge (door off), rusted stove, cracked tile counter, niche shelf + broken teapot/glasses, candle stubs, cigarette debris set, broken padlocks, child's sandal
- Locked room: steel bed + rotted bedding, small cabinet + stiff clothes, broken mirror (frame + wall shard + floor blades), string-tied box, photo scraps, plate + spoon, ribbon on nail, shutter (nailed)

### Decals & textures
Cracks (3 scales) · flaking two-tone paint · damp/AC drip stains · soot fan · rust streaks · chalk kids' marks · crayon drawings (dense→sparse sequence for the locked room) · spray tags (fresh + whitewashed) · scratch-mark set (door, two densities) · footprint path in dust · Arabic hand-painted signage set (shop sign, house numbers, plumber phone numbers, "ممنوع الوقوف" no-parking, faded poster remnants)

### Audio assets
Wind set (alley funnel / open road / rooftop / shaft pulse) · dog volleys (3 distances) · cat rustle/mew · loose cable tap · zinc lift-settle · flag rope clink · gate chain knock · fridge hum + rattle · fluorescent buzz + dying-lamp cycle · drip (irregular) · grit/tile/asphalt/concrete footstep sets · paint-flake crunch · handrail knock · tank slosh · shutter strain creak · far traffic · far generator · distant single car pass · TV/radio murmur (muffled, indistinct)

### Sky/FX
Night HDRI (moon, high thin cloud) · day HDRI (dust-hazed) · height fog (URP volumetrics or fog particles) · dust motes particle (light-volume bound) · ground grit gust sheets · far city glow card · minaret + green light card · radio tower blink card

---

## 8. Making It Authentically Libyan

1. **The unfinished second floor is not decay — it's hope.** Families build the ground floor, live in it, and leave rebar up for the day a son marries. Bare block + rebar should appear on *inhabited* houses too, or the abandoned house won't read as different in kind.
2. **Two-tone interior paint** (dark oil-paint band to ~1.2 m, lighter above) instantly says "old Libyan house" to anyone who's been in one.
3. **Floor seating, not sofas.** The sitting room is mattresses against walls (فراش), a low table, a carpet. A western sofa in the abandoned house would break the spell.
4. **Doors are metal, windows are barred, everywhere.** This is normal security, not menace — which is what makes the one broken door menacing.
5. **The meter box by every door** with hand-scrawled numbers, and wiring that visibly improvises (splices, staples, a neighbor's line borrowed across the alley).
6. **Water is architecture:** rooftop tanks on every silhouette, float-valve pipes, a tap stub low on street walls, hoses. A neighborhood skyline without tanks reads instantly false.
7. **Dust as a material, not an effect.** Sand drifts against every wall base, collects in door channels, buries tile patterns. Wind direction should be consistent — drifts on the same side of every street.
8. **Signage:** hand-painted Arabic in imperfect lettering — shop name, plumbers' phone numbers on walls, "بيع" (for sale) with a number, kids' chalk. Use Libyan phone number formats (091/092/094… prefixes). Avoid real brands; invent plausible Arabic product names.
9. **Sound of faith kept distant and warm:** the far minaret's green light; if the timeline touches dawn, the far fajr adhan is the most authentic "night is ending" signal possible — treat it with complete respect, as safety and normalcy returning, never as a horror element.
10. **Cats own the street.** Multiple strays, unbothered, well-fed by neighbors (the food dish prop). Their *absence* near the abandoned house is a detail Libyan players will feel before they name it.
11. **Life debris is social:** plastic chairs outside doors, a backgammon/dominoes table corner, tea glasses left on a windowsill — evening street life frozen, everyone gone inside.
12. **Restraint with religion and politics:** no Quranic text used for horror, no political symbols, no real party/era references. Whitewashed-over graffiti says "history exists here" without saying which.

---

## 9. What the Player Sees — Master Visibility Table

| From | Can see | Deliberately cannot see |
|---|---|---|
| Start (family doorstep) | own lit door, alley both ways to the bend, rooftop tanks against sky | the abandoned house, the dead-end branch's end |
| Alley bend | alley mouth light, the black mouth of the dead-end branch | inside the branch beyond ~10 m |
| Alley mouth | main road both ways, next streetlight, shop glow far right, school wall far left | anything between light pools |
| Mid road (dead lamp) | school gate + upstairs windows across the road, shop glow ahead, alley light behind | school yard interior, the field |
| Shop interior | doorway framing the dark road; convex mirror view behind self | everything else outside |
| Dead-end branch | abandoned house filling the frame, gate gap, black doorway | any side/rear of the house — no circling |
| Abandoned courtyard | broken locks, worn footpath, second-floor black openings, rooftop-room top edge | interior past the angled entry |
| Ground-floor hall | staggered room doorways, stair door with cool light in the gap | more than one room at a time |
| Stairs | six treads max ahead; landing light; roof door glow at top | bottom of stairs from above (black pool) |
| Rooftop | the whole neighborhood mapped in lights: home lamp, shop, school mass, minaret, city glow | the locked-room corner keeps the view at its back |
| Locked room | everything at once — the room hides nothing | any exterior light; the world outside is gone |

---

## 10. Build Notes (non-code)

- **Blockout order:** road + alley network → house facade masses → school + field → abandoned house shell → interiors → prop pass → decal pass → lighting pass → audio pass.
- **Scenes:** one exterior scene; shop interior and abandoned-house interior may be sub-scenes/additive if performance demands, with doorway transitions hidden by the angled entry hall and the shop's deep shutter opening.
- **Modularity:** the whole neighborhood should build from ~12 wall modules, ~6 door types, ~4 window types, retextured — repetition is authentic here; break it with props, stains, and paint tones, not with new architecture.
- **Lighting tech (URP):** baked GI for the stable night state, real-time only for player-carried light and the flickering lamp; reflection probe inside the shop and one on the roof; light cookies for the sodium pools' texture.
- **LOD/skyline:** rooftop clutter (tanks, dishes, antennas) needs mid-distance LODs — the skyline silhouette is a primary horror surface and must never pop visibly.
- **True black policy:** calibrate the abandoned-house interior on a dim-capable display; resist lifting shadows. The design depends on darkness that is actually dark.
