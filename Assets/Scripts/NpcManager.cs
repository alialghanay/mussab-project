using System.Collections.Generic;
using UnityEngine;

/// Keeps the streets populated during the day and empty at night.
/// NPCs spawn gradually after dawn and despawn after dusk (only when far
/// from the player, so nobody vanishes in front of the camera).
public class NpcManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform waypointsRoot;
    public int maxNpcs = 10;
    public float dayStart = 7f;
    public float dayEnd = 19f;

    DayNightCycle cycle;
    Transform player;
    Transform[] points;
    readonly List<GameObject> npcs = new List<GameObject>();
    float nextSpawnAt;

    void Start()
    {
        cycle = FindAnyObjectByType<DayNightCycle>();
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) player = playerGO.transform;

        if (waypointsRoot != null)
        {
            var list = new List<Transform>();
            foreach (Transform t in waypointsRoot) list.Add(t);
            points = list.ToArray();
        }
    }

    void Update()
    {
        if (cycle == null || npcPrefab == null || points == null || points.Length == 0)
            return;

        bool daytime = cycle.Hour >= dayStart && cycle.Hour < dayEnd;
        npcs.RemoveAll(n => n == null);

        if (daytime)
        {
            if (npcs.Count < maxNpcs && Time.time >= nextSpawnAt)
            {
                Spawn();
                nextSpawnAt = Time.time + Random.Range(1.5f, 4f);
            }
        }
        else
        {
            // despawn out of sight
            for (int i = npcs.Count - 1; i >= 0; i--)
            {
                bool farFromPlayer = player == null ||
                    Vector3.Distance(npcs[i].transform.position, player.position) > 18f;
                if (farFromPlayer)
                {
                    Destroy(npcs[i]);
                    npcs.RemoveAt(i);
                }
            }
        }
    }

    void Spawn()
    {
        var point = points[Random.Range(0, points.Length)];
        // don't pop into existence right next to the player
        if (player != null && Vector3.Distance(point.position, player.position) < 14f)
            return;

        var npc = Instantiate(npcPrefab, point.position, Quaternion.Euler(0, Random.Range(0, 360f), 0));
        npc.SetActive(true);
        var wanderer = npc.GetComponent<NpcWanderer>();
        if (wanderer != null) wanderer.waypoints = points;
        npcs.Add(npc);
    }
}
