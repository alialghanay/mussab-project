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
