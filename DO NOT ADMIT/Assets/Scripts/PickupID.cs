using System.Collections;
using UnityEngine;

public class PickupID : Interactable
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Hold Position")]
    [SerializeField]
    private Vector3 holdPosition =
        new Vector3(0.18f, -0.18f, 0.45f);

    [SerializeField]
    private Vector3 holdRotation =
        new Vector3(10f, 0f, 0f);

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 12f;

    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private bool deskPositionCached;

    private bool isHeld;
    private bool isMoving;

    private Coroutine moveCoroutine;

    public bool IsHeld => isHeld;

    private void Awake()
    {
        CacheDeskPosition();
    }

    protected override void Start()
    {
        base.Start();

        CacheDeskPosition();
    }

    private void CacheDeskPosition()
    {
        if (deskPositionCached)
            return;

        originalParent =
            transform.parent;

        originalLocalPosition =
            transform.localPosition;

        originalLocalRotation =
            transform.localRotation;

        deskPositionCached = true;

        Debug.Log(
            "ID desk position cached: " +
            transform.position
        );
    }

    public override void Interact()
    {
        if (isMoving)
            return;

        if (isHeld)
            PutDown();
        else
            PickUp();
    }

    private void PickUp()
    {
        CacheDeskPosition();

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

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine =
            StartCoroutine(
                MoveToHeldPosition()
            );
    }

    public void PutDown()
    {
        CacheDeskPosition();

        if (!isHeld && !isMoving)
            return;

        isHeld = false;

        HidePrompt();

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        transform.SetParent(
            originalParent,
            true
        );

        moveCoroutine =
            StartCoroutine(
                MoveToDesk()
            );
    }

    private IEnumerator MoveToHeldPosition()
    {
        isMoving = true;

        Quaternion targetRotation =
            Quaternion.Euler(
                holdRotation
            );

        while (
            Vector3.Distance(
                transform.localPosition,
                holdPosition
            ) > 0.002f ||
            Quaternion.Angle(
                transform.localRotation,
                targetRotation
            ) > 0.25f)
        {
            transform.localPosition =
                Vector3.Lerp(
                    transform.localPosition,
                    holdPosition,
                    moveSpeed * Time.deltaTime
                );

            transform.localRotation =
                Quaternion.Slerp(
                    transform.localRotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );

            yield return null;
        }

        transform.localPosition =
            holdPosition;

        transform.localRotation =
            targetRotation;

        isMoving = false;
        moveCoroutine = null;
    }

    private IEnumerator MoveToDesk()
    {
        isMoving = true;

        while (
            Vector3.Distance(
                transform.localPosition,
                originalLocalPosition
            ) > 0.002f ||
            Quaternion.Angle(
                transform.localRotation,
                originalLocalRotation
            ) > 0.25f)
        {
            transform.localPosition =
                Vector3.Lerp(
                    transform.localPosition,
                    originalLocalPosition,
                    moveSpeed * Time.deltaTime
                );

            transform.localRotation =
                Quaternion.Slerp(
                    transform.localRotation,
                    originalLocalRotation,
                    rotateSpeed * Time.deltaTime
                );

            yield return null;
        }

        transform.localPosition =
            originalLocalPosition;

        transform.localRotation =
            originalLocalRotation;

        isMoving = false;
        moveCoroutine = null;
    }

    public void ResetToDeskImmediate()
    {
        /*
         * This is important because DisplayVisitor()
         * can call this before Start().
         */
        CacheDeskPosition();

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isHeld = false;
        isMoving = false;

        HidePrompt();

        transform.SetParent(
            originalParent,
            false
        );

        transform.localPosition =
            originalLocalPosition;

        transform.localRotation =
            originalLocalRotation;
    }
}