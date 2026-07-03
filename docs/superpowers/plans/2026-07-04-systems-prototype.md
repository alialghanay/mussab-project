# Systems-First Prototype Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a playable greybox prototype in the existing Unity project that validates first-person movement, phone-as-tool mechanics, object interaction, audio zones, and one scripted encounter trigger.

**Architecture:** Keep systems decoupled: `PlayerController` handles movement/look; `PhoneController` owns the phone light and UI; `InteractionController` raycasts for interactables; `Interactable` scripts react to input; `AudioZone` volumes crossfade ambience; `GameDirector` sequences the encounter. The existing `NeighborhoodWorldBuilder` editor tool will inject the new player rig into `Neighborhood.unity` after regeneration.

**Tech Stack:** Unity 6000.5.1f1, C# 9, New Input System (direct device polling), URP, `com.unity.ai.navigation` (not used in this phase).

---

## File Structure

| File | Responsibility |
|------|----------------|
| `Assets/Scripts/PlayerController.cs` | Movement, look, sprint, jump, cursor lock. Extended to expose phone toggle input and integrate with `PhoneController`. |
| `Assets/Scripts/PhoneController.cs` | Phone light (replaces old flashlight), phone UI, time display, Quran audio toggle, message popups, vibration. |
| `Assets/Scripts/InteractionController.cs` | Raycasts from camera, finds `Interactable`, calls `Interact()` on input. |
| `Assets/Scripts/Interactable.cs` | Abstract base for all interactable objects. |
| `Assets/Scripts/DoorInteractable.cs` | Opens/closes a door on interaction. |
| `Assets/Scripts/InspectableInteractable.cs` | Triggers a message popup or event when inspected. |
| `Assets/Scripts/PickupInteractable.cs` | Destroys or disables the object and raises a pickup event. |
| `Assets/Scripts/AudioZone.cs` | Trigger volume that blends between ambience states. |
| `Assets/Scripts/GameDirector.cs` | Tracks demo state, spawns the encounter when the ball is picked up. |
| `Assets/Scripts/EncounterTrigger.cs` | One-time scripted event: spawn the woman in black at the abandoned house doorway and make her disappear on approach/after delay. |
| `Assets/Editor/NeighborhoodWorldBuilder.cs` | Updated to create the new player rig with `PhoneController` and to place the ball, encounter spawn point, and audio zones. |

---

## Task 1: Refactor PlayerController to expose phone input and remove direct flashlight ownership

**Files:**
- Modify: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: Open `Assets/Scripts/PlayerController.cs` and read it fully.**

- [ ] **Step 2: Replace the flashlight header and field with a phone controller reference.**

Change:
```csharp
    [Header("Flashlight")]
    public Light flashlight;
```
to:
```csharp
    [Header("Phone")]
    public PhoneController phone;
```

- [ ] **Step 3: Update `Awake` to remove the old flashlight fallback.**

Existing:
```csharp
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraPivot == null && Camera.main != null)
            cameraPivot = Camera.main.transform;
    }
```
Keep as is; the flashlight fallback is removed because the phone reference will be assigned by the builder.

- [ ] **Step 4: Remove the old flashlight toggle input and replace with phone toggle.**

Change:
```csharp
        if (kb.fKey.wasPressedThisFrame && flashlight != null)
            flashlight.enabled = !flashlight.enabled;
```
to:
```csharp
        if (kb.fKey.wasPressedThisFrame && phone != null)
            phone.ToggleLight();
```

- [ ] **Step 5: Commit.**

```bash
git add Assets/Scripts/PlayerController.cs
git commit -m "refactor(player): replace flashlight with phone input"
```

---

## Task 2: Create PhoneController

**Files:**
- Create: `Assets/Scripts/PhoneController.cs`

- [ ] **Step 1: Create the file with the following content.**

