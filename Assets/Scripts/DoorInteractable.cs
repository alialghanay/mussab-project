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
