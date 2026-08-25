using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookUp = 80f;
    [SerializeField] private float maxLookDown = 80f;

    [Header("Horizontal Look Limits")]
    [SerializeField] private float maxLookLeft = 100f;
    [SerializeField] private float maxLookRight = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Vertical look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookUp, maxLookDown);

        // Horizontal look
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -maxLookLeft, maxLookRight);

        // Rotate ONLY the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}