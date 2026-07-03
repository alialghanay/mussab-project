# The Abandoned House — Complete Horror Level Design

Companion to `WorldDesign.md`. This document supersedes sections 6–10 of that file
wherever they conflict: the house is now larger, deeper, and structured as the game's
centerpiece level. Everything else about the world (dead-end alley approach, no basement,
rooftop locked room, realistic Libyan construction) still holds.

Environment design only: no story, characters, dialogue, code, combat, or monsters.

---

## 1. Design Intent

### 1.1 The house is the main character

Every other location in the game exists to make this one land. The neighborhood taught
the player the rules of a Libyan house — metal door, barred windows, courtyard wall,
warm bulb over the threshold. This house obeys every rule and is wrong anyway. Nothing
inside is impossible; everything inside is slightly off: too lived-in for an abandoned
house, too still for a lived-in one.

### 1.2 The core trick: bigger inside than outside

From the dead-end alley the player sees a narrow, one-story facade — maybe 8 m wide —
because the alley only shows the house's east end. The house is actually a traditional
**courtyard house (حوش عربي)**: a 14 × 16 m footprint that extends back and sideways
behind the neighbors' walls, wrapped around a central open courtyard. The player enters
expecting three rooms and finds nine, plus a floor above. The moment of realization —
stepping out of the dark bent entrance into a moonlit open courtyard *inside* the house —
is the level's first scare, and it uses no event at all. Just architecture.

### 1.3 Horror rules for this level

1. **Nothing supernatural is ever shown.** No monsters, no apparitions, no impossible
   geometry. The house scares with silence, darkness, decay, and arrangement.
2. **Wrongness is quiet.** A chair facing a corner. A bed too neatly made. A plate by a
   door. One drawing that stopped being a drawing. The player should keep asking "wait —
   why is that like that?" and never be answered.
3. **Blood budget: almost zero.** No gore. The strongest stains allowed are old water
   damage, soot, rust bleed, and one ambiguous dark patch (Section 12).
4. **Light is trust.** Ground floor: thin blades and grey panels. Stairs: one cold slab.
   Roof: full moon. Locked room: almost nothing. The player's own light is the only warm
   thing in the building, and it makes the darkness around it read darker.
5. **Sound never lies but often misleads.** Every sound has a real physical source
   (findable), but the player hears it before finding it — the zinc sheet, the tap,
   the shutter. Silence is deployed like a sound.

---

## 2. Layout

### 2.1 Ground floor plan (text map)

North is up. The alley approaches from the EAST (right). Dimensions in meters.

```
        W E S T   (deepest, darkest)                          E A S T  (alley)
   +-----------------+-----------------+--------------------+
   |                 |                 |                    |
   |   BACK ROOM     |    BEDROOM      |    LIVING ROOM     |    NORTH WING
   |   4.0 x 3.5     |    4.0 x 3.5    |    5.5 x 4.0       |
   |   (no window)   |                 |  (window to court) |
   +------+----------+----+------------+-----+--------------+
   |      door            door              door            |
   |                                                        |
   | STORAGE                COURTYARD            SQIFA ---> METAL DOOR
   | 2.5x3   door        (وسط الحوش)          (bent entry,   (facade on
   |      (windowless)     5.0 x 4.5           1.4 wide,      the alley)
   +--------+              OPEN SKY            2 turns)      |
   |        door                                door         |
   | KITCHEN     door  +--------+------+------+--------+    |
   | 3.5x3.0           | BATH   | WC*  |      | STAIR-  |    SOUTH WING
   |                   | 1.8x1.6| 1x1.2|      | WELL    |
   +-------------------+--------+------+------+---------+---+
                                        *WC optional/merged
```

- **Courtyard walls** rise the full two stories around the open center; the sky hole is
  ringed by the second floor's edge, so looking up from the courtyard the player sees
  black window openings looking back down (Section 13, technique 3).
- **Circulation is a loop:** sqifa → courtyard → any room → back to courtyard. Every
  room exits only to the courtyard. The player must re-cross the open center to go
  anywhere — the courtyard sees them constantly.
- **The stairwell door** is in the courtyard's southeast corner, right beside where the
  player entered. They walk past it on arrival without registering it; they find it last,
  and realize it was behind them the whole time.

### 2.2 Upper level plan

```
   +-----------------+-----------------+--------------------+
   | LOCKED ROOM     |   OPEN ROOF (over bedroom/back room) |
   | 2.5 x 3.0       |   tank, clothesline, mattress        |   ROOF LEVEL
   | (over the       |                                      |   (3 steps down
   |  back room)     |                                      |    from 2nd floor)
   +-----------------+--------+--------------+--------------+
   |     parapet w/ gap       |  COURTYARD   |  UNFINISHED  |
   |                          |  VOID        |  SECOND      |
   |   UNFINISHED ROOMS       |  (open drop, |  LANDING     |   2ND FLOOR
   |   (block walls 1-2m,     |   low block  |  + stair     |   (over south
   |    no roof, rebar)       |   edge wall) |    head      |    + east wings)
   +--------------------------+--------------+--------------+
```

