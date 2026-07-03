using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    public GameObject womanInBlackPrefab;
    public Transform spawnPoint;
    public float disappearDistance = 3f;
    public float maxDurationSeconds = 6f;

    GameObject spawned;
    Transform player;
    float timer;
    bool activeEncounter;

    public void TriggerEncounter()
    {
        if (womanInBlackPrefab == null || spawnPoint == null) return;

        spawned = Instantiate(womanInBlackPrefab, spawnPoint.position, spawnPoint.rotation);
        player = Camera.main.transform;
        activeEncounter = true;
        timer = maxDurationSeconds;

        var phone = player.GetComponentInParent<PhoneController>();
        if (phone != null) phone.Vibrate();
    }

    void Update()
    {
        if (!activeEncounter || spawned == null) return;

        if (player != null && Vector3.Distance(player.position, spawned.transform.position) < disappearDistance)
        {
            EndEncounter();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
            EndEncounter();
    }

    void EndEncounter()
    {
        activeEncounter = false;
        if (spawned != null)
            Destroy(spawned);
    }
}
