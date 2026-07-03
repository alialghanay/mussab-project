using UnityEngine;
using System;

public class PickupInteractable : Interactable
{
    public event Action<PickupInteractable> OnPickedUp;

    public string itemName = "Item";

    public override void Interact(GameObject interactor)
    {
        OnPickedUp?.Invoke(this);
        gameObject.SetActive(false);
    }
}
