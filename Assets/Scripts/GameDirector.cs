using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public EncounterTrigger encounter;
    public PhoneController phone;
    public PickupInteractable targetPickup;

    bool encounterArmed = true;

    void OnEnable()
    {
        if (targetPickup != null)
            targetPickup.OnPickedUp += OnPickup;
    }

    void OnDisable()
    {
        if (targetPickup != null)
            targetPickup.OnPickedUp -= OnPickup;
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