- The stairs arrive at the **unfinished landing** (southeast). From there the player
  crosses the unfinished second floor — a maze of waist-high block walls tracing rooms
  that were never built — around the **courtyard void** (the open hole down to the
  courtyard, guarded only by two block courses).
- Three steps down onto the **roof slab** over the north/west wing: the open rooftop.
- The **locked room** sits at the far northwest corner — directly **above the back
  room**. The deepest point of the ground floor and the deepest point of the roof are
  the same corner of the house, stacked. (Sound design uses this: Section 6.6.)

### 2.3 Pacing distances

- Front door → courtyard: 7 m of bent corridor (two blind turns).
- Courtyard → farthest room (back room): 11 m, crossing open sky.
- Full ground-floor exploration loop: ~60 m of walking.
- Stairwell → locked room door: ~22 m across the unfinished floor and roof —
  the longest uninterrupted "exposed" walk in the house.

---

## 3. Exterior (rebuilt)

**What the alley shows.** Only the east facade: 8 m of cracked render, one story plus
the raw block edge of the second floor above. Courtyard wall (2.2 m) with the wedged
half-open gate. The house's true depth is hidden behind the neighbors' walls — from
outside there is no way to know how far back it goes.

**Facade details.**
- Render cracked in dry-mud patterns, fallen in two big patches (block exposed);
  one long diagonal structural crack from the door lintel to the roof — the crack the
  eye follows upward to the black second-floor openings.
- **Front door:** metal double-leaf, rusted brown-black, right leaf dented low (kicked),
  left leaf holding one broken hinge so the whole door stands slightly out of plumb.
  The hasp carries an open padlock; two older broken locks lie in the dirt below.
- **Electric meter:** smashed glass, door hanging, wires cut and taped dead. Beside it,
  the ghost rectangle of a removed house-number tile.
- **Windows:** two on the facade, both barred, both broken behind the bars. One is
  stuffed with browned newspaper; the other is a clean black hole with three glass
  teeth still in the frame. Glass shards below on the sill and ground.
- **Trash at the threshold:** drifted bags, a flattened juice carton, cigarette butts
  concentrated in the sheltered corner, one crushed pack, two cans by a fist-sized
  soot spot (an old tiny fire).
- **Illegal-entry evidence:** a worn channel through the drifted sand in the gateway;
  a spray tag on the courtyard wall with an older half-whitewashed one beneath; a milk
  crate placed under the lowest window (someone once climbed).
- **The weak streetlight.** The nearest sodium lamp is ~30 m back at the alley bend.
  It cannot light the house — but it lights the *player* from behind, throwing their
  own long shadow onto the facade as they approach. The closer they get, the taller
  their shadow climbs the wall, until it merges with the doorway's black. This is free,
  physically honest, and the best scare the exterior can offer.

**Lighting.** No artificial light on or in the house. Moon rakes the facade at a low
angle: render texture pops, doorway and window holes read as absolute black.

**Sound.** Dead-end stillness (the alley wind stops here). Bags whispering in the dry
olive tree. Above: the zinc sheet on the roof lifting and settling — the sound the
player will chase all the way to the top. From the doorway: nothing.

---

## 4. Entrance Corridor — السقيفة

The traditional bent entrance: 1.4 m wide, 7 m total, two 90° turns (privacy geometry —
you could never see the courtyard from the street). Perfect horror architecture, and
100% authentic.

**Design.**
- First leg (3 m): floor of patterned cement tile, half the tiles cracked or lifted,
  dust drifted into corners. Wall hooks, a broken shoe rack, shoes: one adult sandal,
  one child's sandal — **not a pair**, sitting neatly side by side anyway.
- First turn: total darkness for two steps — outside light dies here, courtyard light
  hasn't arrived yet. The corridor's one lightless joint.
- Second leg (4 m): peeling two-tone paint (dark oil band below, pale above) in long
  curls that brush the shoulders in the narrow width. A dead ceiling-light wire stub.
  On the wall, at child height, a pencil growth chart — names scratched out, heights
  and dates faint (dates only; no readable names — suggestion, not story).
- Second turn opens on the courtyard: the "bigger inside" reveal, framed by the dark
  corridor like a screen.