```csharp
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [Header("Light")]
    public Light phoneLight;
    public bool lightOnAtStart = false;

    [Header("UI")]
    public GameObject phoneScreen;
    public float messageDisplaySeconds = 4f;

    [Header("Audio")]
    public AudioSource quranSource;

    bool lightOn;
    float messageTimer;

    void Start()
    {
        SetLight(lightOnAtStart);
        if (phoneScreen != null)
            phoneScreen.SetActive(false);
    }

    public void ToggleLight()
    {
        SetLight(!lightOn);
    }

    void SetLight(bool on)
    {
        lightOn = on;
        if (phoneLight != null)
            phoneLight.enabled = on;
    }

    public void ShowMessage(string text)
    {
        // TODO Phase 2: wire to UI text element
        Debug.Log("[Phone] " + text);
        if (phoneScreen != null)
            phoneScreen.SetActive(true);
        messageTimer = messageDisplaySeconds;
    }

    public void ToggleQuran()
    {
        if (quranSource == null) return;
        if (quranSource.isPlaying)
            quranSource.Stop();
        else
            quranSource.Play();
    }

    public void Vibrate(float durationSeconds = 0.3f)
    {
        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif
    }

    void Update()
    {
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && phoneScreen != null)
                phoneScreen.SetActive(false);
        }
    }
}
```

- [ ] **Step 2: Commit.**

```bash
git add Assets/Scripts/PhoneController.cs
git commit -m "feat(phone): add PhoneController with light, Quran, messages"
```

---

## Task 3: Create interaction system base classes

**Files:**
- Create: `Assets/Scripts/Interactable.cs`
- Create: `Assets/Scripts/DoorInteractable.cs`
- Create: `Assets/Scripts/InspectableInteractable.cs`
- Create: `Assets/Scripts/PickupInteractable.cs`

- [ ] **Step 1: Create `Interactable.cs`.**

```csharp
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Text shown when the player looks at this object.")]
    public string promptText = "Interact";

    public abstract void Interact(GameObject interactor);
}
```

- [ ] **Step 2: Create `DoorInteractable.cs`.**

```csharp
using UnityEngine;

public class DoorInteractable : Interactable
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 90f;

    float targetAngle;

    void Start()
    {
        targetAngle = isOpen ? openAngle : 0f;
        ApplyAngle();
    }

    public override void Interact(GameObject interactor)
    {
        isOpen = !isOpen;
        targetAngle = isOpen ? openAngle : 0f;
    }

    void Update()
    {
        float currentY = transform.localEulerAngles.y;
        float delta = Mathf.DeltaAngle(currentY, targetAngle);
        if (Mathf.Abs(delta) < 0.1f) return;

        float step = openSpeed * Time.deltaTime * Mathf.Sign(delta);
        if (Mathf.Abs(step) > Mathf.Abs(delta))
            step = delta;

        transform.Rotate(0f, step, 0f, Space.Self);
    }

    void ApplyAngle()
    {
        Vector3 euler = transform.localEulerAngles;
        euler.y = targetAngle;
        transform.localEulerAngles = euler;
    }
}
```

- [ ] **Step 3: Create `InspectableInteractable.cs`.**

```csharp
using UnityEngine;

public class InspectableInteractable : Interactable
{
    [TextArea]
    public string inspectText = "Nothing special.";

    public override void Interact(GameObject interactor)
    {
        var phone = interactor.GetComponentInChildren<PhoneController>();
        if (phone != null)
            phone.ShowMessage(inspectText);
    }
}
```

- [ ] **Step 4: Create `PickupInteractable.cs`.**

```csharp
using UnityEngine;
using System;

public class PickupInteractable : Interactable
{
    public static event Action<PickupInteractable> OnPickedUp;

    public string itemName = "Item";

    public override void Interact(GameObject interactor)
    {
        OnPickedUp?.Invoke(this);
        gameObject.SetActive(false);
    }
}
```

- [ ] **Step 5: Commit.**

```bash
git add Assets/Scripts/Interactable.cs Assets/Scripts/DoorInteractable.cs Assets/Scripts/InspectableInteractable.cs Assets/Scripts/PickupInteractable.cs
git commit -m "feat(interaction): add interactable base, door, inspectable, pickup"
```

