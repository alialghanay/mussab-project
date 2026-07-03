using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Text shown when the player looks at this object.")]
    public string promptText = "Interact";

    public abstract void Interact(GameObject interactor);
}
