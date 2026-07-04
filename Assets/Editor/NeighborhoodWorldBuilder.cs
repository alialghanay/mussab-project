// Greybox world builder for the Libyan neighborhood horror environment.
// Neighborhood layout follows Docs/WorldDesign.md; the abandoned house follows
// the expanded courtyard-house design in Docs/AbandonedHouseDesign.md.
// Runs automatically once (if Assets/Scenes/Neighborhood.unity does not exist)
// or on demand via Tools > Neighborhood > Build World.
// NOTE: When creating a new scene (e.g., MainMenu), add it to
// ProjectSettings/EditorBuildSettings.asset via File > Build Settings
// so SceneLoader can load it at runtime.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class NeighborhoodWorldBuilder
{
    const string ScenePath = "Assets/Scenes/Neighborhood.unity";
    const string MatFolder = "Assets/Art/Materials/Greybox";

    // ---------------------------------------------------------------- entry

    [InitializeOnLoadMethod]
    static void AutoSetupOnce()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode) return;

            // no scene yet: build the whole world once
            if (!System.IO.File.Exists(ScenePath))
            {
                if (SessionState.GetBool("NeighborhoodBuilt", false)) return;
                SessionState.SetBool("NeighborhoodBuilt", true);
                Build();
                return;
            }

            // scene exists: self-heal a missing player rig in the open Neighborhood scene
            if (SessionState.GetBool("NeighborhoodPlayerChecked", false)) return;
            SessionState.SetBool("NeighborhoodPlayerChecked", true);
            if (EditorSceneManager.GetActiveScene().path != ScenePath) return;
            if (Object.FindFirstObjectByType<PlayerController>() != null) return;
            AddPlayerToScene();
        };
    }

    [MenuItem("Tools/Neighborhood/Add Player To Current Scene")]
    public static void AddPlayerToScene()
    {
        if (Object.FindFirstObjectByType<PlayerController>() != null)
        {
            Debug.Log("[NeighborhoodWorldBuilder] Player rig is already in the scene.");
            return;
        }
        // remove the old static camera and marker so there is exactly one camera + listener
        foreach (var name in new[] { "Main Camera", "PlayerStart", "Player" })
        {
            var stale = GameObject.Find(name);
            if (stale != null && stale.transform.parent == null)
                Object.DestroyImmediate(stale);
        }
        CreatePlayerRig();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("[NeighborhoodWorldBuilder] Player rig added to the scene and saved. Press Play to walk the world.");
    }

    [MenuItem("Tools/Neighborhood/Build World (overwrites Neighborhood scene)")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        EnsureFolders();

        BuildGroundAndRoads();
        BuildAlleyHouses();
        BuildFamilyHouse();
        BuildRoadHouses();
        BuildShop();
        BuildSchool();
        PlayerController playerController = BuildPlayerStart();
        PhoneController phone = playerController != null ? playerController.phone : null;
        BuildAbandonedHouse(phone);
        BuildStreetLighting();
        BuildGlobalLightingAndSky();

        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        Debug.Log("[NeighborhoodWorldBuilder] Greybox world built and saved to " + ScenePath);
    }

    // ------------------------------------------------------------- coordinates
    // X axis = east(+)/west(-), Z axis = north(+)/south(-). Units are meters.
    // Main road: east-west strip at Z 0, width 8, from X -100 to +60.
    // Side street / main alley: runs south at X 40, width 4.5, Z -4 .. -75.
    // Dead-end branch: Z -72, width 2.6, X 2 .. 38. Abandoned house at its west end.
    // Shop: south side of road at X -80. School: north side, X -60 .. -10.
    // Family house: west side of alley at Z -45.

    // ---------------------------------------------------------------- ground

    static void BuildGroundAndRoads()
    {
        var root = Group("_GroundAndRoads");

        Box("Ground_Dust", root, new Vector3(-20, -0.06f, -10), new Vector3(320, 0.1f, 320), MatDust());
        Box("Road_Main_Asphalt", root, new Vector3(-20, 0.005f, 0), new Vector3(160, 0.1f, 8), MatAsphalt());
        Box("Street_Alley", root, new Vector3(40, 0.005f, -39.5f), new Vector3(4.5f, 0.1f, 71), MatAsphaltOld());
        Box("Street_DeadEndBranch", root, new Vector3(20, 0.003f, -72), new Vector3(36, 0.1f, 2.6f), MatDust());
        Box("Road_SpeedBump", root, new Vector3(-35, 0.06f, 0), new Vector3(0.6f, 0.14f, 8), MatConcrete());

        // rubble corner lot before the shop (blocks + sand pile)
        var lot = Group("RubbleLot", root, new Vector3(-68, 0, -8));
        Box("BlockStack_A", lot, new Vector3(0, 0.4f, 0), new Vector3(2f, 0.8f, 1f), MatBlock());
        Box("BlockStack_B", lot, new Vector3(1.2f, 0.25f, 1.4f), new Vector3(1f, 0.5f, 1f), MatBlock());
        Sphere("SandPile", lot, new Vector3(-2.5f, 0.0f, 1f), new Vector3(4, 1.2f, 4), MatDust());
    }

    // ------------------------------------------------------------ alley houses

    static void BuildAlleyHouses()
    {
        var root = Group("_AlleyHouses");

        // West side of the alley (wall line X ~ 37.75), family house slot left empty at Z -45.
        float westX = 33.5f; // house centers (8 m deep, front face at 37.5)
        int i = 0;
        for (float z = -10; z >= -68; z -= 10.5f)
        {
            if (z < -40 && z > -52) { i++; continue; } // family house slot
            House(root, "AlleyHouseW_" + i, new Vector3(westX, 0, z), 8, 9.5f, i);
            i++;
        }

        // East side (front face at 42.25)
        for (float z = -10; z >= -68; z -= 10.5f)
        {
            House(root, "AlleyHouseE_" + i, new Vector3(46.5f, 0, z), 8, 9.5f, i);
            i++;
        }

        // Houses flanking the dead-end branch (its walls close in)
        House(root, "BranchHouseN_" + i, new Vector3(20, 0, -66.5f), 24, 7.5f, i); i++;
        House(root, "BranchHouseS_" + i, new Vector3(20, 0, -77.5f), 24, 7.5f, i); i++;

        // Parked cars in the alley
        Car(root, "Car_AlleySedan", new Vector3(41.8f, 0, -22), 90, MatCarBlue());
        Car(root, "Car_AlleyPickup", new Vector3(38.2f, 0, -58), 270, MatCarWhite());

        // trash corner near the bend
        var trash = Group("TrashCorner", root, new Vector3(38.2f, 0, -66));
        Box("TrashBag_A", trash, new Vector3(0, 0.25f, 0), new Vector3(0.6f, 0.5f, 0.6f), MatTank());
        Box("TrashBag_B", trash, new Vector3(0.5f, 0.2f, 0.2f), new Vector3(0.5f, 0.4f, 0.5f), MatTank());
        Box("Cardboard", trash, new Vector3(0.2f, 0.05f, 0.7f), new Vector3(1f, 0.1f, 0.7f), MatCardboard());
    }

    // ------------------------------------------------------------ family house

    static void BuildFamilyHouse()
    {
        var root = Group("_FamilyHouse", null, new Vector3(33.5f, 0, -45));

        Box("Body", root, new Vector3(0, 1.75f, 0), new Vector3(8, 3.5f, 10), MatRenderCream());
        Parapet(root, new Vector3(0, 3.5f, 0), 8, 10);

        // metal front door (dark green) facing the alley (east face at x +4)
        Box("Door_Metal", root, new Vector3(4.03f, 1.05f, -1.2f), new Vector3(0.08f, 2.1f, 1.3f), MatDoorGreen());
        Box("Doorstep", root, new Vector3(4.35f, 0.08f, -1.2f), new Vector3(0.7f, 0.16f, 1.8f), MatConcrete());

        // barred windows (dark inset + warm glow on one)
        Box("Window_Dark", root, new Vector3(4.03f, 1.9f, 1.6f), new Vector3(0.06f, 1.1f, 1.2f), MatWindowDark());
        Box("Window_Lit", root, new Vector3(4.03f, 1.9f, 3.4f), new Vector3(0.06f, 1.1f, 1.2f), MatWindowWarm());
        PointLight(root, "WindowGlow", new Vector3(4.5f, 1.9f, 3.4f), WarmBulb, 1.2f, 4f);

        // entrance lamp above the door - the warmest, steadiest light on the map
        Box("EntranceLampFixture", root, new Vector3(4.1f, 2.5f, -1.2f), new Vector3(0.15f, 0.2f, 0.3f), MatWindowWarm());
        PointLight(root, "EntranceLamp", new Vector3(4.4f, 2.45f, -1.2f), WarmBulb, 2.6f, 9f);

        // roof life: tank, dish, clothesline posts
        Tank(root, new Vector3(-2, 3.5f, -3));
        Dish(root, new Vector3(2.5f, 3.6f, 3.5f), false);
        Box("LinePost_A", root, new Vector3(-3, 4.1f, 2), new Vector3(0.08f, 1.2f, 0.08f), MatMetal());
        Box("LinePost_B", root, new Vector3(3, 4.1f, 2), new Vector3(0.08f, 1.2f, 0.08f), MatMetal());

        // potted plants along the base
        Box("PlantTin_A", root, new Vector3(4.3f, 0.2f, 0.4f), new Vector3(0.35f, 0.4f, 0.35f), MatMetal());
        Sphere("Plant_A", root, new Vector3(4.3f, 0.55f, 0.4f), new Vector3(0.5f, 0.4f, 0.5f), MatPlant());
        Box("PlantTin_B", root, new Vector3(4.3f, 0.2f, 2.4f), new Vector3(0.35f, 0.4f, 0.35f), MatMetal());
        Sphere("Plant_B", root, new Vector3(4.3f, 0.55f, 2.4f), new Vector3(0.5f, 0.4f, 0.5f), MatPlant());

        // AC unit with drip stain position
        Box("AC_Unit", root, new Vector3(4.15f, 2.7f, 1.6f), new Vector3(0.35f, 0.5f, 0.9f), MatMetal());
    }

    // ------------------------------------------------------------- road houses

    static void BuildRoadHouses()
    {
        var root = Group("_RoadHouses");

        // south side of the main road, between shop (X -80) and alley (X 40)
        int i = 20;
        for (float x = -62; x <= 26; x += 13.5f)
        {
            House(root, "RoadHouseS_" + i, new Vector3(x, 0, -10.5f), 12, 9, i);
            // courtyard wall filling the gap to the street
            Box("CourtyardWall_" + i, root, new Vector3(x + 6.4f, 1.2f, -6.2f), new Vector3(1.2f, 2.4f, 5.5f), MatRenderOchre());
            i++;
        }

        // far side of the road, east of the school: closed courtyard walls only
        Box("LongCourtyardWall_E", root, new Vector3(20, 1.25f, 5.2f), new Vector3(56, 2.5f, 0.4f), MatRenderGreen());
        Box("CourtyardGate_E", root, new Vector3(8, 1.1f, 5.2f), new Vector3(2.6f, 2.2f, 0.45f), MatDoorBlue());

        // parked cars on the road
        Car(root, "Car_RoadTarp", new Vector3(-30, 0, -3f), 0, MatTarp());
        Car(root, "Car_RoadSedan", new Vector3(-2, 0, 3f), 180, MatCarWhite());

        // dumpster corner
        Box("Dumpster", root, new Vector3(-44, 0.7f, 4.6f), new Vector3(2.2f, 1.4f, 1.2f), MatDoorGreen());
    }

    // -------------------------------------------------------------------- shop

    static void BuildShop()
    {
        // corner shop, south side of the road at X -80. Front (opening) faces north.
        var root = Group("_Shop", null, new Vector3(-80, 0, -8));

        Box("Floor", root, new Vector3(0, 0.05f, 0), new Vector3(6, 0.1f, 5), MatTileFloor());
        Box("Wall_Back", root, new Vector3(0, 1.6f, -2.4f), new Vector3(6, 3.2f, 0.2f), MatRenderCream());
        Box("Wall_W", root, new Vector3(-2.9f, 1.6f, 0), new Vector3(0.2f, 3.2f, 5), MatRenderCream());
        Box("Wall_E", root, new Vector3(2.9f, 1.6f, 0), new Vector3(0.2f, 3.2f, 5), MatRenderCream());
        Box("Roof", root, new Vector3(0, 3.25f, 0), new Vector3(6.4f, 0.15f, 5.4f), MatConcrete());
        // front lintel above the opening + rolled-up shutter box
        Box("Lintel", root, new Vector3(0, 2.9f, 2.45f), new Vector3(6, 0.7f, 0.2f), MatRenderCream());
        Box("ShutterRoll", root, new Vector3(0, 2.45f, 2.5f), new Vector3(4.6f, 0.3f, 0.3f), MatMetal());
        // sun-bleached sign
        Box("Sign", root, new Vector3(0, 3.45f, 2.55f), new Vector3(5, 0.8f, 0.1f), MatSign());

        // interior: counter, shelves, fridges, bread table
        Box("Counter", root, new Vector3(1.7f, 0.55f, -1.3f), new Vector3(2.2f, 1.1f, 0.7f), MatCounter());
        Box("Shelf_Back", root, new Vector3(-0.6f, 1.5f, -2.2f), new Vector3(4.4f, 2.8f, 0.35f), MatShelf());
        Box("Shelf_W", root, new Vector3(-2.65f, 1.5f, 0.2f), new Vector3(0.35f, 2.8f, 4.2f), MatShelf());
        Box("Fridge_Drinks", root, new Vector3(2.55f, 1.0f, 0.6f), new Vector3(0.65f, 2.0f, 0.8f), MatFridge());
        Box("Freezer_Chest", root, new Vector3(2.45f, 0.45f, 1.7f), new Vector3(0.8f, 0.9f, 1.1f), MatFridge());
        Box("BreadTable", root, new Vector3(-1.8f, 0.4f, 1.8f), new Vector3(1.2f, 0.8f, 0.8f), MatCounter());
        Box("BreadBags", root, new Vector3(-1.8f, 1.0f, 1.8f), new Vector3(1.0f, 0.4f, 0.6f), MatBread());
        Box("BottleCrates", root, new Vector3(-3.4f, 0.35f, 3.2f), new Vector3(0.8f, 0.7f, 0.8f), MatCrate());
        Box("PlasticChair", root, new Vector3(3.6f, 0.45f, 3.4f), new Vector3(0.5f, 0.9f, 0.5f), MatPlasticChair());

        // the only cool-white light in the neighborhood exterior
        Box("FluorescentTube", root, new Vector3(0, 3.1f, 0.5f), new Vector3(1.4f, 0.06f, 0.12f), MatFluorescent());
        PointLight(root, "ShopLight", new Vector3(0, 2.8f, 0.5f), CoolFluor, 3.2f, 9f);
        PointLight(root, "ShopSpill", new Vector3(0, 2.4f, 3.6f), CoolFluor, 1.8f, 8f);
    }

    // ------------------------------------------------------------------ school

    static void BuildSchool()
    {
        var root = Group("_School");

        // walled yard X -60..-10, Z 6..46; two-story block along the north side
        Box("Building", root, new Vector3(-35, 3.5f, 41), new Vector3(44, 7, 10), MatSchool());
        Parapet(root, new Vector3(-35, 7f, 41), 44, 10);

        // window strips (dark inset rows, two floors) on the south face
        for (int f = 0; f < 2; f++)
            for (int wx = 0; wx < 9; wx++)
                Box("Win_" + f + "_" + wx, root,
                    new Vector3(-53 + wx * 4.5f, 1.9f + f * 3.4f, 35.95f),
                    new Vector3(1.6f, 1.3f, 0.06f), MatWindowDark());

        // yard walls with the gate gap in the south wall
        Box("YardWall_S_W", root, new Vector3(-49.5f, 1.1f, 6), new Vector3(21, 2.2f, 0.35f), MatSchoolWall());
        Box("YardWall_S_E", root, new Vector3(-21.5f, 1.1f, 6), new Vector3(23, 2.2f, 0.35f), MatSchoolWall());
        Box("Gate_Metal", root, new Vector3(-36.5f, 1.05f, 6), new Vector3(4, 2.1f, 0.3f), MatDoorBlue());
        Box("YardWall_W", root, new Vector3(-60, 1.1f, 26), new Vector3(0.35f, 2.2f, 40), MatSchoolWall());
        Box("YardWall_E", root, new Vector3(-10, 1.1f, 26), new Vector3(0.35f, 2.2f, 40), MatSchoolWall());

        // yard props: flagpole, goal frame, guard kiosk
        Cylinder("Flagpole", root, new Vector3(-35, 4, 12), 0.08f, 8, MatMetal());
        var goal = Group("GoalFrame", root, new Vector3(-22, 0, 24));
        Box("Post_A", goal, new Vector3(-1.5f, 1, 0), new Vector3(0.1f, 2, 0.1f), MatMetal());
        Box("Post_B", goal, new Vector3(1.5f, 1, 0), new Vector3(0.1f, 2, 0.1f), MatMetal());
        Box("Bar", goal, new Vector3(0, 2, 0), new Vector3(3.1f, 0.1f, 0.1f), MatMetal());
        Box("GuardKiosk", root, new Vector3(-40.5f, 1.2f, 8.4f), new Vector3(2, 2.4f, 2), MatRenderOchre());

        // palms along the wall + one big ficus
        Palm(root, new Vector3(-56, 0, 10));
        Palm(root, new Vector3(-14, 0, 12));
        Palm(root, new Vector3(-30, 0, 34));
        var ficus = Group("Ficus", root, new Vector3(-46, 0, 30));
        Cylinder("Trunk", ficus, new Vector3(0, 1.5f, 0), 0.25f, 3, MatTrunk());
        Sphere("Crown", ficus, new Vector3(0, 4.2f, 0), new Vector3(6, 4, 6), MatCrown());

        // roof tanks, one leaning
        Tank(root, new Vector3(-45, 7f, 41));
        var lean = Tank(root, new Vector3(-26, 7f, 43));
        lean.transform.rotation = Quaternion.Euler(0, 0, 12);
    }

    // --------------------------------------------------------- abandoned house
    // Courtyard house (حوش عربي) per Docs/AbandonedHouseDesign.md.
    // Local frame: root at world (-6, 0, -69.9). +X = east (toward the alley),
    // +Z = north. Footprint X -7..+7, Z -8..+8. Front door on the east facade,
    // aligned with the dead-end branch street at world Z -72.
    // Heights: LOW wing (back room/living/storage/lobby) walls 2.9, roof top 3.1.
    //          HIGH wing (bedroom/hallway/sqifa/kitchen/bath/stairs) walls 3.5,
    //          second-floor slab top 3.7. Three steps connect 3.7 -> 3.1.

    const float LOW = 2.9f, HIGH = 3.5f, LOWTOP = 3.1f, HIGHTOP = 3.7f;

    static void BuildAbandonedHouse(PhoneController phone)
    {
        var root = Group("_AbandonedHouse", null, new Vector3(-5, 0, -69.9f));
        AH_Forecourt(root);
        AH_FloorsAndMasses(root);
        AH_GroundWalls(root);
        PickupInteractable ballPickup = AH_Courtyard(root);
        AH_RoomProps(root);
        AH_Stairs(root);
        AH_UpperLevel(root);
        AH_LockedRoom(root);

        // Prototype encounter setup
        var encounterGO = new GameObject("_EncounterDirector");
        encounterGO.transform.SetParent(root.transform, false);
        var director = encounterGO.AddComponent<GameDirector>();
        director.phone = phone;
        director.targetPickup = ballPickup;

        var encounter = encounterGO.AddComponent<EncounterTrigger>();
        director.encounter = encounter;

        var spawnPoint = new GameObject("WomanSpawnPoint");
        spawnPoint.transform.SetParent(encounterGO.transform, false);
        spawnPoint.transform.localPosition = new Vector3(7.5f, 0f, -2.5f);
        encounter.spawnPoint = spawnPoint.transform;

        var womanPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        womanPrefab.name = "WomanInBlack_Placeholder";
        Object.DestroyImmediate(womanPrefab.GetComponent<CapsuleCollider>());
        var womanRenderer = womanPrefab.GetComponent<Renderer>();
        if (womanRenderer != null)
            womanRenderer.sharedMaterial = MatVoid();
        womanPrefab.SetActive(false);
        encounter.womanInBlackPrefab = womanPrefab;

        // audio zone: exterior vs interior of the abandoned house
        var audioZoneGO = new GameObject("AudioZone_AbandonedHouseInterior");
        audioZoneGO.transform.SetParent(root.transform, false);
        audioZoneGO.transform.localPosition = new Vector3(0f, 0f, 0f);
        var zoneCollider = audioZoneGO.AddComponent<BoxCollider>();
        zoneCollider.isTrigger = true;
        zoneCollider.size = new Vector3(16f, 6f, 18f);
        var zone = audioZoneGO.AddComponent<AudioZone>();
        var zoneSource = audioZoneGO.AddComponent<AudioSource>();
        zoneSource.loop = true;
        zoneSource.playOnAwake = false;
        zone.ambienceSource = zoneSource;
    }

    static GameObject WallEW(string name, GameObject parent, float x1, float x2, float z, float yBase, float h, Material m)
        => Box(name, parent, new Vector3((x1 + x2) / 2f, yBase + h / 2f, z), new Vector3(Mathf.Abs(x2 - x1), h, 0.25f), m);

    static GameObject WallNS(string name, GameObject parent, float x, float z1, float z2, float yBase, float h, Material m)
        => Box(name, parent, new Vector3(x, yBase + h / 2f, (z1 + z2) / 2f), new Vector3(0.25f, h, Mathf.Abs(z2 - z1)), m);

    static void AH_Forecourt(GameObject root)
    {
        var fc = Group("Forecourt", root, Vector3.zero);
        WallEW("CourtWall_N", fc, 7, 9.2f, 0.6f, 0, 2.2f, MatRuinRender());
        WallEW("CourtWall_S", fc, 7, 9.2f, -4.2f, 0, 2.2f, MatRuinRender());
        WallNS("CourtWall_E_N", fc, 9.2f, -1.5f, 0.6f, 0, 2.2f, MatRuinRender());
        WallNS("CourtWall_E_S", fc, 9.2f, -4.2f, -2.7f, 0, 2.2f, MatRuinRender());
        var gate = Box("Gate_HalfOpen", fc, new Vector3(9.25f, 1.0f, -1.9f), new Vector3(0.08f, 2.0f, 1.15f), MatRust());
        gate.transform.rotation = Quaternion.Euler(0, 35, 0);
        FixRotatedDoorCollider(gate);

        var olive = Group("DeadOlive", fc, new Vector3(8.3f, 0, -3.4f));
        Cylinder("Trunk", olive, new Vector3(0, 0.9f, 0), 0.12f, 1.8f, MatTrunk());
        Sphere("BareCrown", olive, new Vector3(0, 2.1f, 0), new Vector3(1.6f, 1.2f, 1.6f), MatDeadCrown());
        Box("TilePile", fc, new Vector3(7.6f, 0.15f, -3.6f), new Vector3(1.2f, 0.3f, 0.9f), MatRuinRender());
        Cylinder("FireSpot", fc, new Vector3(8.6f, 0.01f, 0f), 0.4f, 0.02f, MatSoot());
        Box("TrashBag_A", fc, new Vector3(7.5f, 0.22f, -0.6f), new Vector3(0.5f, 0.45f, 0.5f), MatTank());
        Box("MilkCrate_UnderWindow", fc, new Vector3(7.4f, 0.18f, 1.0f), new Vector3(0.4f, 0.35f, 0.4f), MatCrate());
        Box("BrokenLock_A", fc, new Vector3(7.35f, 0.03f, -1.4f), new Vector3(0.08f, 0.06f, 0.05f), MatRust());
        Box("BrokenLock_B", fc, new Vector3(7.5f, 0.03f, -1.7f), new Vector3(0.08f, 0.06f, 0.05f), MatRust());

        // facade dressing (facade wall itself is built in AH_GroundWalls)
        Box("MeterBox_Dead", fc, new Vector3(7.08f, 1.7f, -1.0f), new Vector3(0.14f, 0.4f, 0.3f), MatMetal());
        Box("NumberTile_Ghost", fc, new Vector3(7.14f, 1.6f, -0.3f), new Vector3(0.03f, 0.2f, 0.28f), MatPaleGhost());
        Box("Window_Newsprint", fc, new Vector3(7.14f, 1.9f, 1.0f), new Vector3(0.06f, 1.0f, 1.1f), MatCardboard());
        Box("Window_BlackHole", fc, new Vector3(7.14f, 1.9f, -5.0f), new Vector3(0.06f, 1.0f, 1.1f), MatVoid());
    }

    static void AH_FloorsAndMasses(GameObject root)
    {
        var f = Group("FloorsAndMasses", root, Vector3.zero);
        Box("Floor_Interior", f, new Vector3(0, 0.02f, 0), new Vector3(14, 0.06f, 16), MatDirtyTile());
        Box("Floor_Courtyard", f, new Vector3(-0.55f, 0.055f, 1.25f), new Vector3(4.1f, 0.06f, 4.5f), MatTileFloor());

        // sealed poche the player never enters (house abuts the neighbors here)
        Box("Mass_SouthWest", f, new Vector3(-1.3f, HIGH / 2f, -6), new Vector3(11.4f, HIGH, 4), MatRuinBlock());
        Box("Mass_SouthCenter", f, new Vector3(0.6f, HIGH / 2f, -2.5f), new Vector3(4.6f, HIGH, 3), MatRuinBlock());
        Box("Mass_HallSouth", f, new Vector3(3.65f, HIGH / 2f, -2.7f), new Vector3(1.5f, HIGH, 2.6f), MatRuinBlock());
        Box("Mass_FacadeStrip", f, new Vector3(6.75f, HIGH / 2f, -5.4f), new Vector3(0.5f, HIGH, 5.2f), MatRuinBlock());
        Box("Mass_PocheS", f, new Vector3(2.2f, HIGH / 2f, -0.3f), new Vector3(1.4f, HIGH, 1.4f), MatRuinBlock());
        Box("Mass_PocheN", f, new Vector3(2.2f, HIGH / 2f, 2.45f), new Vector3(1.4f, HIGH, 2.1f), MatRuinBlock());
        Box("Mass_StorageN", f, new Vector3(-5.75f, LOW / 2f, 2.75f), new Vector3(2.5f, LOW, 1.5f), MatRuinBlock());

        // second-floor slab (top 3.7) over the south band + east strip; stair head open
        Box("SlabHigh_South", f, new Vector3(-0.75f, 3.6f, -4.5f), new Vector3(12.5f, 0.2f, 7), MatConcrete());
        Box("SlabHigh_SE_A", f, new Vector3(6.25f, 3.6f, -6.45f), new Vector3(1.5f, 0.2f, 3.1f), MatConcrete());
        Box("SlabHigh_SE_B", f, new Vector3(6.25f, 3.6f, -1.85f), new Vector3(1.5f, 0.2f, 1.7f), MatConcrete());
        Box("SlabHigh_East", f, new Vector3(4.6f, 3.6f, 3.5f), new Vector3(4.8f, 0.2f, 9), MatConcrete());

        // roof slab (top 3.1) over the north/west wing; courtyard stays open sky
        Box("SlabLow_North", f, new Vector3(-2.4f, 3.0f, 5.75f), new Vector3(9.2f, 0.2f, 4.5f), MatConcrete());
        Box("SlabLow_West", f, new Vector3(-4.8f, 3.0f, 1.25f), new Vector3(4.4f, 0.2f, 4.5f), MatConcrete());
        Box("SlabLow_EastStrip", f, new Vector3(1.85f, 3.0f, 1.25f), new Vector3(0.7f, 0.2f, 4.5f), MatConcrete());
    }

    static void AH_GroundWalls(GameObject root)
    {
        var w = Group("GroundWalls", root, Vector3.zero);
        var ext = MatRuinRender(); var blk = MatRuinBlock(); var inn = MatRuinInner();

        // exterior shell
        WallNS("Facade_S", w, 7, -8, -2.7f, 0, HIGH, ext);
        WallNS("Facade_N", w, 7, -1.6f, 8, 0, HIGH, ext);
        WallNS("Facade_DoorLintel", w, 7, -2.7f, -1.6f, 2.1f, HIGH - 2.1f, ext);
        var door = Box("FrontDoor_Broken", w, new Vector3(7.15f, 1.02f, -2.5f), new Vector3(0.07f, 2.04f, 1.0f), MatRust());
        door.transform.rotation = Quaternion.Euler(0, -50, 0);
        FixRotatedDoorCollider(door);
        WallNS("Ext_W_S", w, -7, -8, -1, 0, HIGH, blk);
        WallNS("Ext_W_N", w, -7, -1, 8, 0, LOW, blk);
        WallEW("Ext_S", w, -7, 7, -8, 0, HIGH, blk);
        WallEW("Ext_N_W", w, -7, 2.2f, 8, 0, LOW, blk);
        WallEW("Ext_N_E", w, 2.2f, 7, 8, 0, HIGH, blk);

        // north wing south wall (Z 3.5): back room, living room, bedroom doors
        WallEW("NWing_A", w, -7, -4.2f, 3.5f, 0, LOW, inn);
        WallEW("NWing_Lintel_Back", w, -4.2f, -3.3f, 3.5f, 2.1f, LOW - 2.1f, inn);
        WallEW("NWing_B", w, -3.3f, -0.9f, 3.5f, 0, LOW, inn);
        WallEW("NWing_Lintel_Living", w, -0.9f, 0.1f, 3.5f, 2.1f, LOW - 2.1f, inn);
        WallEW("NWing_C", w, 0.1f, 2.2f, 3.5f, 0, LOW, inn);
        WallEW("NWing_D", w, 2.2f, 3.2f, 3.5f, 0, HIGH, inn);
        WallEW("NWing_Lintel_Bed", w, 3.2f, 4.0f, 3.5f, 2.1f, HIGH - 2.1f, inn);
        WallEW("NWing_E", w, 4.0f, 7, 3.5f, 0, HIGH, inn);
        WallNS("Div_BackLiving", w, -3, 3.5f, 8, 0, LOW, inn);
        WallNS("Div_LivingBed", w, 2.2f, 3.5f, 8, 0, HIGH, inn);

        // storage + west lobby
        WallNS("Storage_E_S", w, -4.5f, -1, 0.4f, 0, LOW, inn);
        WallNS("Storage_E_Lintel", w, -4.5f, 0.4f, 1.4f, 2.1f, LOW - 2.1f, inn);
        WallNS("Storage_E_N", w, -4.5f, 1.4f, 2, 0, LOW, inn);
        WallEW("Storage_N", w, -7, -4.5f, 2, 0, LOW, inn);

        // courtyard west wall with the lobby entry gap
        WallNS("CourtW_S", w, -2.6f, -1, 1.0f, 0, LOW, inn);
        WallNS("CourtW_Lintel", w, -2.6f, 1.0f, 2.2f, 2.1f, LOW - 2.1f, inn);
        WallNS("CourtW_N", w, -2.6f, 2.2f, 3.5f, 0, LOW, inn);

        // south run (Z -1): kitchen door + bathroom door gaps
        WallEW("SRun_A", w, -7, -4.4f, -1, 0, HIGH, inn);
        WallEW("SRun_Lintel_Kitchen", w, -4.4f, -3.6f, -1, 2.1f, HIGH - 2.1f, inn);
        WallEW("SRun_B", w, -3.6f, -2.6f, -1, 0, HIGH, inn);
        WallEW("SRun_Lintel_Bath", w, -2.6f, -1.9f, -1, 2.1f, HIGH - 2.1f, inn);
        WallEW("SRun_C", w, -1.9f, 1.5f, -1, 0, HIGH, inn);

        // kitchen + bathroom partitions; the bath door never fully closes
        WallNS("KitchenBath_W", w, -3.5f, -4, -1, 0, HIGH, inn);
        WallEW("Bath_S", w, -3.5f, -1.7f, -2.6f, 0, HIGH, inn);
        WallNS("Bath_E", w, -1.7f, -2.6f, -1, 0, HIGH, inn);
        var bathDoor = Box("Bath_Door_Ajar", w, new Vector3(-2.5f, 1.0f, -1.25f), new Vector3(0.65f, 2.0f, 0.06f), MatWood());
        bathDoor.transform.rotation = Quaternion.Euler(0, -65, 0);
        FixRotatedDoorCollider(bathDoor);

        // sqifa leg 1 (east-west from the front door); stair door in its south wall
        WallEW("Leg1_S_A", w, 4.3f, 4.9f, -2.8f, 0, HIGH, inn);
        WallEW("Leg1_Lintel_Stairs", w, 4.9f, 5.8f, -2.8f, 2.1f, HIGH - 2.1f, inn);
        WallEW("Leg1_S_B", w, 5.8f, 7, -2.8f, 0, HIGH, inn);
        WallEW("Leg1_N", w, 4.3f, 7, -1.4f, 0, HIGH, inn);

        // hallway leg 2 (north-south; first turn); bedroom door at its north end
        WallNS("Hall_W_S", w, 2.9f, -1.4f, 0.4f, 0, HIGH, inn);
        WallNS("Hall_W_Lintel", w, 2.9f, 0.4f, 1.4f, 2.1f, HIGH - 2.1f, inn);
        WallNS("Hall_W_N", w, 2.9f, 1.4f, 3.5f, 0, HIGH, inn);
        WallNS("Hall_E", w, 4.3f, -1.4f, 3.5f, 0, HIGH, inn);

        // leg 3 tunnel into the courtyard (second turn)
        WallEW("Leg3_S", w, 1.5f, 2.9f, 0.4f, 0, HIGH, inn);
        WallEW("Leg3_N", w, 1.5f, 2.9f, 1.4f, 0, HIGH, inn);
        Box("Leg3_Ceiling", w, new Vector3(2.2f, 2.8f, 0.9f), new Vector3(1.4f, 1.4f, 1.0f), MatRuinInner());

        // courtyard east wall with the leg-3 opening
        WallNS("CourtE_S", w, 1.5f, -1, 0.4f, 0, HIGH, inn);
        WallNS("CourtE_Lintel", w, 1.5f, 0.4f, 1.4f, 2.1f, HIGH - 2.1f, inn);
        WallNS("CourtE_N", w, 1.5f, 1.4f, 3.5f, 0, HIGH, inn);

        // stair shaft walls
        WallNS("Shaft_W", w, 4.4f, -5.8f, -2.8f, 0, HIGH, inn);
        WallEW("Shaft_S", w, 4.4f, 6.5f, -5.8f, 0, HIGH, inn);
        WallNS("Shaft_E", w, 6.5f, -5.8f, -2.8f, 0, HIGH, inn);
    }

    static PickupInteractable AH_Courtyard(GameObject root)
    {
        var c = Group("Courtyard", root, Vector3.zero);
        Cylinder("GhostRing", c, new Vector3(-0.5f, 0.09f, 1.2f), 0.9f, 0.015f, MatPaleGhost());
        Cylinder("Drain", c, new Vector3(-0.5f, 0.1f, 1.2f), 0.22f, 0.02f, MatVoid());

        var pipeA = Cylinder("PergolaPipe_A", c, new Vector3(0.6f, 2.6f, 2.9f), 0.03f, 2.0f, MatRust());
        pipeA.transform.rotation = Quaternion.Euler(0, 0, 90);
        var pipeB = Cylinder("PergolaPipe_B", c, new Vector3(0.6f, 2.55f, 3.25f), 0.03f, 2.0f, MatRust());
        pipeB.transform.rotation = Quaternion.Euler(0, 0, 90);
        Sphere("DeadVine", c, new Vector3(0.9f, 2.1f, 3.0f), new Vector3(0.9f, 1.1f, 0.7f), MatDeadCrown());

        var chair = Box("PlasticChair_FacingCorner", c, new Vector3(-2.1f, 0.45f, 2.9f), new Vector3(0.5f, 0.9f, 0.5f), MatPlasticChair());
        chair.transform.rotation = Quaternion.Euler(0, 205, 0);
        Cylinder("ZincBucket", c, new Vector3(1.0f, 0.26f, -0.5f), 0.18f, 0.36f, MatZinc());
        Box("WoodenStool", c, new Vector3(-1.6f, 0.2f, -0.3f), new Vector3(0.35f, 0.4f, 0.35f), MatWood());

        for (int i = 0; i < 5; i++)
        {
            Box("PlanterTin_" + i, c, new Vector3(-2.35f, 0.24f, -0.3f + i * 0.75f), new Vector3(0.3f, 0.34f, 0.3f), MatMetal());
            Sphere("PlanterCrown_" + i, c, new Vector3(-2.35f, 0.52f, -0.3f + i * 0.75f), new Vector3(0.38f, 0.28f, 0.38f), i == 3 ? MatPlant() : MatDeadCrown());
        }
        var sheet = Box("PlasticSheet", c, new Vector3(1.2f, 1.4f, 3.3f), new Vector3(0.7f, 1.1f, 0.02f), MatTarp());
        sheet.transform.rotation = Quaternion.Euler(12, 25, 0);

        // black openings ringing the courtyard from the upper level
        Box("Opening_Dark_S1", c, new Vector3(-1.4f, 4.0f, -0.86f), new Vector3(0.8f, 0.95f, 0.08f), MatVoid());
        Box("Opening_Dark_S2", c, new Vector3(0.6f, 4.0f, -0.86f), new Vector3(0.8f, 0.95f, 0.08f), MatVoid());
        Box("Opening_Dark_N", c, new Vector3(-0.6f, 3.35f, 3.36f), new Vector3(0.8f, 0.5f, 0.08f), MatVoid());

        var ball = Sphere("Ball", c, new Vector3(-0.5f, 0.22f, 1.2f), new Vector3(0.35f, 0.35f, 0.35f), MatDoorLocked());
        var pickup = ball.AddComponent<PickupInteractable>();
        pickup.itemName = "Ball";
        pickup.promptText = "Pick up ball";
        Object.DestroyImmediate(ball.GetComponent<SphereCollider>());
        var ballCollider = ball.AddComponent<BoxCollider>();
        ballCollider.size = new Vector3(0.35f, 0.35f, 0.35f);
        return pickup;
    }

    static void AH_RoomProps(GameObject root)
    {
        // sqifa (bent entrance)
        var sq = Group("Sqifa", root, Vector3.zero);
        var rack = Box("ShoeRack_Broken", sq, new Vector3(6.3f, 0.25f, -1.65f), new Vector3(0.8f, 0.5f, 0.25f), MatWood());
        rack.transform.rotation = Quaternion.Euler(0, 0, -8);
        Box("Sandal_Adult", sq, new Vector3(5.6f, 0.06f, -1.7f), new Vector3(0.28f, 0.05f, 0.11f), MatSoot());
        Box("Sandal_Child", sq, new Vector3(5.35f, 0.06f, -1.72f), new Vector3(0.18f, 0.04f, 0.08f), MatCrate());
        Cylinder("DeadFlashlight", sq, new Vector3(6.6f, 0.08f, -2.4f), 0.03f, 0.18f, MatMetal());

        // living room (X -3..2.2, Z 3.5..8): social shape, nobody in it
        var lv = Group("LivingRoom", root, Vector3.zero);
        Box("Carpet", lv, new Vector3(-0.4f, 0.07f, 5.7f), new Vector3(2.6f, 0.03f, 1.8f), MatCarpet());
        Box("FloorMattress_A", lv, new Vector3(-0.4f, 0.17f, 7.5f), new Vector3(1.9f, 0.24f, 0.8f), MatMattress());
        Box("FloorMattress_B", lv, new Vector3(-2.5f, 0.17f, 5.7f), new Vector3(0.8f, 0.24f, 1.9f), MatMattress());
        Box("FloorMattress_C", lv, new Vector3(1.1f, 0.17f, 4.2f), new Vector3(1.6f, 0.24f, 0.8f), MatMattress());
        Box("TeaTable", lv, new Vector3(-0.4f, 0.26f, 5.7f), new Vector3(0.8f, 0.35f, 0.8f), MatWood());
        Cylinder("TeaTray", lv, new Vector3(-0.4f, 0.46f, 5.7f), 0.3f, 0.03f, MatZinc());
        Sphere("Teapot_Fallen", lv, new Vector3(-0.15f, 0.51f, 5.6f), new Vector3(0.18f, 0.18f, 0.18f), MatMetal());
        var tv = Box("CRT_TV_FacingDoorway", lv, new Vector3(1.7f, 0.35f, 7.5f), new Vector3(0.6f, 0.5f, 0.55f), MatVoid());
        tv.transform.rotation = Quaternion.Euler(0, 215, 0);
        Box("PhotoShelf", lv, new Vector3(-2.8f, 2.2f, 6.5f), new Vector3(0.2f, 0.05f, 1.2f), MatWood());
        Box("Frame_FaceDown", lv, new Vector3(-2.8f, 2.25f, 6.2f), new Vector3(0.16f, 0.03f, 0.24f), MatWood());
        Box("Frame_Standing", lv, new Vector3(-2.82f, 2.35f, 6.7f), new Vector3(0.03f, 0.25f, 0.2f), MatWood());
        Box("Window_ToCourt", lv, new Vector3(-0.4f, 1.9f, 3.66f), new Vector3(1.1f, 1.0f, 0.06f), MatWindowDark());
        var curtain = Box("Curtain_Rag", lv, new Vector3(-0.15f, 1.75f, 3.72f), new Vector3(0.5f, 1.4f, 0.03f), MatTarp());
        curtain.transform.rotation = Quaternion.Euler(4, 0, 6);
        // the NW corner is left empty on purpose - the watched corner

        // bedroom (X 2.2..7, Z 3.5..8): the contradictory bed
        var bd = Group("Bedroom", root, Vector3.zero);
        Box("BedFrame", bd, new Vector3(5.6f, 0.3f, 7.0f), new Vector3(2.0f, 0.5f, 1.4f), MatRust());
        Box("BedMattress", bd, new Vector3(5.6f, 0.62f, 7.0f), new Vector3(1.9f, 0.18f, 1.3f), MatMattress());
        var thrown = Box("Sheet_ThrownBack", bd, new Vector3(5.2f, 0.76f, 6.7f), new Vector3(1.1f, 0.08f, 0.9f), MatPaleGhost());
        thrown.transform.rotation = Quaternion.Euler(0, 18, 4);
        Box("Wardrobe", bd, new Vector3(6.6f, 1.0f, 4.4f), new Vector3(0.6f, 2.0f, 1.2f), MatWood());
        var wdoor = Box("WardrobeDoor_Leaning", bd, new Vector3(6.2f, 0.95f, 5.3f), new Vector3(0.06f, 1.9f, 0.55f), MatWood());
        wdoor.transform.rotation = Quaternion.Euler(-10, 15, 0);
        FixRotatedDoorCollider(wdoor);
        Box("Suitcase_OnTop", bd, new Vector3(6.6f, 2.25f, 4.4f), new Vector3(0.5f, 0.35f, 0.8f), MatCardboard());
        Box("WeddingChest", bd, new Vector3(4.6f, 0.3f, 6.1f), new Vector3(1.0f, 0.6f, 0.55f), MatWood());
        var lid = Box("ChestLid_Open", bd, new Vector3(4.6f, 0.68f, 5.9f), new Vector3(1.0f, 0.06f, 0.55f), MatWood());
        lid.transform.rotation = Quaternion.Euler(-25, 0, 0);
        FixRotatedDoorCollider(lid);
        Box("Dresser", bd, new Vector3(3.0f, 0.5f, 7.6f), new Vector3(0.9f, 1.0f, 0.45f), MatWood());
        Box("Mirror_CrackedAtFaceHeight", bd, new Vector3(3.0f, 1.75f, 7.84f), new Vector3(0.5f, 0.7f, 0.04f), MatMirror());
        Box("Drawing_UnderBed", bd, new Vector3(5.0f, 0.06f, 7.3f), new Vector3(0.28f, 0.01f, 0.2f), MatPaleGhost());

        // kitchen (X -7..-3.5, Z -4..-1)
        var kt = Group("Kitchen", root, Vector3.zero);
        Box("Counter", kt, new Vector3(-6.55f, 0.45f, -2.5f), new Vector3(0.7f, 0.9f, 2.6f), MatDirtyTile());
        Box("SinkHole", kt, new Vector3(-6.55f, 0.91f, -2.0f), new Vector3(0.5f, 0.04f, 0.5f), MatVoid());
        Box("Stove_Rusted", kt, new Vector3(-6.5f, 0.4f, -3.7f), new Vector3(0.6f, 0.8f, 0.55f), MatRust());
        Cylinder("GasCylinder_Butane", kt, new Vector3(-5.8f, 0.38f, -3.75f), 0.16f, 0.75f, MatDoorBlue());
        Box("DeadFridge", kt, new Vector3(-3.9f, 0.85f, -3.6f), new Vector3(0.7f, 1.7f, 0.7f), MatFridgeDead());
        var fdoor = Box("FridgeDoor_Leaning", kt, new Vector3(-4.6f, 0.8f, -3.85f), new Vector3(0.08f, 1.6f, 0.6f), MatFridgeDead());
        fdoor.transform.rotation = Quaternion.Euler(-8, 30, 0);
        FixRotatedDoorCollider(fdoor);
        Box("WallCabinet", kt, new Vector3(-5.4f, 2.0f, -3.8f), new Vector3(0.8f, 0.6f, 0.35f), MatWood());
        var cdoor = Box("CabinetDoor_Ajar", kt, new Vector3(-5.0f, 2.0f, -3.6f), new Vector3(0.4f, 0.55f, 0.04f), MatWood());
        cdoor.transform.rotation = Quaternion.Euler(0, 40, 0);
        FixRotatedDoorCollider(cdoor);
        for (int i = 0; i < 4; i++)
            Box("DishShard_" + i, kt, new Vector3(-6.7f + i * 0.12f, 0.07f, -1.3f - (i % 2) * 0.15f), new Vector3(0.12f, 0.04f, 0.1f), MatFridge());
        Cylinder("Can_A", kt, new Vector3(-6.2f, 0.11f, -1.5f), 0.05f, 0.12f, MatMetal());
        Cylinder("Can_B", kt, new Vector3(-6.0f, 0.11f, -1.35f), 0.05f, 0.12f, MatMetal());
        Box("TeaTin_Waiting", kt, new Vector3(-6.5f, 0.98f, -2.9f), new Vector3(0.12f, 0.16f, 0.12f), MatCrate());

        // bathroom (X -3.5..-1.7, Z -2.6..-1): fractured mirror, dripping tap
        var bt = Group("Bathroom", root, Vector3.zero);
        Box("Sink", bt, new Vector3(-3.2f, 0.75f, -2.3f), new Vector3(0.45f, 0.15f, 0.35f), MatFridge());
        Box("MirrorFrame", bt, new Vector3(-3.35f, 1.5f, -2.3f), new Vector3(0.04f, 0.5f, 0.4f), MatWood());
        Box("MirrorShard_A", bt, new Vector3(-3.32f, 1.55f, -2.38f), new Vector3(0.02f, 0.2f, 0.14f), MatMirror());
        Box("MirrorShard_B", bt, new Vector3(-3.32f, 1.42f, -2.2f), new Vector3(0.02f, 0.16f, 0.12f), MatMirror());
        Cylinder("FloorDrain", bt, new Vector3(-2.5f, 0.06f, -1.9f), 0.08f, 0.02f, MatVoid());
        Cylinder("Bucket", bt, new Vector3(-2.0f, 0.25f, -2.4f), 0.16f, 0.4f, MatZinc());

        // storage (X -7..-4.5, Z -1..2): windowless, closed door feel
        var st = Group("Storage", root, Vector3.zero);
        Box("BoxStack_A", st, new Vector3(-6.5f, 0.45f, 1.4f), new Vector3(0.8f, 0.8f, 0.8f), MatCardboard());
        Box("BoxStack_B", st, new Vector3(-6.5f, 1.1f, 1.4f), new Vector3(0.7f, 0.5f, 0.7f), MatCardboard());
        Box("BoxStack_C", st, new Vector3(-6.4f, 0.35f, 0.5f), new Vector3(0.6f, 0.6f, 0.6f), MatCardboard());
        Sphere("BlanketBundle", st, new Vector3(-5.5f, 0.32f, 1.6f), new Vector3(0.8f, 0.55f, 0.6f), MatMattress());
        Sphere("Sack_Knotted_A", st, new Vector3(-6.6f, 0.35f, -0.5f), new Vector3(0.55f, 0.65f, 0.5f), MatTarp());
        Sphere("Sack_Knotted_B", st, new Vector3(-6.0f, 0.31f, -0.7f), new Vector3(0.5f, 0.55f, 0.45f), MatTarp());
        Sphere("OilJar", st, new Vector3(-4.9f, 0.38f, -0.6f), new Vector3(0.5f, 0.7f, 0.5f), MatRuinRender());
        Box("ToyBox", st, new Vector3(-5.6f, 0.18f, 0.3f), new Vector3(0.5f, 0.3f, 0.4f), MatCardboard());
        Box("ToyTruck", st, new Vector3(-5.55f, 0.38f, 0.3f), new Vector3(0.18f, 0.1f, 0.1f), MatCrate());
        Sphere("Football_Deflated", st, new Vector3(-5.2f, 0.11f, 0.7f), new Vector3(0.24f, 0.12f, 0.24f), MatFridge());
        Box("Shelf_Alone", st, new Vector3(-6.75f, 1.5f, 0.2f), new Vector3(0.25f, 0.05f, 0.7f), MatWood());
        Box("SchoolSatchel", st, new Vector3(-6.75f, 1.64f, 0.2f), new Vector3(0.18f, 0.22f, 0.32f), MatCrate());

        // back room (X -7..-3, Z 3.5..8): the recent layer, directly under the locked room
        var br = Group("BackRoom", root, Vector3.zero);
        Box("Mattress_Squared", br, new Vector3(-6.3f, 0.16f, 7.2f), new Vector3(0.9f, 0.22f, 1.9f), MatMattress());
        Box("Blanket_Folded", br, new Vector3(-6.3f, 0.32f, 7.6f), new Vector3(0.7f, 0.1f, 0.5f), MatPaleGhost());
        Cylinder("Candle_Saucer", br, new Vector3(-5.6f, 0.06f, 7.4f), 0.08f, 0.02f, MatFridge());
        Cylinder("Candle_Stub", br, new Vector3(-5.6f, 0.11f, 7.4f), 0.02f, 0.08f, MatPaleGhost());
        Cylinder("WaterBottle_Recent", br, new Vector3(-5.4f, 0.17f, 7.2f), 0.045f, 0.24f, MatDish());
        Box("Drawing_Pinned", br, new Vector3(-6.86f, 1.1f, 6.0f), new Vector3(0.02f, 0.28f, 0.2f), MatPaleGhost());
    }

    static void AH_Stairs(GameObject root)
    {
        var s = Group("Stairwell", root, Vector3.zero);
        Box("SpineWall", s, new Vector3(5.45f, 1.75f, -3.85f), new Vector3(0.15f, 3.5f, 2.1f), MatRuinInner());
        for (int i = 0; i < 9; i++) // flight one, down the west side, rising south
            Box("Step_A" + i, s, new Vector3(4.9f, 0.1f + i * 0.205f, -3.0f - i * 0.233f), new Vector3(0.95f, 0.2f, 0.28f), MatConcrete());
        Box("Landing", s, new Vector3(5.45f, 1.75f, -5.35f), new Vector3(2.1f, 0.2f, 0.9f), MatConcrete());
        for (int i = 0; i < 9; i++) // flight two, east side, rising north to the slab
            Box("Step_B" + i, s, new Vector3(6.0f, 1.96f + i * 0.205f, -4.75f + i * 0.233f), new Vector3(0.95f, 0.2f, 0.28f), MatConcrete());
        var rail = Box("Handrail_Loose", s, new Vector3(4.6f, 1.15f, -3.9f), new Vector3(0.05f, 0.05f, 2.4f), MatMetal());
        rail.transform.rotation = Quaternion.Euler(38, 0, 0);
        Box("SlitWindow_Dark", s, new Vector3(5.45f, 2.3f, -5.68f), new Vector3(0.9f, 0.25f, 0.06f), MatVoid());

        // baked-moon stand-in: the one cold slab of light, falling on the landing
        var slitGO = new GameObject("Moonlight_Slit");
        slitGO.transform.SetParent(s.transform, false);
        slitGO.transform.localPosition = new Vector3(5.45f, 2.5f, -5.6f);
        slitGO.transform.rotation = Quaternion.Euler(50, 0, 0);
        var slit = slitGO.AddComponent<Light>();
        slit.type = LightType.Spot;
        slit.spotAngle = 45;
        slit.range = 4.5f;
        slit.color = Hex("#A8BBD4");
        slit.intensity = 1.3f;
        slit.shadows = LightShadows.None;
    }

    static void AH_UpperLevel(GameObject root)
    {
        var u = Group("UpperLevel", root, Vector3.zero);
        var blk = MatRuinBlock();

        // low block edge around the courtyard void - the only guard rail
        WallEW("VoidEdge_S", u, -2.6f, 1.5f, -1, HIGHTOP, 0.6f, blk);
        WallEW("VoidEdge_N", u, -2.6f, 1.5f, 3.5f, LOWTOP, 0.6f, blk);
        WallNS("VoidEdge_W", u, -2.6f, -1, 3.5f, LOWTOP, 0.6f, blk);
        WallNS("VoidEdge_E", u, 1.5f, -1, 3.5f, LOWTOP, 0.6f, blk);

        // seams where the high slab (3.7) meets the low roof (3.1) + the three steps
        WallEW("StepWall_W", u, -7, -2.6f, -1, LOWTOP, 0.6f, blk);
        WallNS("StepWall_E", u, 2.2f, -1, 3.5f, LOWTOP, 0.6f, blk);
        WallNS("StepWall_N_A", u, 2.2f, 3.5f, 5.0f, LOWTOP, 0.6f, blk);
        WallNS("StepWall_N_B", u, 2.2f, 6.2f, 8, LOWTOP, 0.6f, blk);
        Box("StepTread_1", u, new Vector3(2.02f, 3.3f, 5.6f), new Vector3(0.36f, 0.4f, 1.2f), MatConcrete());
        Box("StepTread_2", u, new Vector3(1.67f, 3.2f, 5.6f), new Vector3(0.34f, 0.2f, 1.2f), MatConcrete());

        // parapets
        WallEW("Parapet_N_Low", u, -7, 2.2f, 8, LOWTOP, 1.0f, blk);
        WallEW("Parapet_N_High", u, 2.2f, 7, 8, HIGHTOP, 1.0f, blk);
        WallNS("Parapet_W_Low", u, -7, -1, 8, LOWTOP, 1.0f, blk);
        WallNS("Parapet_W_High", u, -7, -8, -1, HIGHTOP, 1.0f, blk);
        WallNS("Parapet_E", u, 7, -8, 8, HIGHTOP, 1.0f, blk);
        WallEW("Parapet_S", u, -7, 7, -8, HIGHTOP, 1.0f, blk);

        // unfinished maze: block walls tracing rooms that were never built
        var mz = Group("UnfinishedMaze", u, Vector3.zero);
        WallEW("Trace_A", mz, -6.2f, -2, -4.6f, HIGHTOP, 1.2f, blk);
        WallNS("Trace_B", mz, -2, -7.2f, -4.6f, HIGHTOP, 1.2f, blk);
        WallNS("Trace_C_S", mz, 1.2f, -6.5f, -4.6f, HIGHTOP, 1.2f, blk);
        WallNS("Trace_C_N", mz, 1.2f, -3.7f, -2.2f, HIGHTOP, 1.2f, blk);
        WallEW("Trace_D", mz, 1.2f, 4.6f, -2.2f, HIGHTOP, 0.6f, blk);
        Column(mz, new Vector3(-6.2f, HIGHTOP, -4.6f));
        Column(mz, new Vector3(1.2f, HIGHTOP, -6.5f));
        Column(mz, new Vector3(4.6f, HIGHTOP, -2.2f));
        Box("CementBags_Hardened", mz, new Vector3(-4.5f, HIGHTOP + 0.3f, -6.5f), new Vector3(1.2f, 0.6f, 0.8f), MatDust());
        Sphere("SandPile_Crusted", mz, new Vector3(-0.5f, HIGHTOP, -6.8f), new Vector3(2.4f, 0.7f, 2.4f), MatDust());
        Box("BlockStack", mz, new Vector3(3.4f, HIGHTOP + 0.3f, -6.9f), new Vector3(1.6f, 0.6f, 0.8f), MatBlock());
        Box("Mattress_Hideout", mz, new Vector3(-5.6f, HIGHTOP + 0.35f, -3.4f), new Vector3(1.9f, 0.25f, 0.9f), MatMattress());
        Box("CarSeat", mz, new Vector3(-4.4f, HIGHTOP + 0.35f, -3.2f), new Vector3(0.6f, 0.7f, 0.6f), MatSoot());
        Box("JunctionBox_Dead", mz, new Vector3(6.8f, HIGHTOP + 1.3f, -5), new Vector3(0.15f, 0.3f, 0.25f), MatMetal());
        var wire = Cylinder("LooseWire", mz, new Vector3(5.8f, HIGHTOP + 1.0f, -5), 0.012f, 2.0f, MatSoot());
        wire.transform.rotation = Quaternion.Euler(0, 0, 70);

        // rooftop over the north/west wing - every chased sound gets its source here
        var rf = Group("Roof", u, Vector3.zero);
        Tank(rf, new Vector3(-1.6f, LOWTOP, 5.6f));
        Dish(rf, new Vector3(0.6f, LOWTOP + 0.1f, 4.6f), true);
        Box("ZincSheet", rf, new Vector3(-5.2f, LOWTOP + 0.05f, 4.1f), new Vector3(1.3f, 0.04f, 2.2f), MatZinc());
        Box("HatchShadow", rf, new Vector3(-5.2f, LOWTOP - 0.02f, 4.1f), new Vector3(1.0f, 0.02f, 1.9f), MatVoid());
        Box("Mattress_Folded", rf, new Vector3(0.6f, LOWTOP + 0.35f, 7.5f), new Vector3(1.0f, 0.7f, 0.5f), MatMattress());
        var chair = Box("PlasticChair_Broken", rf, new Vector3(-0.8f, LOWTOP + 0.25f, 6.6f), new Vector3(0.5f, 0.5f, 0.5f), MatPlasticChair());
        chair.transform.rotation = Quaternion.Euler(0, 30, 95);
        Box("LinePost_A", rf, new Vector3(-6.4f, LOWTOP + 0.6f, 0.3f), new Vector3(0.08f, 1.2f, 0.08f), MatMetal());
        Box("LinePost_B", rf, new Vector3(-6.4f, LOWTOP + 0.6f, 2.9f), new Vector3(0.08f, 1.2f, 0.08f), MatMetal());
        Box("Clothesline", rf, new Vector3(-6.4f, LOWTOP + 1.15f, 1.6f), new Vector3(0.03f, 0.03f, 2.6f), MatMetal());
        Box("Rag_Petrified", rf, new Vector3(-6.4f, LOWTOP + 0.9f, 1.9f), new Vector3(0.05f, 0.5f, 0.4f), MatTarp());
        var antenna = Cylinder("Antenna_Fallen", rf, new Vector3(-6.5f, LOWTOP + 0.9f, 7.3f), 0.03f, 2.4f, MatMetal());
        antenna.transform.rotation = Quaternion.Euler(35, 20, 0);
        Box("BlockStack_Roof", rf, new Vector3(1.5f, LOWTOP + 0.25f, 6.8f), new Vector3(1.0f, 0.5f, 0.6f), MatBlock());
    }

    static void AH_LockedRoom(GameObject root)
    {
        // NW corner of the roof, directly above the back room, in the wind-shadow
        var lr = Group("LockedRoom", root, Vector3.zero);
        var ext = MatRuinRender();
        WallEW("LR_Wall_N", lr, -6.8f, -3.9f, 7.7f, LOWTOP, 2.4f, ext);
        WallEW("LR_Wall_S", lr, -6.8f, -3.9f, 4.7f, LOWTOP, 2.4f, ext);
        WallNS("LR_Wall_W", lr, -6.8f, 4.7f, 7.7f, LOWTOP, 2.4f, ext);
        WallNS("LR_Wall_E_S", lr, -3.9f, 4.7f, 5.7f, LOWTOP, 2.4f, ext);
        WallNS("LR_Wall_E_N", lr, -3.9f, 6.6f, 7.7f, LOWTOP, 2.4f, ext);
        WallNS("LR_Lintel", lr, -3.9f, 5.7f, 6.6f, LOWTOP + 1.95f, 0.45f, ext);
        Box("LR_Roof", lr, new Vector3(-5.35f, LOWTOP + 2.475f, 6.2f), new Vector3(3.2f, 0.15f, 3.3f), MatConcrete());

        // the heavy locked door: hasp, newer padlock, hand-polished patch
        Box("LR_Door_Locked", lr, new Vector3(-3.9f, LOWTOP + 0.975f, 6.15f), new Vector3(0.09f, 1.95f, 0.88f), MatDoorLocked());
        Box("LR_Hasp", lr, new Vector3(-3.83f, LOWTOP + 1.1f, 5.78f), new Vector3(0.05f, 0.06f, 0.2f), MatMetal());
        Box("LR_Padlock_New", lr, new Vector3(-3.81f, LOWTOP + 0.98f, 5.72f), new Vector3(0.06f, 0.14f, 0.09f), MatFridge());
        Box("LR_RubbedPatch", lr, new Vector3(-3.845f, LOWTOP + 1.15f, 6.5f), new Vector3(0.005f, 0.18f, 0.14f), MatDish());

        // tiny cracked window, shuttered from inside, on the approach face
        Box("LR_Shutter", lr, new Vector3(-5.9f, LOWTOP + 1.85f, 4.56f), new Vector3(0.5f, 0.4f, 0.07f), MatWood());

        // interior: readable in one sweep, worse on the second look
        Box("LR_BedFrame", lr, new Vector3(-6.35f, LOWTOP + 0.22f, 6.4f), new Vector3(0.75f, 0.4f, 1.8f), MatRust());
        Box("LR_BedMattress", lr, new Vector3(-6.35f, LOWTOP + 0.48f, 6.4f), new Vector3(0.7f, 0.14f, 1.7f), MatMattress());
        Box("LR_Blanket_Made", lr, new Vector3(-6.35f, LOWTOP + 0.58f, 6.3f), new Vector3(0.66f, 0.06f, 1.3f), MatPaleGhost());
        Box("LR_Pillow", lr, new Vector3(-6.35f, LOWTOP + 0.6f, 7.15f), new Vector3(0.5f, 0.1f, 0.3f), MatMattress());
        Box("LR_Cabinet", lr, new Vector3(-4.7f, LOWTOP + 0.55f, 4.95f), new Vector3(0.85f, 1.1f, 0.4f), MatWood());
        Box("LR_CassetteRecorder", lr, new Vector3(-4.8f, LOWTOP + 1.16f, 4.95f), new Vector3(0.24f, 0.12f, 0.16f), MatSoot());
        Box("LR_CassetteTape", lr, new Vector3(-4.5f, LOWTOP + 1.13f, 4.95f), new Vector3(0.11f, 0.02f, 0.07f), MatVoid());
        Box("LR_MirrorFrame", lr, new Vector3(-5.4f, LOWTOP + 1.5f, 7.52f), new Vector3(0.5f, 0.7f, 0.04f), MatWood());
        Box("LR_PaleRectangle", lr, new Vector3(-5.4f, LOWTOP + 1.5f, 7.565f), new Vector3(0.42f, 0.62f, 0.02f), MatPaleGhost());
        Box("LR_GlassInFrame", lr, new Vector3(-5.22f, LOWTOP + 1.28f, 7.51f), new Vector3(0.12f, 0.16f, 0.02f), MatMirror());
        var shardA = Box("LR_GlassBlade_A", lr, new Vector3(-5.5f, LOWTOP + 0.03f, 7.4f), new Vector3(0.3f, 0.02f, 0.12f), MatMirror());
        shardA.transform.rotation = Quaternion.Euler(0, 15, 0);
        var shardB = Box("LR_GlassBlade_B", lr, new Vector3(-5.25f, LOWTOP + 0.03f, 7.35f), new Vector3(0.24f, 0.02f, 0.1f), MatMirror());
        shardB.transform.rotation = Quaternion.Euler(0, -25, 0);
        Box("LR_Box_TiedBlackCloth", lr, new Vector3(-6.3f, LOWTOP + 0.12f, 5.55f), new Vector3(0.45f, 0.22f, 0.32f), MatWood());
        Box("LR_ClothStrip", lr, new Vector3(-6.3f, LOWTOP + 0.24f, 5.55f), new Vector3(0.47f, 0.03f, 0.08f), MatSoot());
        Cylinder("LR_Plate", lr, new Vector3(-4.25f, LOWTOP + 0.04f, 6.0f), 0.12f, 0.03f, MatFridge());
        Box("LR_Spoon", lr, new Vector3(-4.3f, LOWTOP + 0.06f, 5.85f), new Vector3(0.15f, 0.01f, 0.03f), MatMetal());
        for (int i = 0; i < 5; i++)
            Box("LR_PhotoScrap_" + i, lr,
                new Vector3(-6.55f + (i * 0.37f) % 1.1f, LOWTOP + 0.03f, 5.2f + (i * 0.53f) % 0.8f),
                new Vector3(0.09f, 0.005f, 0.07f), MatPaleGhost());
        // the SW corner is left completely empty - it must contain nothing, every time
    }

    // --------------------------------------------------------- street lighting

    static void BuildStreetLighting()
    {
        var root = Group("_StreetLights");

        // working sodium lights: alley mouth, alley bend, road x 20, road x -55 (dying)
        StreetLamp(root, "Lamp_AlleyMouth", new Vector3(42.6f, 0, -6), true, 1.0f);
        StreetLamp(root, "Lamp_AlleyBend", new Vector3(42.6f, 0, -64), true, 0.9f);
        StreetLamp(root, "Lamp_Road_E", new Vector3(20, 0, 4.6f), true, 1.0f);
        StreetLamp(root, "Lamp_Road_Dead", new Vector3(-20, 0, 4.6f), false, 0);   // pole only
        StreetLamp(root, "Lamp_Road_Dying", new Vector3(-55, 0, 4.6f), true, 0.45f);

        // two more doorway bulbs deep in the alley (weak warm pools)
        PointLight(root, "DoorBulb_A", new Vector3(37.6f, 2.3f, -18), WarmBulb, 1.3f, 6f);
        PointLight(root, "DoorBulb_B", new Vector3(42.4f, 2.3f, -36), WarmBulb, 1.1f, 6f);

        // distant scenery: minaret with green crown light + city glow marker
        var minaret = Group("Distant_Minaret", root, new Vector3(-120, 0, 90));
        Cylinder("Tower", minaret, new Vector3(0, 14, 0), 1.2f, 28, MatRenderCream());
        Sphere("Crown", minaret, new Vector3(0, 29, 0), new Vector3(2.4f, 2.4f, 2.4f), MatMinaretGreen());
        PointLight(minaret, "GreenLight", new Vector3(0, 29, 0), new Color(0.35f, 1f, 0.55f), 2.5f, 25f);
    }

    // ---------------------------------------------------- global light and sky

    static void BuildGlobalLightingAndSky()
    {
        var root = Group("_GlobalLighting");

        // moon: the true key light - cool, low intensity, hard shadows
        var moonGO = new GameObject("Moon_Directional");
        moonGO.transform.SetParent(root.transform);
        moonGO.transform.rotation = Quaternion.Euler(52, 215, 0);
        var moon = moonGO.AddComponent<Light>();
        moon.type = LightType.Directional;
        moon.color = Hex("#A8BBD4");
        moon.intensity = 0.35f;
        moon.shadows = LightShadows.Soft;

        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Hex("#1E222A");
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Hex("#151A20");
        RenderSettings.fogDensity = 0.012f;
        RenderSettings.sun = moon;
    }

    // -------------------------------------------------------------- player start

    static PlayerController BuildPlayerStart()
    {
        return CreatePlayerRig();
    }

    static PlayerController CreatePlayerRig()
    {
        // playable first-person rig on the family house doorstep, facing up the alley
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(38.6f, 0.1f, -45.6f);
        player.transform.rotation = Quaternion.Euler(0, 15, 0);

        var cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0, 0.95f, 0);
        cc.stepOffset = 0.35f;  // stair risers are 0.2
        cc.slopeLimit = 50f;

        var cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, 1.62f, 0);
        cam.transform.localRotation = Quaternion.identity;
        var c = cam.AddComponent<Camera>();
        c.nearClipPlane = 0.05f;
        c.farClipPlane = 400f;
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = Hex("#0D1117");
        cam.AddComponent<AudioListener>();

        // warm hand light against the cold moon (F toggles it in play mode)
        var phoneGO = new GameObject("Phone");
        phoneGO.transform.SetParent(player.transform, false);
        phoneGO.transform.localPosition = new Vector3(0.25f, 1.45f, 0.35f);
        var phoneLight = phoneGO.AddComponent<Light>();
        phoneLight.type = LightType.Spot;
        phoneLight.color = Hex("#F2C98C");
        phoneLight.spotAngle = 55f;
        phoneLight.range = 18f;
        phoneLight.intensity = 1.6f;
        phoneLight.shadows = LightShadows.Soft;

        var phone = player.AddComponent<PhoneController>();
        phone.phoneLight = phoneLight;

        var controller = player.AddComponent<PlayerController>();
        controller.cameraPivot = cam.transform;
        controller.phone = phone;

        var interaction = player.AddComponent<InteractionController>();
        interaction.interactionRange = 2.5f;
        interaction.interactableLayers = LayerMask.GetMask("Default");

        return controller;
    }

    // ============================================================ prefab helpers

    static GameObject House(GameObject parent, string name, Vector3 pos, float depth, float width, int seed)
    {
        var h = Group(name, parent, pos);
        bool unfinished = seed % 4 == 1;
        float height = unfinished ? 3.5f : (seed % 3 == 0 ? 3.5f : 6.5f);
        Material tone = seed % 3 == 0 ? MatRenderCream() : seed % 3 == 1 ? MatRenderOchre() : MatRenderGreen();

        Box("Body", h, new Vector3(0, height / 2f, 0), new Vector3(depth, height, width), tone);
        Parapet(h, new Vector3(0, height, 0), depth, width);

        if (unfinished)
        {
            Box("UnfinishedWall_A", h, new Vector3(0, height + 0.6f, width / 2f - 0.15f), new Vector3(depth, 1.2f, 0.25f), MatBlock());
            Box("UnfinishedWall_B", h, new Vector3(0, height + 0.6f, -width / 2f + 0.15f), new Vector3(depth, 1.2f, 0.25f), MatBlock());
            Column(h, new Vector3(depth / 2f - 0.3f, height, width / 2f - 0.3f));
            Column(h, new Vector3(-depth / 2f + 0.3f, height, -width / 2f + 0.3f));
        }
        if (seed % 2 == 0) Tank(h, new Vector3(depth * 0.2f, height, -width * 0.25f));
        if (seed % 3 == 0) Dish(h, new Vector3(-depth * 0.2f, height + 0.1f, width * 0.25f), false);

        // door + meter box on the street-facing side (east for west-side houses, chosen by X sign)
        float face = pos.x < 40 ? depth / 2f : -depth / 2f;
        Material door = seed % 3 == 0 ? MatDoorGreen() : seed % 3 == 1 ? MatDoorBrown() : MatDoorBlue();
        Box("Door", h, new Vector3(face * 1.008f, 1.05f, 0.8f), new Vector3(0.08f, 2.1f, 1.3f), door);
        Box("MeterBox", h, new Vector3(face * 1.01f, 1.7f, 1.9f), new Vector3(0.12f, 0.4f, 0.3f), MatMetal());
        Box("Window", h, new Vector3(face * 1.005f, 1.9f, -1.6f), new Vector3(0.06f, 1.0f, 1.1f), MatWindowDark());
        return h;
    }

    static void Parapet(GameObject parent, Vector3 topCenter, float sizeX, float sizeZ)
        => ParapetAt(parent, topCenter, sizeX, sizeZ, 0.7f, null);

    static void ParapetAt(GameObject parent, Vector3 topCenter, float sizeX, float sizeZ, float h, Material m)
    {
        m = m != null ? m : MatConcrete();
        Box("Parapet_N", parent, topCenter + new Vector3(0, h / 2f, sizeZ / 2f - 0.1f), new Vector3(sizeX, h, 0.2f), m);
        Box("Parapet_S", parent, topCenter + new Vector3(0, h / 2f, -sizeZ / 2f + 0.1f), new Vector3(sizeX, h, 0.2f), m);
        Box("Parapet_E", parent, topCenter + new Vector3(sizeX / 2f - 0.1f, h / 2f, 0), new Vector3(0.2f, h, sizeZ - 0.4f), m);
        Box("Parapet_W", parent, topCenter + new Vector3(-sizeX / 2f + 0.1f, h / 2f, 0), new Vector3(0.2f, h, sizeZ - 0.4f), m);
    }

    static GameObject Tank(GameObject parent, Vector3 basePos)
    {
        var t = Group("WaterTank", parent, basePos);
        Box("Stand", t, new Vector3(0, 0.4f, 0), new Vector3(1.2f, 0.8f, 1.2f), MatMetal());
        Cylinder("Tank", t, new Vector3(0, 1.55f, 0), 0.75f, 1.5f, MatTank());
        return t;
    }

    static void Dish(GameObject parent, Vector3 pos, bool fallen)
    {
        var d = Group("SatDish", parent, pos);
        Cylinder("DishFace", d, new Vector3(0, fallen ? 0.15f : 1.0f, 0), 0.55f, 0.06f, MatDish());
        d.transform.GetChild(0).rotation = Quaternion.Euler(fallen ? 80 : 35, 20, 0);
        if (!fallen) Box("Mast", d, new Vector3(0, 0.5f, 0), new Vector3(0.06f, 1.0f, 0.06f), MatMetal());
    }

    static void Column(GameObject parent, Vector3 basePos)
    {
        var c = Group("Column", parent, basePos);
        Box("Concrete", c, new Vector3(0, 1.1f, 0), new Vector3(0.3f, 2.2f, 0.3f), MatBlock());
        for (int r = 0; r < 4; r++)
            Cylinder("Rebar_" + r, c, new Vector3(-0.08f + 0.05f * r, 2.5f, -0.08f + 0.05f * r), 0.012f, 0.6f, MatRust());
    }

    static void StreetLamp(GameObject parent, string name, Vector3 basePos, bool lit, float strength)
    {
        var l = Group(name, parent, basePos);
        Cylinder("Pole", l, new Vector3(0, 3.25f, 0), 0.09f, 6.5f, MatMetal());
        // arm reaches over the street (toward the road/alley center)
        float armDir = basePos.x > 41 ? -1 : (Mathf.Abs(basePos.z) < 6 ? -1 : 1);
        Vector3 armOffset = Mathf.Abs(basePos.z) < 6 ? new Vector3(0, 6.4f, armDir * 0.8f) : new Vector3(armDir * 0.8f, 6.4f, 0);
        Box("Arm", l, armOffset * 0.5f + new Vector3(0, 6.4f, 0) * 0.5f, new Vector3(Mathf.Abs(armOffset.x) + 0.1f, 0.08f, Mathf.Abs(armOffset.z) + 0.1f), MatMetal());
        Box("Head", l, armOffset, new Vector3(0.35f, 0.15f, 0.35f), lit ? MatSodiumHead() : MatMetal());
        if (!lit) return;
        var lightGO = new GameObject("SodiumLight");
        lightGO.transform.SetParent(l.transform, false);
        lightGO.transform.localPosition = armOffset + new Vector3(0, -0.15f, 0);
        lightGO.transform.rotation = Quaternion.Euler(90, 0, 0);
        var s = lightGO.AddComponent<Light>();
        s.type = LightType.Spot;
        s.spotAngle = 110;
        s.range = 16;
        s.color = Sodium;
        s.intensity = 3.4f * strength;
        s.shadows = LightShadows.Soft;
    }

    static void Palm(GameObject parent, Vector3 basePos)
    {
        var p = Group("Palm", parent, basePos);
        Cylinder("Trunk", p, new Vector3(0, 3, 0), 0.18f, 6, MatTrunk());
        for (int f = 0; f < 5; f++)
        {
            var frond = Box("Frond_" + f, p, new Vector3(0, 6.1f, 0), new Vector3(2.6f, 0.06f, 0.5f), MatCrown());
            frond.transform.rotation = Quaternion.Euler(18, f * 72, 0);
        }
    }

    static void Car(GameObject parent, string name, Vector3 pos, float yRot, Material m)
    {
        var c = Group(name, parent, pos);
        Box("Body", c, new Vector3(0, 0.55f, 0), new Vector3(4.2f, 0.9f, 1.7f), m);
        Box("Cabin", c, new Vector3(-0.2f, 1.25f, 0), new Vector3(2.2f, 0.5f, 1.6f), m);
        c.transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    // ============================================================== primitives

    static GameObject Group(string name, GameObject parent = null, Vector3 pos = default)
    {
        var g = new GameObject(name);
        if (parent != null)
        {
            // position is local to the parent so nested groups place correctly
            g.transform.SetParent(parent.transform);
            g.transform.localPosition = pos;
        }
        else
        {
            g.transform.position = pos;
        }
        return g;
    }

    static GameObject Box(string name, GameObject parent, Vector3 localPos, Vector3 size, Material m)
        => Prim(PrimitiveType.Cube, name, parent, localPos, size, m);

    // GameObject.CreatePrimitive adds an axis-aligned BoxCollider. After rotating a
    // door or gate, that collider's world AABB grows and can block a doorway or
    // corridor. Replace it with a convex MeshCollider so the collision volume
    // matches the rotated visual mesh.
    static void FixRotatedDoorCollider(GameObject door)
    {
        Object.DestroyImmediate(door.GetComponent<BoxCollider>());
        var mc = door.AddComponent<MeshCollider>();
        mc.convex = true;
    }

    static GameObject Sphere(string name, GameObject parent, Vector3 localPos, Vector3 size, Material m)
        => Prim(PrimitiveType.Sphere, name, parent, localPos, size, m);

    static GameObject Cylinder(string name, GameObject parent, Vector3 localPos, float radius, float height, Material m)
        => Prim(PrimitiveType.Cylinder, name, parent, localPos, new Vector3(radius * 2, height / 2f, radius * 2), m);

    static GameObject Prim(PrimitiveType t, string name, GameObject parent, Vector3 localPos, Vector3 scale, Material m)
    {
        var go = GameObject.CreatePrimitive(t);
        go.name = name;
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        return go;
    }

    static void PointLight(GameObject parent, string name, Vector3 localPos, Color color, float intensity, float range)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPos;
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.None;
    }

    // =============================================================== materials
    // Palette hexes come from WorldDesign.md section 5.

    static readonly Color WarmBulb = new Color(1f, 0.72f, 0.42f);
    static readonly Color Sodium = new Color(1f, 0.63f, 0.28f);
    static readonly Color CoolFluor = new Color(0.88f, 0.95f, 0.93f);

    static Material MatDust() => Mat("Dust", "#C7B594");
    static Material MatAsphalt() => Mat("Asphalt", "#4A4843");
    static Material MatAsphaltOld() => Mat("AsphaltOld", "#5C574E");
    static Material MatConcrete() => Mat("Concrete", "#8A857C");
    static Material MatBlock() => Mat("CinderBlock", "#9E9484");
    static Material MatRenderCream() => Mat("RenderCream", "#D8C9A8");
    static Material MatRenderOchre() => Mat("RenderOchre", "#C2A171");
    static Material MatRenderGreen() => Mat("RenderGreen", "#A8B396");
    static Material MatDoorGreen() => Mat("DoorGreen", "#4E5D4A", 0.4f, 0.35f);
    static Material MatDoorBrown() => Mat("DoorBrown", "#6B4A38", 0.4f, 0.35f);
    static Material MatDoorBlue() => Mat("DoorBlue", "#3E566B", 0.4f, 0.35f);
    static Material MatDoorLocked() => Mat("DoorLocked", "#43413C", 0.5f, 0.3f);
    static Material MatRust() => Mat("Rust", "#7A4326", 0.3f, 0.2f);
    static Material MatMetal() => Mat("MetalGrey", "#6E6E6A", 0.6f, 0.4f);
    static Material MatZinc() => Mat("Zinc", "#8F918D", 0.7f, 0.45f);
    static Material MatTank() => Mat("TankBlack", "#2B2B29", 0.1f, 0.3f);
    static Material MatDish() => Mat("DishGrey", "#B9BBB6", 0.4f, 0.4f);
    static Material MatSchool() => Mat("SchoolOchre", "#C9B68C");
    static Material MatSchoolWall() => Mat("SchoolWall", "#BCA97F");
    static Material MatRuinRender() => Mat("RuinRender", "#A99C85");
    static Material MatRuinBlock() => Mat("RuinBlock", "#8C8375");
    static Material MatRuinInner() => Mat("RuinInner", "#75695C");
    static Material MatDirtyTile() => Mat("DirtyTile", "#6E655A");
    static Material MatTileFloor() => Mat("Terrazzo", "#A79F8F", 0f, 0.5f);
    static Material MatWood() => Mat("OldWood", "#5E4A33");
    static Material MatMattress() => Mat("Mattress", "#B0A07E");
    static Material MatFridge() => Mat("Fridge", "#D8DAD2", 0.2f, 0.5f);
    static Material MatFridgeDead() => Mat("FridgeDead", "#9A968C", 0.1f, 0.2f);
    static Material MatCounter() => Mat("Counter", "#8B7355", 0f, 0.4f);
    static Material MatShelf() => Mat("Shelf", "#7D7468");
    static Material MatBread() => Mat("BreadBags", "#D9C48E", 0f, 0.55f);
    static Material MatCrate() => Mat("Crate", "#B03A2E");
    static Material MatPlasticChair() => Mat("PlasticChair", "#DDD8CC", 0f, 0.5f);
    static Material MatCardboard() => Mat("Cardboard", "#A98F63");
    static Material MatTarp() => Mat("Tarp", "#7F8894");
    static Material MatCarBlue() => Mat("CarBlue", "#3D4F63", 0.5f, 0.5f);
    static Material MatCarWhite() => Mat("CarWhite", "#C9C9C2", 0.5f, 0.5f);
    static Material MatTrunk() => Mat("Trunk", "#6B5A43");
    static Material MatCrown() => Mat("Crown", "#3E4A33");
    static Material MatDeadCrown() => Mat("DeadCrown", "#5C5648");
    static Material MatPlant() => Mat("Plant", "#4C6B3C");
    static Material MatSoot() => Mat("Soot", "#211F1C");
    static Material MatVoid() => Mat("Void", "#0A0A0B");
    static Material MatMirror() => Mat("Mirror", "#A9B4B8", 0.9f, 0.9f);
    static Material MatSign() => Mat("ShopSign", "#C24E3A");
    static Material MatCarpet() => Mat("Carpet", "#7A3A30");
    static Material MatPaleGhost() => Mat("PaleGhost", "#E8DFC8");

    static Material MatWindowDark() => MatEmissive("WindowDark", "#101318", "#000000", 0f);
    static Material MatWindowWarm() => MatEmissive("WindowWarm", "#33291A", "#E8A84C", 1.6f);
    static Material MatFluorescent() => MatEmissive("Fluorescent", "#DDE5E2", "#E9F0EE", 2.2f);
    static Material MatSodiumHead() => MatEmissive("SodiumHead", "#5A4A33", "#FFA84C", 2.0f);
    static Material MatMinaretGreen() => MatEmissive("MinaretGreen", "#1E3A28", "#3FDB77", 2.0f);

    static Material Mat(string name, string hex, float metallic = 0f, float smooth = 0.15f)
    {
        string path = MatFolder + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", Hex(hex));
        m.SetFloat("_Metallic", metallic);
        m.SetFloat("_Smoothness", smooth);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static Material MatEmissive(string name, string baseHex, string emitHex, float strength)
    {
        string path = MatFolder + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", Hex(baseHex));
        m.SetFloat("_Smoothness", 0.4f);
        if (strength > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", Hex(emitHex) * strength);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
        if (!AssetDatabase.IsValidFolder("Assets/Art/Materials")) AssetDatabase.CreateFolder("Assets/Art", "Materials");
        if (!AssetDatabase.IsValidFolder(MatFolder)) AssetDatabase.CreateFolder("Assets/Art/Materials", "Greybox");
    }
}
