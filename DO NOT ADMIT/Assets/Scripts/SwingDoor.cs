using System.Collections;
using UnityEngine;

public class SwingDoor : Interactable
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;

    [Header("Rotation")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 4f;

    [Header("Automatic Closing")]
    [SerializeField] private float autoCloseDelay = 0.5f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool doorOpen;
    private bool moving;

    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localRotation;

        openRotation =
            closedRotation *
            Quaternion.Euler(
                0f,
                openAngle,
                0f
            );
    }

    // ==================================================
    // PROMPT
    // ==================================================

    public override void ShowPrompt()
    {
        if (GameFlowManager.Instance != null &&
            !GameFlowManager.Instance.CanUseBoothDoor)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }

    // ==================================================
    // PLAYER INTERACTION
    // ==================================================

    public override void Interact()
    {
        if (GameFlowManager.Instance != null &&
            !GameFlowManager.Instance.CanUseBoothDoor)
        {
            Debug.Log(
                "Booth door is currently locked."
            );

            return;
        }

        if (moving)
            return;

        doorOpen = !doorOpen;

        StopAllCoroutines();

        StartCoroutine(
            RotateDoor(
                doorOpen
                    ? openRotation
                    : closedRotation
            )
        );
    }

    // ==================================================
    // AUTOMATIC CLOSING
    // ==================================================

    public void AutoCloseDoor()
    {
        if (!doorOpen)
            return;

        StopAllCoroutines();

        StartCoroutine(
            AutoCloseRoutine()
        );
    }

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(
            autoCloseDelay
        );

        doorOpen = false;

        yield return StartCoroutine(
            RotateDoor(
                closedRotation
            )
        );

        HidePrompt();

        Debug.Log(
            "Booth door automatically closed."
        );
    }

    // ==================================================
    // ROTATION
    // ==================================================

    private IEnumerator RotateDoor(
        Quaternion targetRotation)
    {
        moving = true;

        while (
            Quaternion.Angle(
                doorPivot.localRotation,
                targetRotation
            ) > 0.5f)
        {
            doorPivot.localRotation =
                Quaternion.Slerp(
                    doorPivot.localRotation,
                    targetRotation,
                    openSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        doorPivot.localRotation =
            targetRotation;

        moving = false;
    }
}