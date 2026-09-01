using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Header("Vertical Look Limits")]
    [SerializeField] private float maxLookUp = 80f;
    [SerializeField] private float maxLookDown = 80f;

    [Header("References")]
    [SerializeField] private Transform playerBody;

    private float xRotation;

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

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
    public void SetSensitivity(float value)
    {
        mouseSensitivity = value;
    }
}