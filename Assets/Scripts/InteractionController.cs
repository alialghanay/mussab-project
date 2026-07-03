using UnityEngine;
using UnityEngine.InputSystem;

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
