using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    public float interactionRange = 2.5f;
    public LayerMask interactableLayers;

    Camera playerCamera;
    Interactable currentTarget;

    void Awake()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        FindTarget();

        if (currentTarget != null && currentTarget.isActiveAndEnabled && Keyboard.current.eKey.wasPressedThisFrame)
            currentTarget.Interact(gameObject);
    }

    void FindTarget()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayers))
        {
            currentTarget = hit.collider.GetComponentInParent<Interactable>();
        }
        else
        {
            currentTarget = null;
        }
    }
}