**Props.** Broken shoe rack · mismatched sandals · wall hooks + a rag of a jacket ·
lifted/cracked tiles · paint curls (decal + mesh flakes) · growth-chart decal · dust
drifts · a dead flashlight lying just inside the door (someone left in a hurry;
it doesn't work).

**Lighting.** A grey wedge of alley-moonlight reaching 2 m past the door, then black,
then the courtyard's cool glow around the second corner. The player's light feels
*loud* in here.

**Sound.** The threshold is an audio gate: crossing it drops the exterior ambience
(bags, wind) by half and adds close reflections — footsteps get a boxy slap. Grit and
tile-shard crunch underfoot. Far ahead, faintly: the courtyard's wind chime-less
quiet — one plastic sheet ticking somewhere above.

**Feel.** Unsafe, but pulling forward. The corridor should make the player walk slower
without any script: narrowness + turns + peeling walls at shoulder height do it.

---

## 5. The Courtyard — وسط الحوش

The heart of the house and its best trick: an open-air room, moonlit, *inside* the dark
building. The brightest space in the level — and somehow the least safe-feeling, because
the house surrounds it on all four sides and two stories, and every black doorway and
window opening faces the player standing in the light.

**Design.**
- 5 × 4.5 m, floor of large cement tiles with a center drain. Around the drain, a
  ghost-circle of cleaner tile where a fountain basin or big planter once stood.
- Overhead: the open sky, ringed by the second floor's edge. A **broken pergola** —
  three rusted pipes that once held a vine — crosses one corner; the dead vine still
  hangs from it as a black tangle, and its shadow moves on the tiles when the wind
  pushes it. (The only moving shadow in the courtyard, and it's explainable — barely.)
- One corner holds the household's outdoor life, abandoned mid-use: a broken white
  plastic chair **facing the corner, not the center** (wrongness detail — chairs face
  people, not walls), an old zinc water bucket with a dry brown ring inside, a low
  wooden stool, dead potted plants in paint tins along one wall — and one pot where
  something green actually still grows (the only living thing in the house).
- Strange stains: a long pale streak down one courtyard wall from a second-floor
  drain spout — mineral, white-grey, harmless — and beneath the pergola corner one
  darker patch in the tile grout, old and scrubbed-looking. Ambiguous. Never referenced
  again.
- Five doorways open onto the courtyard (living room, bedroom/back-room lobby, kitchen,
  bath, storage) plus the sqifa and the stairwell door. Two doors stand open, two ajar,
  one (storage) closed. Their states should feel arbitrary — like the last person left
  mid-thought.

**Props.** Center drain + ghost ring · broken pergola pipes + dead vine · plastic chair
(facing corner) · zinc bucket · wooden stool · paint-tin planters (dead ×4, alive ×1) ·
drain-spout stain decal · dark grout patch decal · drifted leaves and one drifted
plastic sheet that ticks against the pergola in gusts · a clothesline hook on the wall
with 1 m of frayed line.

**Lighting.** Full moonlight pool — the level's key image. Cool blue-grey, hard tile
shadows from the pergola and vine. The surrounding doorways read as pure black
rectangles ringing the light. At the sky opening's edge, the second-floor window holes
catch just enough moon to show they're openings — eyes-shaped, always in peripheral
vision, always empty.

**Sound.** Wind heard *above* but barely felt below (the court is a well of still air):
a high, hollow moving-air tone with the plastic sheet's irregular tick. The vine
scratches the pipe once in a while. Standing still long enough, the bathroom tap's
drip becomes audible from the south side — the sound that leads the player onward.

**Feel.** The level's ambiguous refuge. The player will return here between rooms to
breathe — let them. The courtyard never changes… except it's where returning players
will first notice door-state differences if the design uses them (Section 14).

---

## 6. The Rooms

### 6.1 Living room (north wing, 5.5 × 4 m)

The house's public face, frozen. Floor seating (فراش عربي) along three walls — torn
mattresses, burst foam yellowed, cushions gutted or missing. A dusty red machine-woven
carpet, its pattern only visible in the swept footpath across it. A low tea table with
a tin tray (صينية), three small glasses — **one still holding a fossilized brown residue
ring** — and a teapot on its side. An old CRT TV in the corner, screen cracked in a
spider web, its face turned ten degrees away from the seating — *toward the doorway*.
A wall cabinet niche with the family's good dishes gone, one broken cup left.

**Family photos.** A high shelf holds three framed photographs, glass dusty: a group in
a courtyard (this courtyard — same drain, same pergola, recognizable), a beach, a
school line-up. **Faces are not scratched out** in these — that's too loud. Instead
one frame lies face-down; lifting angle shows its backing is torn open, photo removed.
The scratched/torn photos are saved for the locked room; down here, absence does the work.

The dark corner: the room's northeast corner gets no window light from any angle. Put
nothing in it. An empty dark corner the player keeps checking is scarier than any prop.
The curtain: one window to the courtyard, rag of curtain on a bent rod. It stirs when
the courtyard's plastic sheet ticks (shared air path — physically honest) — movement
the player half-catches from the seating area.

- **Props:** floor mattresses ×3, gutted cushions, carpet + path, tea table + tray +
  glasses + fallen teapot, CRT TV (angled), TV doily, wall niche + broken cup, photo
  shelf + face-down frame, curtain rag, cigarette butts + saucer near one mattress
  (visitor debris), candle stub, wire stubs in ceiling.
- **Lighting:** one grey window-panel of courtyard-moonlight across the carpet, bar
  shadows from the window grill. Corners black.
- **Sound:** the quietest room's bed is faint courtyard air through the window; the
  curtain's fabric sound when it stirs; the TV's glass *ticks* once as it cools at
  night (a real phenomenon, used maybe twice, unexplained).
- **Feel:** falsely social. A room shaped for eight people, holding none.

### 6.2 Bedroom (north wing, 4 × 3.5 m)

Iron double bed frame, thin mattress on it. **The sheets are disturbed** — thrown back
in a diagonal, pillow dented — as if slept in last night; but the dust on the blanket's
folds is years thick. Both facts must read clearly and contradict each other. That
contradiction is the room.

An old wardrobe (one door off, leaning beside it): inside, clothes still on hangers —
men's shirts, a woman's coat — and a folded stack on the shelf. A wedding chest
(صندوق) at the bed's foot, lid open a hand's width, tissue paper inside, contents gone.
Cracked wall mirror over a small dresser — crack runs through where a face would be at
standing height (positioning, not magic). Hairbrush, empty perfume bottle, a jar of
dried-out henna on the dresser. Under the bed's edge, half-hidden by the blanket's
overhang: **a child's drawing, face-down** — the room's hidden detail; players who
crouch find a crayon house with too many windows drawn on it. No caption.

- **Props:** iron bed + disturbed bedding, wardrobe + hangers + clothes, leaning door,
  wedding chest, dresser + mirror (cracked at face height), hairbrush/bottle/henna jar,
  drawing under bed, curtain rag, prayer rug rolled on the wardrobe shelf (respectful,
  ordinary, human), suitcase on top of wardrobe furred with dust.
- **Lighting:** the darkest bedroom possible while readable: one thin blade through a
  gap in the newspaper-covered window crossing the bed diagonally — it lands on the
  thrown-back sheets. Mirror catches the player's light and throws a dim smear on the
  ceiling.
- **Sound:** near-silence; the house's settling creak used here at maximum sparsity —
  one wooden tick per several minutes. Under it, from above, *very* rarely: the grit-
  shift of the roof slab (wind load, real, and directly below the roof path to the
  locked room).
- **Feel:** trespass. This is someone's bedroom, and "someone" feels closer here than
  anywhere else on the ground floor.

### 6.3 Kitchen (southwest, 3.5 × 3 m)

Concrete counter, cracked tile top; the sink stripped for scrap — a hole with a rust
fan below it. A rusted two-burner stove; a **blue butane cylinder** still connected by
a perished hose (Libyan kitchens run on these). A dead fridge, door removed and leaned
against the wall. Open shelf brackets, shelves gone; one wall cabinet remains, **its
door hanging open at ~40° on a dying hinge** — set at scene start, never animated
(the player will still swear it moved). Broken dishes swept into a corner heap —
someone once cleaned up after the breakage, which is worse than the breakage. Empty
cans (tomato paste, tuna), a pot with fossilized residue, a soot fan up the wall over
the stove. On the counter: a tin of tea and a spoon, together, waiting.

- **Props:** counter + sink hole, stove, gas cylinder + hose, dead fridge + leaned
  door, wall cabinet (door ajar), dish heap, cans, pot, kettle, soot decal, spoon +
  tea tin, rat-gnawed carton, fly strip (dead) hanging from ceiling wire.
- **Lighting:** small barred window to the courtyard: grey panel on the counter,
  grill-bar shadows. The cabinet's open door hangs a hard shadow diagonal across
  the wall.
- **Sound:** the level's insect room: a fly buzz that rises near the sink hole and
  dish heap, ticking against a surface sometimes. Under it, the gas hose's dry rubber
  squeak when wind pressure changes (rare). The bathroom drip is loudest from here —
  next door.
- **Feel:** biological unease. Nothing threatens; everything is faintly sticky, dry,
  and wrong to touch.

### 6.4 Bathroom (south, 1.8 × 1.6 m)

The smallest, tightest space in the house — the door opens *inward*, forcing the player
to step into the room to open it fully, and it **cannot fully close behind them**
(swollen frame, drags on the tile, stops 20° open — the gap always shows a slice
of the dark hall).

Floor-level tiles stained in overlapping tide rings; a squat basin/drain in the floor;
a small cracked sink, its mirror above **broken into a fractured mosaic still in the
frame** — the player's face appears in it as five offset fragments (geometry does this,
nothing magical). The tap **drips** — the sound the whole south wing hears — into a
rust-brown stain. A zinc bucket half under the sink, an old water dipper (كوز), a rag
hard as wood on a nail, a shampoo bottle bleached colorless.

- **Props:** sink + fractured mirror, tap (dripping), floor drain, bucket, dipper,
  rag on nail, bleached bottle, tide-ring tile decals, swollen door.
- **Lighting:** none. A ventilation slit near the ceiling admits a 10 cm sliver of
  grey. The player's light in a 1.8 m mirrored box is claustrophobia itself: every
  beam angle bounces a fragment of their own movement back at them.
- **Sound:** the drip, close and irregular — and it must be mixed so that when the
  player stands perfectly still, the interval *feels* like it lengthens (it doesn't;
  never cheat measurably). Their own movement sounds huge; cloth brushes tile on both
  sides.
- **Feel:** the ground floor's pressure peak. Nobody stays in here one second longer
  than needed.

### 6.5 Storage room (west, 2.5 × 3 m, windowless)

The only ground-floor room with a **closed** door, and the only windowless one. Opening
it releases the still-air feeling (audio: a low pressure shift, nothing more).

Stacked and honest: cardboard boxes gone soft, string-tied bundles of blankets and
winter clothes, a sack of hardened plaster, paint cans with lids rusted on, a bicycle
frame without wheels, a cracked olive-oil jar (خابية) in the corner. **Broken toys** in
an open box — a truck without wheels, a doll with the paint of its face worn away by
handling (worn, not defaced) — and a deflated football. Bags of unknown items: two
woven plastic sacks (شوالة), knotted shut, contents lumpy and soft. They are never
openable and never explained.

**The clue-like object:** at the back, on a shelf alone, deliberately apart from the
clutter: a **child's school satchel**, buckled shut, dust-furred, placed square to the
shelf edge as if put there carefully. It can be looked at from all sides. It is never
opened. (Environmental suggestion only; no story text anywhere.)

- **Props:** soft boxes, blanket bundles, plaster sack, paint cans, bike frame, jar,
  toy box (truck/doll/football), knotted sacks ×2, shelf + satchel, wall of stacked
  junk making the room feel half its size.
- **Lighting:** absolute darkness. The room only ever shows what the player's beam
  touches; the doorway's grey behind them is the only orientation. The satchel shelf
  should sit just past the beam's comfortable reach so it's found late.
- **Sound:** dead room — the house's ambience drops out entirely inside (blankets and
  boxes eat sound; physically true). The player hears only themselves. Stepping back
  out, the courtyard's air returns like surfacing.
