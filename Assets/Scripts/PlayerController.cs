// First-person controller for walking the greybox world.
// New Input System (direct device polling - no action asset required).
// WASD move, mouse look, Shift sprint, Space jump, F flashlight,
// Esc releases the cursor, left-click recaptures it.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.6f;
    public float sprintSpeed = 4.6f;
    public float jumpHeight = 1.0f;
    public float gravity = -18f;

    [Header("Look")]
    public float mouseSensitivity = 0.08f;
    public float pitchLimit = 85f;
    public Transform cameraPivot;

    [Header("Flashlight")]
    public Light flashlight;

    CharacterController controller;
    float pitch;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraPivot == null && Camera.main != null)
            cameraPivot = Camera.main.transform;
    }

    void Start()
    {
        LockCursor(true);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        if (kb.escapeKey.wasPressedThisFrame) LockCursor(false);
        if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) LockCursor(true);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(0f, look.x, 0f);
            pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        Vector2 move = Vector2.zero;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
        move = Vector2.ClampMagnitude(move, 1f);

        float speed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
        Vector3 velocity = (transform.right * move.x + transform.forward * move.y) * speed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // keep the capsule seated on slopes and stairs
            if (kb.spaceKey.wasPressedThisFrame)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (kb.fKey.wasPressedThisFrame && flashlight != null)
            flashlight.enabled = !flashlight.enabled;
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
