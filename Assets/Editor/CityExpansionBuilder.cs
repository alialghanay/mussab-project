using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// Doubles the neighborhood: north + south districts, a walled park, grass,
/// trees, rose beds, props, street lamps, NPC prefab/waypoints and city-life
/// systems. Everything lives under _CityExpansion (plus _StreetLights/
/// ExpansionLamps so the day/night cycle drives the new lamps too), so
/// rebuilding never touches the original neighborhood.
/// Layout constants assume the NeighborhoodWorldBuilder coordinates:
/// main road Z 0 (X -100..60), alley X 40 (Z -4..-75), school X -60..-10 Z 6..46.
public static class CityExpansionBuilder
{
    const string MatFolder = "Assets/Art/Materials/Greybox";
    const string PrefabFolder = "Assets/Art/Prefabs";
    const string GrassTexPath = "Assets/Fantasy Skybox FREE/Scenes/Textures (Terrain)/Brush_Grass_01.png";
    const string LawnTexPath = "Assets/Fantasy Skybox FREE/Scenes/Textures (Terrain)/Texture_Grass_Diffuse.png";
    const string BenchPrefabPath = "Assets/WoodenParkBench/Prefabs/WoodenParkBench.prefab";

    static System.Random rng;

    [MenuItem("Tools/Neighborhood/Build City Expansion")]
    public static void Build()
    {
        rng = new System.Random(42);
        EnsureFolders();

        // idempotent rebuild: remove previous expansion content only
        var old = GameObject.Find("_CityExpansion");
        if (old != null) Object.DestroyImmediate(old);
        var lights = GameObject.Find("_StreetLights");
        var oldLamps = lights != null ? lights.transform.Find("ExpansionLamps") : null;
        if (oldLamps != null) Object.DestroyImmediate(oldLamps.gameObject);

        var root = Group("_CityExpansion");
        var lampRoot = Group("ExpansionLamps", lights);

        BuildRoads(root);
        BuildNorthDistrict(root);
        BuildSouthDistrict(root);
        BuildPark(root);
        BuildStreetTrees(root);
        BuildGrass(root);
        BuildProps(root);
        BuildLamps(lampRoot);
        var npcPrefab = BuildNpcPrefab();
        BuildCityLife(root, npcPrefab);

        // let the day/night cycle pick up the new lamps
        var cycle = Object.FindAnyObjectByType<DayNightCycle>();
        if (cycle != null) { cycle.SendMessage("CacheLamps"); cycle.ApplyTimeOfDay(); }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(root.scene);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[CityExpansionBuilder] Expansion built: " + CountChildren(root.transform) + " objects.");
    }

    // ------------------------------------------------------------------ roads

    const string RoadTilePath = "Assets/StarsandShellsStudio/Prefabs/straightRoad.prefab";
    const float RoadTileLength = 28.18f; // native Z length of the straight tile

    static void BuildRoads(GameObject root)
    {
        var r = Group("Roads", root);

        // 3D Road Pack tiles replace the flat asphalt boxes. The original
        // main-road box (and its speed bump) are deactivated, not deleted,
        // so the base neighborhood stays untouched and restorable.
        var ground = GameObject.Find("_GroundAndRoads");
        if (ground != null)
            foreach (string n in new[] { "Road_Main_Asphalt", "Road_SpeedBump" })
            {
                var t = ground.transform.Find(n);
                if (t != null) t.gameObject.SetActive(false);
            }

        LayRoadRun(r, "Main", true, 0, -100, 60, 0.45f);
        LayRoadRun(r, "North", true, 52, -96, 56, 0.4f);
        LayRoadRun(r, "South", true, -88, -85, 56, 0.4f);
        LayRoadRun(r, "ConnNW", false, -64, 4, 49, 0.35f);
        LayRoadRun(r, "ConnNE", false, 52, 4, 49, 0.35f);
        LayRoadRun(r, "ConnSW", false, -72.5f, -85, -4, 0.35f);

        // the narrow alley extension stays greybox - the pack's tiles are
        // multi-lane and cannot shrink to a 4.5 m alley believably
        Box("Alley_Extension", r, new Vector3(40, 0.004f, -80), new Vector3(4.5f, 0.1f, 10), M("AsphaltOld"));
    }