- **Feel:** the room players don't want to enter — and the design *makes* them choose
  to: the door must be opened, the dark must be walked into, and nothing forces it.

### 6.6 Back room (far west, 4 × 3.5 m) — the deepest point

The room furthest from the door, under the locked room. Its function is unclear — that
is the point. It was a bedroom once, maybe; now it holds a single item set:

The room is **almost empty**. Swept-looking floor (cleaner than every other room — dust
thin, as if disturbed more recently). Against the far wall: **one floor mattress, one
folded blanket on it, squared**. Beside it, a candle stub on a saucer with a dead match.
A water bottle, quarter full, its plastic not yet yellowed. On the wall above, faint
rectangular ghosts where pictures once hung — and **one small drawing pinned at child
height**: crayon, a house with a flat roof and a little box on top of the roof. The
little box has a scribbled black window.

The ceiling: bare slab — and the room's key sensory detail: this is where the roof
sounds are loudest. The zinc sheet, the grit-shift, the wind working the slab above —
all of it happens *directly overhead* in this room, because the locked room and its
corner of roof sit right on top.

- **Props:** floor mattress + folded blanket, candle + saucer + match, water bottle,
  picture-ghost decals, pinned crayon drawing, thin-dust floor treatment, a nail with
  a loop of cord by the door.