---

## Task 4: Create InteractionController

**Files:**
- Create: `Assets/Scripts/InteractionController.cs`
- Modify: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: Create `InteractionController.cs`.**

```csharp
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float reach = 2.5f;
    public LayerMask interactableLayers;

    Camera playerCamera;
    Interactable currentTarget;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        FindTarget();

        if (currentTarget != null && Keyboard.current.eKey.wasPressedThisFrame)
            currentTarget.Interact(gameObject);
    }

    void FindTarget()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reach, interactableLayers))
        {
            currentTarget = hit.collider.GetComponent<Interactable>();
        }
        else
        {
            currentTarget = null;
        }
    }
}
```

- [ ] **Step 2: Add `using UnityEngine.InputSystem;` to `PlayerController.cs` if not already present, and remove unused imports.**

- [ ] **Step 3: Add the interaction controller reference to the player prefab creation in `NeighborhoodWorldBuilder.cs` later (Task 7).**

- [ ] **Step 4: Commit.**

```bash
git add Assets/Scripts/InteractionController.cs
git commit -m "feat(interaction): add raycast-based InteractionController"
```

---

## Task 5: Create AudioZone

**Files:**
- Create: `Assets/Scripts/AudioZone.cs`

- [ ] **Step 1: Create `AudioZone.cs`.**

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioZone : MonoBehaviour
{
    public AudioSource ambienceSource;
    public float fadeInSeconds = 1.5f;
    public float fadeOutSeconds = 1.5f;
    public float targetVolume = 1f;

    float velocity;

    void Start()
    {
        if (ambienceSource != null)
        {
            ambienceSource.volume = 0f;
            ambienceSource.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(targetVolume, fadeInSeconds));
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f, fadeOutSeconds));
    }

    System.Collections.IEnumerator FadeTo(float target, float duration)
    {
        if (ambienceSource == null) yield break;

        float start = ambienceSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        ambienceSource.volume = target;
    }
}
```

- [ ] **Step 2: Commit.**

```bash
git add Assets/Scripts/AudioZone.cs
git commit -m "feat(audio): add crossfading AudioZone trigger volume"
```

---

## Task 6: Create GameDirector and EncounterTrigger

**Files:**
- Create: `Assets/Scripts/GameDirector.cs`
- Create: `Assets/Scripts/EncounterTrigger.cs`

- [ ] **Step 1: Create `GameDirector.cs`.**

```csharp
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public EncounterTrigger encounter;
    public PhoneController phone;

    bool encounterArmed = true;

    void OnEnable()
    {
        PickupInteractable.OnPickedUp += OnPickup;
    }

    void OnDisable()
    {
        PickupInteractable.OnPickedUp -= OnPickup;
    }

    void OnPickup(PickupInteractable item)
    {
        if (!encounterArmed) return;
        if (item.itemName != "Ball") return;

        encounterArmed = false;
        if (encounter != null)
            encounter.TriggerEncounter();

        if (phone != null)
            phone.ShowMessage("You found the ball. The doorway feels wrong.");
    }
}
```

- [ ] **Step 2: Create `EncounterTrigger.cs`.**

```csharp
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
```

- [ ] **Step 3: Commit.**

```bash
git add Assets/Scripts/GameDirector.cs Assets/Scripts/EncounterTrigger.cs
git commit -m "feat(encounter): add GameDirector and woman-in-black encounter trigger"
```

---

## Task 7: Update NeighborhoodWorldBuilder to assemble the new rig and prototype objects

**Files:**
- Modify: `Assets/Editor/NeighborhoodWorldBuilder.cs`

- [ ] **Step 1: Update `CreatePlayerRig()` to add `PhoneController` and `InteractionController`.**

Find the `CreatePlayerRig` method. After the flashlight light is created, attach the phone controller to the player and assign the light.

Existing flashlight creation inside `CreatePlayerRig()`:
```csharp
        var flashGO = new GameObject("Flashlight");
        flashGO.transform.SetParent(player.transform, false);
        flashGO.transform.localPosition = new Vector3(0.25f, 1.55f, 0.3f);
        var flash = flashGO.AddComponent<Light>();
        flash.type = LightType.Spot;
        flash.color = Hex("#F2C98C");
        flash.spotAngle = 55f;
        flash.range = 18f;
        flash.intensity = 1.6f;
        flash.shadows = LightShadows.Soft;
        controller.flashlight = flash;