    /// Lay a run of straight road tiles between 'from' and 'to' along X or Z.
    /// Tiles sink so the driving surface sits just above the dust ground;
    /// the last tile shifts back to end exactly at 'to'.
    static void LayRoadRun(GameObject parent, string name, bool alongX, float fixedCoord, float from, float to, float s)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RoadTilePath);
        if (prefab == null) { Debug.LogWarning("[CityExpansionBuilder] Road tile prefab missing"); return; }

        float tileLen = RoadTileLength * s;
        // the tile's asphalt plane sits at its pivot; only sidewalks rise above
        float y = 0.02f;
        int count = Mathf.CeilToInt((to - from) / tileLen);
        for (int i = 0; i < count; i++)
        {
            float center = Mathf.Min(from + tileLen * (i + 0.5f), to - tileLen * 0.5f);
            var t = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            t.name = name + "_Tile_" + i;
            t.transform.SetParent(parent.transform);
            t.transform.position = alongX ? new Vector3(center, y, fixedCoord) : new Vector3(fixedCoord, y, center);
            t.transform.rotation = Quaternion.Euler(0, alongX ? 90 : 0, 0);
            t.transform.localScale = Vector3.one * s;
            foreach (var mf in t.GetComponentsInChildren<MeshFilter>())
                if (mf.GetComponent<Collider>() == null) mf.gameObject.AddComponent<MeshCollider>();
        }
    }

    // -------------------------------------------------------------- districts

    static void BuildNorthDistrict(GameObject root)
    {
        var d = Group("NorthDistrict", root);
        int i = 100;
        // north side of the north street, fronts facing south
        float[] xsN = { -92, -78.5f, -65, -51.5f, -38, -24.5f, -11, 16, 43 };
        foreach (float x in xsN)
        {
            HouseEx(d, "NorthHouseN_" + i, new Vector3(x, 0, 60), 8, 9.5f, i, 90, +1);
            i++;
        }
        // courtyard walls filling two of the gaps
        Box("NCourtyardWall_A", d, new Vector3(2.5f, 1.15f, 60), new Vector3(9, 2.3f, 0.4f), M("RenderOchre"));
        Box("NCourtyardWall_B", d, new Vector3(29.5f, 1.15f, 60), new Vector3(9, 2.3f, 0.4f), M("RenderGreen"));

        // south side, east of the school (fills the old walled void)
        float[] xsS = { 0, 13.5f, 27, 41 };
        foreach (float x in xsS)
        {
            HouseEx(d, "NorthHouseS_" + i, new Vector3(x, 0, 43.5f), 8, 9.5f, i, 90, -1);
            i++;
        }
    }

    static void BuildSouthDistrict(GameObject root)
    {
        var d = Group("SouthDistrict", root);
        int i = 130;
        // south side of the south street, fronts facing north
        float[] xsS = { -80, -66.5f, -53, -39.5f, -26, 1, 14.5f, 28, 41.5f };
        foreach (float x in xsS)
        {
            HouseEx(d, "SouthHouseS_" + i, new Vector3(x, 0, -95.5f), 8, 9.5f, i, 90, -1);
            i++;
        }
        // north side (west stretch only - abandoned house / branch houses own the east)
        float[] xsN = { -66.5f, -53, -39.5f, -26 };
        foreach (float x in xsN)
        {
            HouseEx(d, "SouthHouseN_" + i, new Vector3(x, 0, -81), 8, 9.5f, i, 90, +1);
            i++;
        }
        Box("SCourtyardWall", d, new Vector3(-12.25f, 1.15f, -95.5f), new Vector3(9, 2.3f, 0.4f), M("RenderCream"));
    }

    // ------------------------------------------------------------------- park

    static void BuildPark(GameObject root)
    {
        var p = Group("Park", root, new Vector3(-82, 0, 19));
        // lawn (X -96..-68, Z 8..30 world)
        var lawn = Box("Lawn", p, new Vector3(0, 0.03f, 0), new Vector3(28, 0.06f, 22), MatLawn());
        lawn.isStatic = true;

        // low sittable boundary wall with south + east gate gaps
        Material w = M("Concrete");
        Box("Wall_N", p, new Vector3(0, 0.25f, 11), new Vector3(28, 0.5f, 0.3f), w);
        Box("Wall_W", p, new Vector3(-14, 0.25f, 0), new Vector3(0.3f, 0.5f, 22), w);
        Box("Wall_S_A", p, new Vector3(-8.25f, 0.25f, -11), new Vector3(11.5f, 0.5f, 0.3f), w);
        Box("Wall_S_B", p, new Vector3(8.25f, 0.25f, -11), new Vector3(11.5f, 0.5f, 0.3f), w);
        Box("Wall_E_A", p, new Vector3(14, 0.25f, 6.25f), new Vector3(0.3f, 0.5f, 9.5f), w);
        Box("Wall_E_B", p, new Vector3(14, 0.25f, -6.25f), new Vector3(0.3f, 0.5f, 9.5f), w);

        // dust paths through the gates to the middle
        Box("Path_NS", p, new Vector3(0, 0.055f, -5.5f), new Vector3(2.4f, 0.05f, 11), M("Dust"));
        Box("Path_EW", p, new Vector3(7, 0.055f, 0), new Vector3(14, 0.05f, 2.4f), M("Dust"));

        // trees
        Tree(p, new Vector3(-9, 0, 6), 1.15f, false);
        Tree(p, new Vector3(-4, 0, -6.5f), 0.95f, false);
        Tree(p, new Vector3(5, 0, 7.5f), 1.3f, false);
        Tree(p, new Vector3(10.5f, 0, -7), 1.0f, false);
        Tree(p, new Vector3(-11.5f, 0, -3), 0.8f, true);
        Palm(p, new Vector3(12, 0, 3));

        // rose beds
        RoseBed(p, new Vector3(-6, 0, 1.8f));
        RoseBed(p, new Vector3(3, 0, -3.2f));
        RoseBed(p, new Vector3(-10, 0, 8.5f));
        RoseBed(p, new Vector3(9, 0, 9));

        // benches
        Bench(p, new Vector3(-2.2f, 0, -1.6f), 0);
        Bench(p, new Vector3(2.4f, 0, 3.2f), 180);
        Bench(p, new Vector3(-8.5f, 0, -8.5f), 90);
    }

    static void BuildStreetTrees(GameObject root)
    {
        var t = Group("StreetTrees", root);
        Tree(t, new Vector3(-70, 0, 56.5f), 1.0f, false);
        Tree(t, new Vector3(-30, 0, 56.5f), 1.1f, false);
        Tree(t, new Vector3(8, 0, 56.5f), 0.9f, true);
        Tree(t, new Vector3(-60, 0, -84.2f), 1.05f, false);
        Tree(t, new Vector3(-18, 0, -84.2f), 0.9f, false);
        Palm(t, new Vector3(34, 0, -84.5f));
        Palm(t, new Vector3(-88, 0, 5.5f));
    }

    // ------------------------------------------------------------- vegetation

    static void BuildGrass(GameObject root)
    {
        var g = Group("Grass", root);
        Material tuftMat = MatGrassTuft();

        // dense in the park
        Scatter(g, tuftMat, 70, new Vector3(-95, 0, 9), new Vector3(-69, 0, 29));
        // north street edges
        Scatter(g, tuftMat, 25, new Vector3(-94, 0, 55.3f), new Vector3(54, 0, 56.6f));
        Scatter(g, tuftMat, 15, new Vector3(-94, 0, 47.6f), new Vector3(54, 0, 48.8f));
        // south street edges
        Scatter(g, tuftMat, 25, new Vector3(-83, 0, -84.8f), new Vector3(54, 0, -83.6f));
        Scatter(g, tuftMat, 15, new Vector3(-83, 0, -92.4f), new Vector3(54, 0, -91.2f));
        // scruffy corners of the old neighborhood
        Scatter(g, tuftMat, 12, new Vector3(-66, 0, -9), new Vector3(-58, 0, -5));
        Scatter(g, tuftMat, 10, new Vector3(43, 0, -6), new Vector3(50, 0, -4.5f));
        Scatter(g, tuftMat, 12, new Vector3(3, 0, -71), new Vector3(36, 0, -70));
    }

    static void Scatter(GameObject parent, Material mat, int count, Vector3 min, Vector3 max)
    {
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector3(Rand(min.x, max.x), 0, Rand(min.z, max.z));
            GrassTuft(parent, pos, mat);
        }
    }

    static void GrassTuft(GameObject parent, Vector3 pos, Material mat)
    {
        var t = Group("GrassTuft", parent, pos);
        float s = Rand(0.6f, 1.25f);
        for (int q = 0; q < 2; q++)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "Blade_" + q;
            Object.DestroyImmediate(quad.GetComponent<MeshCollider>());
            quad.transform.SetParent(t.transform, false);
            quad.transform.localPosition = new Vector3(0, 0.24f * s, 0);
            quad.transform.localScale = new Vector3(0.95f * s, 0.5f * s, 1f);
            quad.transform.localRotation = Quaternion.Euler(0, q * 90f, 0);
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        t.transform.rotation = Quaternion.Euler(0, Rand(0, 360f), 0);
    }

    static void Tree(GameObject parent, Vector3 pos, float scale, bool dead)
    {
        var t = Group(dead ? "DeadTree" : "Tree", parent, pos);
        Material crown = dead ? M("DeadCrown") : M("Crown");
        float h = 2.8f * scale;
        Cylinder("Trunk", t, new Vector3(0, h / 2f, 0), 0.18f * scale, h, M("Trunk"));
        var c1 = Sphere("Crown_A", t, new Vector3(0, h + 0.9f * scale, 0), Vector3.one * 2.7f * scale, crown);
        var c2 = Sphere("Crown_B", t, new Vector3(0.8f * scale, h + 0.4f * scale, 0.3f * scale), Vector3.one * 1.8f * scale, crown);
        var c3 = Sphere("Crown_C", t, new Vector3(-0.7f * scale, h + 0.5f * scale, -0.4f * scale), Vector3.one * 1.6f * scale, crown);
        Object.DestroyImmediate(c1.GetComponent<Collider>());
        Object.DestroyImmediate(c2.GetComponent<Collider>());
        Object.DestroyImmediate(c3.GetComponent<Collider>());
        if (dead) { c1.transform.localScale *= 0.55f; c2.SetActive(false); }
    }

    static void Palm(GameObject parent, Vector3 basePos)
    {
        var p = Group("Palm", parent, basePos);
        Cylinder("Trunk", p, new Vector3(0, 3, 0), 0.18f, 6, M("Trunk"));
        for (int f = 0; f < 5; f++)
        {
            var frond = Box("Frond_" + f, p, new Vector3(0, 6.1f, 0), new Vector3(2.6f, 0.06f, 0.5f), M("Crown"));
            Object.DestroyImmediate(frond.GetComponent<Collider>());
            frond.transform.rotation = Quaternion.Euler(18, f * 72, 0);
        }
    }

    static void RoseBed(GameObject parent, Vector3 pos)
    {
        var bed = Group("RoseBed", parent, pos);
        Box("Soil", bed, new Vector3(0, 0.09f, 0), new Vector3(2.6f, 0.18f, 1.4f), MatColor("SoilDark", "#4A3C2E"));
        for (int i = 0; i < 6; i++)
        {
            var bp = new Vector3(Rand(-1f, 1f), 0.18f, Rand(-0.45f, 0.45f));
            var bush = Sphere("Bush_" + i, bed, bp + new Vector3(0, 0.22f, 0), new Vector3(0.5f, 0.44f, 0.5f), M("Plant"));
            Object.DestroyImmediate(bush.GetComponent<Collider>());
            Material rose = rng.NextDouble() < 0.5 ? MatColor("RoseRed", "#B23A50") : MatColor("RosePink", "#C97F94");
            for (int r = 0; r < 5; r++)
            {
                var rosePos = bp + new Vector3(Rand(-0.18f, 0.18f), 0.42f + Rand(0f, 0.08f), Rand(-0.18f, 0.18f));
                var flower = Sphere("Rose_" + i + "_" + r, bed, rosePos, Vector3.one * 0.09f, rose);
                Object.DestroyImmediate(flower.GetComponent<Collider>());
            }
        }
    }

    static void Bench(GameObject parent, Vector3 localPos, float yRot)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BenchPrefabPath);
        if (prefab == null)
        {
            // fallback: simple greybox bench
            var b = Group("Bench", parent, localPos);
            Box("Seat", b, new Vector3(0, 0.42f, 0), new Vector3(1.6f, 0.08f, 0.5f), M("OldWood"));
            Box("Back", b, new Vector3(0, 0.75f, -0.22f), new Vector3(1.6f, 0.5f, 0.06f), M("OldWood"));
            Box("Leg_A", b, new Vector3(-0.65f, 0.2f, 0), new Vector3(0.08f, 0.4f, 0.45f), M("MetalGrey"));
            Box("Leg_B", b, new Vector3(0.65f, 0.2f, 0), new Vector3(0.08f, 0.4f, 0.45f), M("MetalGrey"));
            b.transform.localRotation = Quaternion.Euler(0, yRot, 0);
            return;
        }

        ConvertToUrp(prefab);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        inst.transform.SetParent(parent.transform);
        inst.transform.localPosition = localPos;
        inst.transform.localRotation = Quaternion.Euler(0, yRot, 0);

        // normalize odd import scales to a ~1.6 m bench sitting on the ground
        var rends = inst.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            float footprint = Mathf.Max(b.size.x, b.size.z);
            if (footprint > 0.01f && (footprint < 1f || footprint > 2.6f))
                inst.transform.localScale *= 1.6f / footprint;
            b = rends[0].bounds; foreach (var r in rends) b.Encapsulate(r.bounds);
            inst.transform.position += Vector3.up * -b.min.y;
        }
    }

    // ------------------------------------------------------------------ props

    static void BuildProps(GameObject root)
    {
        var p = Group("Props", root);
        CarBox(p, "Car_North_A", new Vector3(-56, 0, 49.6f), 0, M("CarBlue"));
        CarBox(p, "Car_North_B", new Vector3(20, 0, 54.4f), 180, M("CarWhite"));
        CarBox(p, "Car_South_A", new Vector3(-44, 0, -90.4f), 0, M("Tarp"));
        CarBox(p, "Car_South_B", new Vector3(24, 0, -85.6f), 180, M("CarWhite"));
        CarBox(p, "Car_Park", new Vector3(-71, 0, 4.2f), 90, M("CarBlue"));

        Box("Dumpster_N", p, new Vector3(-46, 0.7f, 55.8f), new Vector3(2.2f, 1.4f, 1.2f), M("DoorGreen"));
        Box("Dumpster_S", p, new Vector3(6, 0.7f, -85.6f), new Vector3(2.2f, 1.4f, 1.2f), M("DoorGreen"));

        var crates = Group("CrateCorner", p, new Vector3(-75.5f, 0, -12));
        Box("Crate_A", crates, new Vector3(0, 0.35f, 0), new Vector3(0.7f, 0.7f, 0.7f), M("Crate"));
        Box("Crate_B", crates, new Vector3(0.8f, 0.3f, 0.2f), new Vector3(0.6f, 0.6f, 0.6f), M("Crate"));
        Box("Cardboard", crates, new Vector3(0.3f, 0.75f, 0.1f), new Vector3(0.9f, 0.1f, 0.7f), M("Cardboard"));

        var trash = Group("TrashCorner_S", p, new Vector3(38, 0, -84.5f));
        Box("TrashBag_A", trash, new Vector3(0, 0.25f, 0), new Vector3(0.6f, 0.5f, 0.6f), M("TankBlack"));
        Box("TrashBag_B", trash, new Vector3(0.5f, 0.2f, 0.2f), new Vector3(0.5f, 0.4f, 0.5f), M("TankBlack"));
    }

    static void CarBox(GameObject parent, string name, Vector3 pos, float yRot, Material m)
    {
        var c = Group(name, parent, pos);
        Box("Body", c, new Vector3(0, 0.55f, 0), new Vector3(4.2f, 0.9f, 1.7f), m);
        Box("Cabin", c, new Vector3(-0.2f, 1.25f, 0), new Vector3(2.2f, 0.5f, 1.6f), m);
        c.transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    // ------------------------------------------------------------------ lamps

    static void BuildLamps(GameObject lampRoot)
    {
        // arms lean toward the street they serve
        Lamp(lampRoot, "XLamp_N1", new Vector3(-80, 0, 55.8f), new Vector3(0, 0, -0.9f));
        Lamp(lampRoot, "XLamp_N2", new Vector3(-40, 0, 48.2f), new Vector3(0, 0, 0.9f));
        Lamp(lampRoot, "XLamp_N3", new Vector3(0, 0, 55.8f), new Vector3(0, 0, -0.9f));
        Lamp(lampRoot, "XLamp_N4", new Vector3(40, 0, 48.2f), new Vector3(0, 0, 0.9f));
        Lamp(lampRoot, "XLamp_S1", new Vector3(-64, 0, -84.8f), new Vector3(0, 0, -0.9f));
        Lamp(lampRoot, "XLamp_S2", new Vector3(-24, 0, -91.2f), new Vector3(0, 0, 0.9f));
        Lamp(lampRoot, "XLamp_S3", new Vector3(16, 0, -84.8f), new Vector3(0, 0, -0.9f));
        Lamp(lampRoot, "XLamp_ConnNW", new Vector3(-66.9f, 0, 26), new Vector3(0.9f, 0, 0));
        Lamp(lampRoot, "XLamp_ConnSW", new Vector3(-75.2f, 0, -44), new Vector3(0.9f, 0, 0));
        Lamp(lampRoot, "XLamp_Park1", new Vector3(-88, 0, 13), new Vector3(0.7f, 0, 0.5f));
        Lamp(lampRoot, "XLamp_Park2", new Vector3(-76, 0, 25), new Vector3(-0.7f, 0, -0.5f));
    }

    static void Lamp(GameObject parent, string name, Vector3 basePos, Vector3 armDir)
    {
        var l = Group(name, parent, basePos);
        Cylinder("Pole", l, new Vector3(0, 3.25f, 0), 0.09f, 6.5f, M("MetalGrey"));
        Vector3 armOffset = armDir + new Vector3(0, 6.4f, 0);
        Box("Arm", l, Vector3.Lerp(new Vector3(0, 6.4f, 0), armOffset, 0.5f),
            new Vector3(Mathf.Abs(armDir.x) + 0.1f, 0.08f, Mathf.Abs(armDir.z) + 0.1f), M("MetalGrey"));
        Box("Head", l, armOffset, new Vector3(0.35f, 0.15f, 0.35f), M("SodiumHead"));
        var lightGO = new GameObject("SodiumLight");
        lightGO.transform.SetParent(l.transform, false);
        lightGO.transform.localPosition = armOffset + new Vector3(0, -0.15f, 0);
        lightGO.transform.rotation = Quaternion.Euler(90, 0, 0);
        var s = lightGO.AddComponent<Light>();
        s.type = LightType.Spot;
        s.spotAngle = 110;
        s.range = 16;
        s.color = new Color(1f, 0.63f, 0.28f);
        s.intensity = 3.4f;
        s.shadows = LightShadows.Soft;
    }

    // -------------------------------------------------------------- NPC setup

    /// Articulated humanoid: legs and arms hang from hip/shoulder pivot
    /// groups so NpcWanderer can swing them while walking.
    static GameObject BuildNpcPrefab()
    {
        var fig = new GameObject("NpcFigure");
        Material skin = MatColor("NpcSkin", "#B08D6A");
        Material shirt = MatColor("NpcShirt", "#C9BFA8");
        Material trousers = MatColor("NpcTrousers", "#3A3B40");
        Material shoes = MatColor("NpcShoes", "#26221E");
        Material hair = MatColor("NpcHair", "#241C14");

        var hipL = Group("Hip_L", fig, new Vector3(-0.10f, 0.92f, 0));
        Box("Thigh", hipL, new Vector3(0, -0.24f, 0), new Vector3(0.15f, 0.46f, 0.17f), trousers);
        Box("Shin", hipL, new Vector3(0, -0.66f, 0.01f), new Vector3(0.13f, 0.42f, 0.15f), trousers);
        Box("Shoe", hipL, new Vector3(0, -0.885f, 0.05f), new Vector3(0.13f, 0.07f, 0.27f), shoes);
        var hipR = Group("Hip_R", fig, new Vector3(0.10f, 0.92f, 0));
        Box("Thigh", hipR, new Vector3(0, -0.24f, 0), new Vector3(0.15f, 0.46f, 0.17f), trousers);
        Box("Shin", hipR, new Vector3(0, -0.66f, 0.01f), new Vector3(0.13f, 0.42f, 0.15f), trousers);
        Box("Shoe", hipR, new Vector3(0, -0.885f, 0.05f), new Vector3(0.13f, 0.07f, 0.27f), shoes);

        Box("Waist", fig, new Vector3(0, 0.925f, 0), new Vector3(0.36f, 0.11f, 0.22f), trousers);
        Box("Torso", fig, new Vector3(0, 1.19f, 0), new Vector3(0.40f, 0.56f, 0.24f), shirt);
        Box("Shoulders", fig, new Vector3(0, 1.44f, 0), new Vector3(0.46f, 0.10f, 0.25f), shirt);

        var shoulderL = Group("Shoulder_L", fig, new Vector3(-0.265f, 1.42f, 0));
        Box("UpperArm", shoulderL, new Vector3(0, -0.17f, 0), new Vector3(0.10f, 0.34f, 0.11f), shirt);
        Box("Forearm", shoulderL, new Vector3(0, -0.46f, 0.02f), new Vector3(0.09f, 0.26f, 0.10f), skin);
        var shoulderR = Group("Shoulder_R", fig, new Vector3(0.265f, 1.42f, 0));
        Box("UpperArm", shoulderR, new Vector3(0, -0.17f, 0), new Vector3(0.10f, 0.34f, 0.11f), shirt);
        Box("Forearm", shoulderR, new Vector3(0, -0.46f, 0.02f), new Vector3(0.09f, 0.26f, 0.10f), skin);

        Cylinder("Neck", fig, new Vector3(0, 1.51f, 0), 0.055f, 0.10f, skin);
        Sphere("Head", fig, new Vector3(0, 1.63f, 0), new Vector3(0.22f, 0.26f, 0.24f), skin);
        Sphere("Hair", fig, new Vector3(0, 1.69f, -0.02f), new Vector3(0.225f, 0.19f, 0.245f), hair);

        foreach (var c in fig.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);

        var capsule = fig.AddComponent<CapsuleCollider>();
        capsule.center = new Vector3(0, 0.88f, 0);
        capsule.height = 1.76f;
        capsule.radius = 0.24f;
        fig.AddComponent<NpcWanderer>();

        string path = PrefabFolder + "/NpcFigure.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(fig, path);
        Object.DestroyImmediate(fig);
        return prefab;
    }

    static void BuildCityLife(GameObject root, GameObject npcPrefab)
    {
        var life = Group("_CityLife", root);

        var wp = Group("NpcWaypoints", life);
        // main road sidewalks
        for (float x = -90; x <= 50; x += 20) { Waypoint(wp, x, 5.6f); Waypoint(wp, x, -5.6f); }
        // alley
        for (float z = -10; z >= -70; z -= 15) Waypoint(wp, 38.6f, z);
        // north street
        for (float x = -88; x <= 48; x += 17) Waypoint(wp, x, 47.3f);
        for (float x = -80; x <= 40; x += 20) Waypoint(wp, x, 56.7f);
        // south street
        for (float x = -78; x <= 44; x += 17.5f) Waypoint(wp, x, -83.9f);
        for (float x = -70; x <= 40; x += 22) Waypoint(wp, x, -92.1f);
        // connectors
        Waypoint(wp, -64, 15); Waypoint(wp, -64, 38); Waypoint(wp, 52, 15); Waypoint(wp, 52, 38);
        Waypoint(wp, -72.5f, -25); Waypoint(wp, -72.5f, -60);
        // park
        Waypoint(wp, -82, 13.5f); Waypoint(wp, -82, 19); Waypoint(wp, -75, 19); Waypoint(wp, -88, 24);

        var mgr = life.AddComponent<NpcManager>();
        mgr.npcPrefab = npcPrefab;
        mgr.waypointsRoot = wp.transform;
        mgr.maxNpcs = 10;

    }

    static void Waypoint(GameObject parent, float x, float z)
    {
        var w = new GameObject("WP");
        w.transform.SetParent(parent.transform);
        w.transform.position = new Vector3(x, 0, z);
    }

    // ---------------------------------------------------------------- housing

    /// Same recipe as the original neighborhood houses, plus AC units, and
    /// rotatable so doors can face north/south streets. faceSign picks which
    /// local X face gets the door before rotation.
    static GameObject HouseEx(GameObject parent, string name, Vector3 pos, float depth, float width, int seed, float yRot, int faceSign)
    {
        var h = Group(name, parent, pos);
        bool unfinished = seed % 5 == 1;
        float height = unfinished ? 3.5f : (seed % 3 == 0 ? 3.5f : 6.5f);
        Material tone = seed % 3 == 0 ? M("RenderCream") : seed % 3 == 1 ? M("RenderOchre") : M("RenderGreen");

        Box("Body", h, new Vector3(0, height / 2f, 0), new Vector3(depth, height, width), tone);
        Material par = M("Concrete");
        Box("Parapet_N", h, new Vector3(0, height + 0.35f, width / 2f - 0.1f), new Vector3(depth, 0.7f, 0.2f), par);
        Box("Parapet_S", h, new Vector3(0, height + 0.35f, -width / 2f + 0.1f), new Vector3(depth, 0.7f, 0.2f), par);
        Box("Parapet_E", h, new Vector3(depth / 2f - 0.1f, height + 0.35f, 0), new Vector3(0.2f, 0.7f, width - 0.4f), par);
        Box("Parapet_W", h, new Vector3(-depth / 2f + 0.1f, height + 0.35f, 0), new Vector3(0.2f, 0.7f, width - 0.4f), par);

        if (unfinished)
        {
            Box("UnfinishedWall_A", h, new Vector3(0, height + 0.6f, width / 2f - 0.15f), new Vector3(depth, 1.2f, 0.25f), M("CinderBlock"));
            Box("UnfinishedWall_B", h, new Vector3(0, height + 0.6f, -width / 2f + 0.15f), new Vector3(depth, 1.2f, 0.25f), M("CinderBlock"));
        }
        if (seed % 2 == 0)
        {
            var t = Group("WaterTank", h, new Vector3(depth * 0.2f, height, -width * 0.25f));
            Box("Stand", t, new Vector3(0, 0.4f, 0), new Vector3(1.2f, 0.8f, 1.2f), M("MetalGrey"));
            Cylinder("Tank", t, new Vector3(0, 1.55f, 0), 0.75f, 1.5f, M("TankBlack"));
        }
        if (seed % 3 == 0)
        {
            var d = Group("SatDish", h, new Vector3(-depth * 0.2f, height + 0.1f, width * 0.25f));
            var dish = Cylinder("DishFace", d, new Vector3(0, 1.0f, 0), 0.55f, 0.06f, M("DishGrey"));
            dish.transform.localRotation = Quaternion.Euler(35, 20, 0);
            Box("Mast", d, new Vector3(0, 0.5f, 0), new Vector3(0.06f, 1.0f, 0.06f), M("MetalGrey"));
        }

        float face = faceSign * depth / 2f;
        Material door = seed % 3 == 0 ? M("DoorGreen") : seed % 3 == 1 ? M("DoorBrown") : M("DoorBlue");
        Box("Door", h, new Vector3(face * 1.008f, 1.05f, 0.8f), new Vector3(0.08f, 2.1f, 1.3f), door);
        Box("MeterBox", h, new Vector3(face * 1.01f, 1.7f, 1.9f), new Vector3(0.12f, 0.4f, 0.3f), M("MetalGrey"));
        Box("Window", h, new Vector3(face * 1.005f, 1.9f, -1.6f), new Vector3(0.06f, 1.0f, 1.1f), M("WindowDark"));
        if (seed % 2 == 1)
            Box("AC_Unit", h, new Vector3(face * 1.04f, 2.6f, -1.6f), new Vector3(0.35f, 0.5f, 0.9f), M("MetalGrey"));
        if (seed % 4 == 2)
        {
            Box("PlantTin", h, new Vector3(face * 1.06f, 0.2f, 1.6f), new Vector3(0.35f, 0.4f, 0.35f), M("MetalGrey"));
            var plant = Sphere("Plant", h, new Vector3(face * 1.06f, 0.55f, 1.6f), new Vector3(0.5f, 0.4f, 0.5f), M("Plant"));
            Object.DestroyImmediate(plant.GetComponent<Collider>());
        }

        h.transform.rotation = Quaternion.Euler(0, yRot, 0);
        return h;
    }

    // -------------------------------------------------------------- materials

    static Material M(string name)
    {
        var m = AssetDatabase.LoadAssetAtPath<Material>(MatFolder + "/" + name + ".mat");
        if (m == null) Debug.LogWarning("[CityExpansionBuilder] Missing material: " + name + " - using default");
        return m;
    }

    static Material MatColor(string name, string hex)
    {
        string path = MatFolder + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        ColorUtility.TryParseHtmlString(hex, out var c);
        m.SetColor("_BaseColor", c);
        m.SetFloat("_Smoothness", 0.15f);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static Material MatGrassTuft()
    {
        string path = MatFolder + "/GrassTuft.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(GrassTexPath);
        m.SetTexture("_BaseMap", tex);
        ColorUtility.TryParseHtmlString("#7A8A5A", out var tint);
        m.SetColor("_BaseColor", tint);
        m.SetFloat("_AlphaClip", 1f);
        m.EnableKeyword("_ALPHATEST_ON");
        m.SetFloat("_Cutoff", 0.45f);
        m.SetFloat("_Cull", 0f); // double sided
        m.SetFloat("_Smoothness", 0.05f);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static Material MatLawn()
    {
        string path = MatFolder + "/Lawn.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(LawnTexPath);
        m.SetTexture("_BaseMap", tex);
        m.SetTextureScale("_BaseMap", new Vector2(9f, 7f));
        ColorUtility.TryParseHtmlString("#9AA36F", out var tint);
        m.SetColor("_BaseColor", tint);
        m.SetFloat("_Smoothness", 0.05f);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    /// Swap Built-In Standard materials on an imported asset to URP Lit.
    static void ConvertToUrp(GameObject prefab)
    {
        foreach (var r in prefab.GetComponentsInChildren<Renderer>(true))
            foreach (var m in r.sharedMaterials)
            {
                if (m == null || !m.shader.name.Contains("Standard")) continue;
                var tex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
                var col = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
                m.shader = Shader.Find("Universal Render Pipeline/Lit");
                if (tex != null) m.SetTexture("_BaseMap", tex);
                m.SetColor("_BaseColor", col);
                EditorUtility.SetDirty(m);
            }
    }

    // -------------------------------------------------------------- utilities

    static float Rand(float min, float max) => min + (float)rng.NextDouble() * (max - min);

    static GameObject Group(string name, GameObject parent = null, Vector3 pos = default)
    {
        var g = new GameObject(name);
        if (parent != null) { g.transform.SetParent(parent.transform); g.transform.localPosition = pos; }
        else g.transform.position = pos;
        return g;
    }

    static GameObject Box(string name, GameObject parent, Vector3 localPos, Vector3 size, Material m)
        => Prim(PrimitiveType.Cube, name, parent, localPos, size, m);

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
        if (m != null) go.GetComponent<MeshRenderer>().sharedMaterial = m;
        return go;
    }

    static int CountChildren(Transform t)
    {
        int c = t.childCount;
        foreach (Transform child in t) c += CountChildren(child);
        return c;
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
        if (!AssetDatabase.IsValidFolder("Assets/Art/Materials")) AssetDatabase.CreateFolder("Assets/Art", "Materials");
        if (!AssetDatabase.IsValidFolder(MatFolder)) AssetDatabase.CreateFolder("Assets/Art/Materials", "Greybox");
        if (!AssetDatabase.IsValidFolder(PrefabFolder)) AssetDatabase.CreateFolder("Assets/Art", "Prefabs");
    }
}