- **Lighting:** windowless like storage, but its door usually stands open to the
  bedroom lobby, admitting a faint grey throw. The mattress wall stays black until
  the player's light finds it.
- **Sound:** overhead activity (wind-driven, real, sparse). The one room where the
  house above is louder than the house around.
- **Feel:** recency. Everything else says "years." This room says "recently" and
  refuses to say more. It should quietly reorder the player's model of the house.

---

## 7. The Stairway

Relocated to the courtyard's southeast corner — behind the door the player walked past
in their first ten seconds. A concrete switchback shaft, 85 cm wide, treads terrazzo,
16 risers to the landing, 9 more to the top.

**Design.**
- Wooden door, half open, lower corner scraping an arc in the tile. Behind it, the
  first flight climbs away from the courtyard's light fast — by step four the player
  is in shaft-darkness.
- Treads 3 and 9 are chipped to aggregate; tread 12 has a **spall broken from its nose**
  (the "broken step" — visually alarming, physically passable; the player will step
  over it and hate it).
- The handrail is square steel tube, two floor anchors pulled loose: it *shifts* if
  brushed, with a knock that echoes up the shaft.
- Mid landing: a glassless **slit window** admitting the level's one cold slab of
  moonlight — it lights the landing and nothing else. Climbing = dark → light → dark.
- Wall writing: at the bottom, children's crayon scribbles (faded, low). From the
  landing upward the walls change: a tally-mark cluster scratched into the paint at
  shoulder height, then long incidental scratches (furniture was dragged up or down
  here — read as moving-day damage… or not), then bare dusty render. Writing gets
  *less* human as you climb.
- Creepy shadows, honestly sourced: the slit window's moonlight throws the handrail's
  broken silhouette up the stair wall — a bent, ladder-like shadow that slides as the
  player climbs past the landing. Test this in-engine; it's the shaft's key image.
- The top: a metal door to the unfinished floor, **ajar**, night sky glowing grey-blue
  in its gap. Its lower corner grinds an arc of bare concrete when pushed.