```

Replace with:
```csharp
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
        controller.phone = phone;

        var interaction = player.AddComponent<InteractionController>();
        interaction.reach = 2.5f;
        interaction.interactableLayers = LayerMask.GetMask("Default");

        var audioListener = cameraGO.GetComponent<AudioListener>();
        if (audioListener == null)
            cameraGO.AddComponent<AudioListener>();
```

- [ ] **Step 2: Add a prototype ball to the abandoned house courtyard.**

At the end of `AH_Courtyard(GameObject root)`, add:

```csharp
        var ball = Sphere("Ball", c, new Vector3(-0.5f, 0.22f, 1.2f), new Vector3(0.35f, 0.35f, 0.35f), MatDoorLocked());
        var pickup = ball.AddComponent<PickupInteractable>();
        pickup.itemName = "Ball";
        pickup.promptText = "Pick up ball";
        DestroyImmediate(ball.GetComponent<SphereCollider>());
        var ballCollider = ball.AddComponent<BoxCollider>();
        ballCollider.size = new Vector3(0.35f, 0.35f, 0.35f);
```

- [ ] **Step 3: Add a placeholder woman in black prefab stub to the builder.**

Since this prototype does not use real prefab assets, create the encounter spawn point and a placeholder GameObject in the scene.

At the end of `BuildAbandonedHouse()`, after `AH_LockedRoom(root);`, add:

```csharp
        // Prototype encounter setup
        var encounterGO = new GameObject("_EncounterDirector");
        encounterGO.transform.SetParent(root.transform, false);
        var director = encounterGO.AddComponent<GameDirector>();
        director.phone = phone; // phone reference was set on the player rig earlier in the build

        var encounter = encounterGO.AddComponent<EncounterTrigger>();
        director.encounter = encounter;

        var spawnPoint = new GameObject("WomanSpawnPoint");
        spawnPoint.transform.SetParent(encounterGO.transform, false);
        spawnPoint.transform.localPosition = new Vector3(7.5f, 0f, -2.5f);
        encounter.spawnPoint = spawnPoint.transform;

        var womanPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        womanPrefab.name = "WomanInBlack_Placeholder";
        DestroyImmediate(womanPrefab.GetComponent<CapsuleCollider>());
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
```

- [ ] **Step 4: Fix the phone reference issue in the builder.**

`Build()` currently does not have access to the `phone` variable when building the abandoned house. Refactor `CreatePlayerRig()` to return the `PlayerController`, and store the phone reference in `Build()` before calling `BuildAbandonedHouse()`.

Change `CreatePlayerRig()` signature from `static void CreatePlayerRig()` to:
```csharp
    static PlayerController CreatePlayerRig()
```

At the end of `CreatePlayerRig()`, add:
```csharp
        return controller;
```

In `Build()`, change:
```csharp
        BuildPlayerStart();
```
to:
```csharp
        PlayerController playerController = BuildPlayerStart();
        PhoneController phone = playerController != null ? playerController.phone : null;
```

Change `BuildPlayerStart()` signature from `static void BuildPlayerStart()` to:
```csharp
    static PlayerController BuildPlayerStart()
```

At the end of `BuildPlayerStart()`, replace the existing player creation call with:
```csharp
        return CreatePlayerRig();
```

Then pass `phone` into `BuildAbandonedHouse()` by changing the call and method signature:

Change:
```csharp
        BuildAbandonedHouse();
```
to:
```csharp
        BuildAbandonedHouse(phone);
