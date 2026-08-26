using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3.5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed)
            z += 1f;

        if (Keyboard.current.sKey.isPressed)
            z -= 1f;

        if (Keyboard.current.dKey.isPressed)
            x += 1f;

        if (Keyboard.current.aKey.isPressed)
            x -= 1f;

        Vector3 direction = new Vector3(x, 0f, z).normalized;

        Vector3 movement =
            transform.right * direction.x +
            transform.forward * direction.z;

        controller.Move(movement * walkSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }
}