**Lighting.** One source: the landing slit. Flights black above and below. Player light
in the shaft reaches two walls at once (it's that narrow) — claustrophobic by geometry.

**Sound.** The shaft is the house's amplifier: close slap-back on every footstep,
doubled and hard — mix footsteps here +4 dB and add 60 ms early reflection; the player's
own climb becomes the scariest sound in the level. The handrail's knock. Wind pulsing
at the slit like slow breathing. From above, nearer per step: the zinc sheet.

**Feel.** Trapped, both directions. Six visible treads at a time, no exits, no space.
The design goal: the player pauses at the landing light, and does not want to leave it.

---

## 8. The Unfinished Second Floor

Bigger and stranger than before: the second floor was poured over the south and east
wings, its rooms traced in block walls one to two meters high — **a roofless maze at
night**, walls exactly tall enough to hide a crouching person and exactly low enough
to see over when standing. The player's eye does the scary work: every wall could
have something behind it; none does.

**Design.**
- Slab underfoot: bare concrete, formwork ridges, drifted sand in wind shadows,
  puddle-stains from the last rains.
- Block-wall maze: door-shaped gaps where doors were never hung; a window opening at
  standing height framing the courtyard void; rebar reeds rising from every corner
  column, wind-humming faintly at the top of their range.
- **The courtyard void**: the open hole down into the courtyard, edged by two block
  courses (60 cm). From up here the player looks down into the moonlit court they stood
  in — and sees how visible they were from above the whole time.
- Construction leftovers: a pallet of cement bags hardened to stone, a sand pile
  crusted over, a mortar pan with set mortar, a shovel handle without a head, stacked
  hollow blocks, **loose wires** run through the block cells and dangling from a dead
  junction box, a coil of rusted tie-wire, a rusted rebar bender.
- Habitation echo: in one traced room — the one that would have been a bedroom — an old
  mattress on blocks, a car seat, cigarette butts, a wire hook holding a rag that was
  a shirt. Someone used the shell as a hideout years ago; dust says nobody since.
- Broken windows: the east traced-wall carries two actual window frames that were
  installed before the money ran out — both broken, glass teeth catching moonlight,
  the only glass on the whole floor.

**Lighting.** Full moon over everything, hard block shadows striping the slab into
dark/light lanes; the maze reads as bands of visibility. No artificial light. The
locked room is not visible from here — but its corner of roof is, past the three
steps down, as a *darker* dark.

**Sound.** Wind returns at full strength, whistling in block cells (a real, eerie,
multi-tone effect), the rebar hum, tie-wire coil ringing faintly when gusts roll it a
centimeter. First time the player hears the zinc sheet at true close range: it's on
the roof section, one level of parapet away, lifting and slapping.

**Feel.** Exposed after the shaft's compression — relief that curdles. The maze walls
mean the player is never sure the floor is as empty as it looks, and the void edge
adds real, honest physical danger (the level's only genuine hazard, unfenced).

---

## 9. The Rooftop

Three steps down from the unfinished slab onto the roof over the north/west wing:
the classic Libyan roof, gone to seed — and the approach lane to the locked room.

**Design & props.**
- Black poly water tank on a rusted stand, ladder welded to the side; float-valve pipe
  running to a drilled hole. The tank still holds some water: it **sloshes**, deep and
  soft, in strong gusts — the roof's most animal sound.
- Satellite dish knocked off-aim, LNB cable snaking to a drilled hole; a fallen TV
  antenna leaning in the parapet corner.
- Clothesline between two posts: two wooden pins, one petrified rag that flags in the
  wind — the roof's one large movement, visible in silhouette from across the slab.
- Old foam mattress folded against the parapet (summer sleeping, once), a broken white
  plastic chair on its side, a salvaged car seat, concrete blocks in a loose stack,
  the **zinc sheet** lying over the old hatch hole — found at last, lifting at one
  corner in gusts. Every sound the player chased is now visible and explained. The
  house has kept every promise… which is exactly when it stops making noise:
- **The wind-shadow.** The locked room's corner is shielded by the parapet and tank.
  Three steps before its door, the wind dies mid-stride and the rag stops flagging in
  peripheral vision. The quietest outdoor spot in the game is the doorstep of the
  locked room.
- **The view.** Over the parapet: the whole neighborhood the player walked — home's
  warm lamp findable, the shop's cool glow, the school's black mass, the minaret's
  green point, the far city smear, weak and distant. Isolation by scale: everything
  safe is visible and none of it is reachable without going back through the house.

**Lighting.** Brightest exterior in the level: open moon, clean parapet shadow-line,
tank shadow like a sundial. The locked room's wall sits in the one wedge that moonlight
never enters at any hour — approach means walking from the brightest into the darkest
outdoor pixel on the map.

**Sound.** Full open wind (loudest ambience in the level) → tank slosh → clothesline
rag → zinc sheet, all spatialized and honest → then the wind-shadow's abrupt quiet at
the corner. Distant dogs below. The player's own steps go slab-flat and dry.

**Feel.** False summit. Openness, orientation, the whole world visible — the level lets
the player believe they've reached the end and survived it. Then they notice the little
room in the corner has a door. And a lock.

---

## 10. The Locked Room — غرفة السطح (maximum effort)

### 10.1 Arrival ritual

The door must be *earned* as an image before it opens:
1. Seen from the stairs' top as a darker block in the dark (unreadable).
2. Seen from the roof as a small rendered box, one tiny **cracked window** shuttered
   from inside, high and dark.
3. Approached through the wind-shadow — sound falls away step by step.
4. Read up close: a **heavy old metal door**, over-painted then rusted, its hasp
   industrial, its padlock **newer than everything around it**. Someone kept renewing
   this lock long after the house emptied. Around the handle, paint rubbed to bright
   metal in one hand-sized patch. Low on the door's face and jamb: old **scratch
   marks** — worn, grime-varnished, wrong side (they're on the *outside* here only
   as faint over-paint ghosts; the deep ones wait inside).

### 10.2 The room (2.5 × 3 m, 2.4 m ceiling)

Opening the door releases years of still air: dust motes hang in the player's beam
like weather. The room reads in one sweep — small, bare, bedroom-shaped — and then
re-reads, detail by detail, into the worst room in the game.

**Contents, composed:**
- **The bed:** narrow steel frame, thin mattress, bedding rotted to a grey compressed
  layer — but *made*, roughly, blanket squared, pillow centered with a stain-shadow
  at its middle. Made beds are for rooms people expect to return to.
- **The door's inner face:** deep scratch marks, concentrated at handle height and
  again low near the floor, worn smooth and dark with age. Above the handle: the same
  bright rubbed patch as outside, mirrored. No fresh marks. Everything here finished
  happening a long time ago.
- **Small cabinet:** two doors, one warped open a finger's width. Inside: folded
  child-size clothes stiff as cardboard, a tin cup, an empty medicine strip, a comb
  with missing teeth.
- **Broken mirror:** the frame still hangs level; most of the glass lies below it
  against the skirting in three long blades, dust-furred. One shard remains in the
  frame's corner. Behind where the glass was: a sharp pale rectangle of never-faded
  paint — the room's original color, shockingly clean, like a wound that never
  weathered. The eye returns to it constantly.
- **The box:** under the bed, a small wooden box **tied shut with a strip of black
  cloth**, the knot old and fused. Dust on the lid shows one arc — slid out, slid
  back, long ago. It is never opened.
- **Torn family photos:** a scatter in and near the box's arc — 80s color and older
  black-and-white scraps. Backgrounds survive: this courtyard, a beach, a doorway.
  The tears pass through where the people were. A few scraps have drifted to the
  dark corner.
- **The cassette:** on the cabinet's top, alone: a **portable cassette recorder**,
  corroded, its window dusty — a tape inside, spooled mid-reel. One more cassette
  beside it, unlabeled except a hand-drawn symbol (a small house). Whether either
  ever plays is outside this document; as objects they are the room's loudest
  silence — recorded sound, present, unheard.
- **The drawings:** low on one wall, crayon and pencil on the paint: a dense, happy
  1 m band at child height — house, sun, stick figures, a cat — thinning along the
  wall's length, figures simplifying, colors dropping to one, until the final meter
  is only **repeated vertical strokes**, hundreds, in patient rows. The single most
  disturbing surface in the game, made of nothing but marks.
- **The cracked window:** tiny, high, shuttered from inside, the shutter nailed —
  but its glass behind the shutter is cracked, and at its warped edge two hairline
  moonlines enter and lie across the far wall. When gusts strain the shutter it
  creaks once, wood on nail; the moonlines tremble.
- **The plate:** near the door, on the floor: an enamel plate and spoon, empty, dusty.
  On the wall above, a nail with a ring of faded ribbon.
- **The stains:** restrained and ambiguous — a water-damage bloom on the ceiling
  corner, a darker hand-height smudge-line along one wall (the polish of repeated
  touch, not a substance), and the mattress's old tide-lines. Nothing red. Nothing
  explained.
- **The dark corner:** the corner opposite the window is built to defeat light:
  two matte-black-painted... no — honestly: it's the corner shadowed by both the
  cabinet and the bed, its plaster darkened by an old soot bloom, and the photo
  scraps drifted there. Player light grazes it obliquely from every stance the
  furniture allows; it never quite fills. Something is in it: nothing. It must
  contain nothing, every time, forever.

### 10.3 Lighting

Two hairline moonlines and the player's beam. That's all. Dust in every beam. The
mirror blades on the floor catch the player's light and throw broken streaks across
the ceiling when crossed — the room's only "event," fully physical, never scripted.
The pale mirror-rectangle on the wall reads brighter than everything, like a screen
with nothing on it.

### 10.4 Sound

Active silence: the roof wind fully muffled to a felt pressure; the player's own
presence (cloth, breath-space, floor grit) pushed forward in the mix. One sound
exists: the shutter's single wooden creak on hard gusts, followed by a silence that
feels *longer* than before it. If the player stands still for a long time, add
nothing. The discipline is the horror.

### 10.5 Why it will be remembered

- It's the only room the house *locks* — the architecture's one explicit statement.
- It hides nothing and explains nothing: full visibility, zero answers.
- Every element is domestic: bed, cabinet, mirror, drawings, a plate. The horror is
  arrangement, not content — it reconstructs itself in the player's head on the walk
  home, which is where the game actually happens.
- It is small. After a level that kept growing, the final room is the smallest
  space in the house — and the biggest.

---

## 11. Safety Gradient Map

Where the player should feel safe → unsafe (design targets, no mechanics):

| Zone | Feel | Why |
|---|---|---|
| Alley approach | tension | last streetlight behind, dead end ahead |
| Courtyard gate / threshold | last safety | exterior light still reachable in one step |
| Sqifa | committed | light behind gone at first turn |
| **Courtyard** | ambiguous refuge | moonlight + open sky, but watched by every opening |
| Living room | uneasy calm | social shape, empty; dark corner |
| Bedroom | trespass | intimacy + the contradictory bed |
| Kitchen | disgust-unease | insects, stickiness, the ajar cabinet |
| Bathroom | pressure spike | smallest space, door won't close, mirror fragments |
| Storage | dread by choice | windowless, closed door, self-inflicted entry |
| Back room | model-breaking | "recent" evidence + sounds from directly above |
| Stairway | trapped | enclosure, echo, one cold light |
| Unfinished floor | exposed | maze walls, void edge, moon lanes |
| Rooftop | false summit | openness, view, every sound explained |
| Wind-shadow corner | threshold dread | world's sound removed three steps early |
| **Locked room** | peak | everything above, concentrated and silent |

The gradient is a loop with two relief valves (courtyard, rooftop) so fear can be
re-armed instead of saturating. The final relief (rooftop) is the trap for the peak.

---

## 12. Environmental Storytelling (suggestion only — never explained)

The house carries **three time layers**, readable in materials, never in text:

1. **The family (years ago):** furniture, clothes on hangers, growth chart, wedding
   chest, photos, drawings that start happy. Everything of value was *taken* — good
   dishes gone, one photo removed from its frame — an orderly, chosen departure…
   except the locked room's contents were left, and locked in.
2. **The trespassers (since):** cigarette butts, cans, tag over whitewashed tag,
   broken padlocks below a re-locked hasp, the mattress hideout upstairs, the milk
   crate under the window. Their traces stop at the roof: no butts, no tags past the
   wind-shadow. Even they didn't go to the corner.
3. **The recent unknown (recently, thinly):** the back room's squared mattress and
   un-yellowed water bottle, the thin dust there, the cleaner path in the courtyard
   tiles toward it. Smallest layer, biggest implication, zero explanation.

Rules: no readable documents, no names anywhere (scratched-out or absent), no dates
except the growth chart's faint ones, nothing that resolves layer 3 against layers 1–2.
The player should be able to build three different theories and the house confirms none.

---

## 13. Making the House Scarier at Night — Without Monsters

All techniques are physical, cheap, and deniable:

1. **Own-shadow approach** (ext.): the alley lamp behind casts the player's shadow
   onto the facade, growing as they approach.
2. **Courtyard reveal**: architecture as jump-scare replacement — dark 7 m corridor
   into open moonlit sky, indoors.
3. **Down-looking openings** (see courtyard, Section 5): black second-floor holes ring
   the courtyard light; the player is always in their sightline and knows it.
4. **Sound-source debt**: zinc sheet / drip / shutter heard rooms before being found;
   the house always pays the debt (source findable), so silence stays trustworthy —
   and therefore terrifying when deployed.
5. **The wind-shadow**: subtracting ambience at the locked-room corner instead of
   adding stingers. Removal scares more than addition.
6. **Contradiction props**: made bed under years of dust; disturbed sheets with
   dust in the folds; swept floor in the deepest room; plate by a locked door.
7. **Wrong-facing objects**: chair toward the corner, TV toward the doorway,
   mirror cracked at exactly face height.
8. **The permanently empty dark corner** — one per key room (living room NE corner, locked-room
   corner). Never occupied. The player's imagination staffs it.
9. **Self-noise amplification**: stairwell and bathroom mix the player's own sounds
   louder than the world's; the scariest audio actor is the player.
10. **Micro-motion with alibis**: curtain (shared air path), vine shadow (wind),
    moonline tremble (shutter strain), rag on the line. Every motion has a defense
    in physics — barely.
11. **Light discipline**: never lift blacks indoors; the player's beam warm against
    cold moon; no colored light anywhere in the house.
12. **Real hazard, honestly presented**: the courtyard void edge and the spalled
    stair tread — physical danger the player can see and must respect. Grounded fear
    calibrates the ungrounded kind.

---

## 14. Build Notes (Unity / Blender, low budget)

- **Modularity:** the whole house builds from the neighborhood kit plus ~10 new
  modules: courtyard wall ring, pergola pipes, sqifa corner pieces, block-maze wall
  (1 m and 2 m), void edge course, roof-room shell, stair flight + landing, swollen
  bathroom doorframe, tank stand. Everything else is prop placement and decals.
- **Decal-first detailing:** cracks, paint curls, tide rings, soot fans, growth chart,
  drawings (one texture sheet: dense→sparse→vertical strokes), scratch sets (door
  inside/outside), photo ghosts, tag + whitewash. ~2 texture atlases cover the level.
- **Dust as a system:** one tiling dust-floor material with a vertex-paint "worn path"
  channel — paths tell the traffic story in every room (heavy to living room, thin to
  back room, none past the wind-shadow).
- **Light plan:** zero artificial lights inside the house. One baked moon key +
  courtyard/stair-slit/window-blade shafts (baked); the player light real-time. The
  cheapest possible lighting rig, on purpose.
- **Audio zones:** exterior / sqifa / courtyard well / rooms / shaft / unfinished /
  roof / wind-shadow / locked room — nine reverb-ambience volumes; every scare in this
  document is an ambience crossfade, not an event system.
- **Scene split:** house interior may be an additive scene; the sqifa's second turn is
  a natural load boundary that never shows a seam.
- **Blockout order:** shell + courtyard void → sqifa → stairs → room volumes → roof +
  locked room → prop pass (contradiction props last, placed by hand, one per room
  maximum — restraint is the aesthetic) → decal pass → audio volumes → moon bake.