```

Change:
```csharp
    static void BuildAbandonedHouse()
    {
        var root = Group("_AbandonedHouse", null, new Vector3(-5, 0, -69.9f));
```
to:
```csharp
    static void BuildAbandonedHouse(PhoneController phone)
    {
        var root = Group("_AbandonedHouse", null, new Vector3(-5, 0, -69.9f));
```

- [ ] **Step 5: Commit.**

```bash
git add Assets/Editor/NeighborhoodWorldBuilder.cs
git commit -m "feat(builder): wire phone, interaction, and encounter into generated scene"
```

---

## Task 8: Regenerate the scene and smoke test

**Files:**
- Modify: `Assets/Scenes/Neighborhood.unity` (via Unity Editor)

- [ ] **Step 1: Open the project in Unity 6000.5.1f1.**

- [ ] **Step 2: Click `Tools > Neighborhood > Build World (overwrites Neighborhood scene)`.**

- [ ] **Step 3: Open `Assets/Scenes/Neighborhood.unity`.**

- [ ] **Step 4: Press Play.**

- [ ] **Step 5: Verify movement, look, sprint, jump, and cursor lock work as before.**

- [ ] **Step 6: Press `F` and verify the phone light toggles.**

- [ ] **Step 7: Walk to the abandoned house courtyard, find the ball, and press `E`.**

Expected: the ball disappears, the phone shows a message, and a black capsule appears near the doorway.

- [ ] **Step 8: Commit the regenerated scene and any new material assets.**

```bash
git add Assets/Scenes/Neighborhood.unity Assets/Art/Materials/Greybox/
git commit -m "chore(scene): regenerate Neighborhood with prototype systems"
```

---

## Task 9: Add a placeholder Quran audio clip and time display hook

**Files:**
- Create: `Assets/Audio/` folder and add a placeholder audio file (manual step)
- Modify: `Assets/Scripts/PhoneController.cs`
- Modify: `Assets/Editor/NeighborhoodWorldBuilder.cs`

- [ ] **Step 1: Add a `timeText` field to `PhoneController.cs`.**

Add to the `UI` header:
```csharp
    public TMPro.TextMeshProUGUI timeText;
```

Add to `Update()`:
```csharp
        if (timeText != null)
            timeText.text = System.DateTime.Now.ToString("HH:mm");
```

- [ ] **Step 2: In `CreatePlayerRig()`, do not wire the Quran audio source yet because no clip exists.**

Leave `quranSource` unassigned; `ToggleQuran()` will safely no-op.

- [ ] **Step 3: Document that a placeholder clip should be dropped into `Assets/Audio/QuranPlaceholder.wav` and assigned to `PhoneController.quranSource`.**

- [ ] **Step 4: Commit.**

```bash
git add Assets/Scripts/PhoneController.cs
git commit -m "feat(phone): add time display hook and placeholder Quran slot"
```

---

## Self-Review

**Spec coverage:**
- Phone as sole light source → Task 2 + Task 7.
- Phone time/Quran/messages → Task 2 + Task 9.
- Interaction system → Tasks 3 and 4.
- Audio zones → Task 5.
- Ball pickup + encounter → Tasks 6 and 7.
- Prototype scope (no cutscenes/night sequence) → all tasks.

**Placeholder scan:**
- The "TODO Phase 2" comment in `PhoneController.ShowMessage` is intentional because UI wiring is out of prototype scope; it logs to console instead.
- The Quran audio clip is documented as a manual placeholder step.
- No vague "handle edge cases" or "add validation" placeholders remain.

**Type consistency:**
- `PickupInteractable.OnPickedUp` event is defined in Task 3 and subscribed in Task 6.
- `PhoneController` reference flows from `CreatePlayerRig()` → `BuildPlayerStart()` → `Build()` → `BuildAbandonedHouse()` → `GameDirector`.
- `InteractionController.interactableLayers` uses `LayerMask.GetMask("Default")`, consistent with the default primitive layer used by builder objects.

**Execution handoff:** see final message.
