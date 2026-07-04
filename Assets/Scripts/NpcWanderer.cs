using UnityEngine;

/// Walks a humanoid figure between waypoints with idle pauses and simple
/// procedural animation: legs and arms swing from their hip/shoulder pivots
/// while moving. No navmesh: straight-line XZ movement with obstacle checks.
public class NpcWanderer : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 1.1f;
    public float turnSpeed = 5f;
    public float strideDegrees = 27f;

    Transform target;
    Transform hipL, hipR, shoulderL, shoulderR;
    float idleUntil;
    float walkPhase;

    static readonly Color[] ShirtPalette =
    {
        new Color(0.85f, 0.83f, 0.78f), // off-white
        new Color(0.79f, 0.75f, 0.66f), // beige
        new Color(0.56f, 0.55f, 0.52f), // grey
        new Color(0.36f, 0.44f, 0.54f), // faded blue
        new Color(0.35f, 0.39f, 0.31f), // olive
        new Color(0.45f, 0.32f, 0.28f), // brick
    };

    void Start()
    {
        speed *= Random.Range(0.85f, 1.2f);
        walkPhase = Random.value * 10f;

        hipL = transform.Find("Hip_L");
        hipR = transform.Find("Hip_R");
        shoulderL = transform.Find("Shoulder_L");
        shoulderR = transform.Find("Shoulder_R");

        // vary shirt color per NPC without instantiating materials
        var tint = ShirtPalette[Random.Range(0, ShirtPalette.Length)];
        var block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", tint);
        foreach (var r in GetComponentsInChildren<Renderer>())
            if (r.name == "Torso" || r.name == "Shoulders" || r.name == "UpperArm")
                r.SetPropertyBlock(block);

        PickNextTarget();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (Time.time < idleUntil || target == null)
        {
            Swing(0f); // settle limbs while idling
            return;
        }

        Vector3 to = target.position - transform.position;
        to.y = 0f;

        if (to.magnitude < 0.6f)
        {
            if (Random.value < 0.4f)
                idleUntil = Time.time + Random.Range(2f, 6f);
            PickNextTarget();
            return;
        }

        // re-route if a wall or house blocks the way
        Vector3 chest = transform.position + Vector3.up * 1.1f;
        if (Physics.Raycast(chest, transform.forward, out RaycastHit hit, 1.4f) &&
            !hit.collider.transform.IsChildOf(transform) &&
            hit.collider.GetComponentInParent<NpcWanderer>() == null)
        {
            PickNextTarget();
            return;
        }

        Quaternion look = Quaternion.LookRotation(to.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;

        // walk cycle: ~1.8 steps per meter, small vertical bob on step beats
        walkPhase += Time.deltaTime * speed * 5.6f;
        Swing(Mathf.Sin(walkPhase));

        // stand on whatever surface is underfoot (road tiles are raised)
        float groundY = 0f;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit ground, 4f))
            if (!ground.collider.transform.IsChildOf(transform))
                groundY = ground.point.y;
        Vector3 p = transform.position;
        p.y = groundY + Mathf.Abs(Mathf.Sin(walkPhase)) * 0.025f;
        transform.position = p;
    }

    void Swing(float s)
    {
        float leg = s * strideDegrees;
        float arm = -s * strideDegrees * 0.65f;
        if (hipL != null) hipL.localRotation = Damp(hipL.localRotation, leg);
        if (hipR != null) hipR.localRotation = Damp(hipR.localRotation, -leg);
        if (shoulderL != null) shoulderL.localRotation = Damp(shoulderL.localRotation, arm);
        if (shoulderR != null) shoulderR.localRotation = Damp(shoulderR.localRotation, -arm);
    }

    static Quaternion Damp(Quaternion current, float targetX)
        => Quaternion.Slerp(current, Quaternion.Euler(targetX, 0, 0), 14f * Time.deltaTime);

    void PickNextTarget()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;
        // prefer a reasonably close waypoint so NPCs stay in their area
        Transform best = null;
        for (int attempt = 0; attempt < 4; attempt++)
        {
            var c = waypoints[Random.Range(0, waypoints.Length)];
            if (c == target) continue;
            float d = Vector3.Distance(transform.position, c.position);
            if (d < 45f) { best = c; break; }
            if (best == null) best = c;
        }
        target = best;
    }
}
