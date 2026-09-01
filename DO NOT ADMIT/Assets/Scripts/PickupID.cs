using System.Collections;
using UnityEngine;

public class PickupID : Interactable
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Hold Position")]
    [SerializeField]
    private Vector3 holdPosition =
        new Vector3(0.3f, -0.2f, 0.55f);

    [SerializeField]
    private Vector3 holdRotation =
        new Vector3(10f, 0f, 0f);

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotateSpeed = 10f;

    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isHeld;
    private bool isMoving;

    private Coroutine moveCoroutine;

    protected override void Start()
    {
        base.Start();

        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public override void Interact()
    {
        if (isMoving)
            return;

        if (isHeld)
        {
            PutDown();
        }
        else
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning(
                "Player Camera is not assigned on " +
                gameObject.name
            );

            return;
        }

        isHeld = true;

        HidePrompt();

        transform.SetParent(
            playerCamera,
            true
        );

        Vector3 targetPosition =
            playerCamera.TransformPoint(
                holdPosition
            );

        Quaternion targetRotation =
            playerCamera.rotation *
            Quaternion.Euler(
                holdRotation
            );

        StartMove(
            targetPosition,
            targetRotation,
            false
        );
    }

    private void PutDown()
    {
        isHeld = false;

        HidePrompt();

        transform.SetParent(
            null,
            true
        );

        StartMove(
            originalPosition,
            originalRotation,
            true
        );
    }

    private void StartMove(
        Vector3 targetPosition,
        Quaternion targetRotation,
        bool restoreParent)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );
        }

        moveCoroutine =
            StartCoroutine(
                MoveID(
                    targetPosition,
                    targetRotation,
                    restoreParent
                )
            );
    }

    private IEnumerator MoveID(
        Vector3 targetPosition,
        Quaternion targetRotation,
        bool restoreParent)
    {
        isMoving = true;

        while (
            Vector3.Distance(
                transform.position,
                targetPosition
            ) > 0.005f ||
            Quaternion.Angle(
                transform.rotation,
                targetRotation
            ) > 0.5f)
        {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );

            yield return null;
        }

        transform.position =
            targetPosition;

        transform.rotation =
            targetRotation;

        if (restoreParent)
        {
            transform.SetParent(
                originalParent,
                true
            );
        }

        isMoving = false;
        moveCoroutine = null;
    }
}