using System.Collections;
using UnityEngine;

public class SlidingDoor : Interactable
{
    [Header("Door")]
    [SerializeField] private Transform doorPanel;

    [Header("Sliding")]
    [SerializeField] private Vector3 openOffset = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private float slideSpeed = 4f;

    [Header("Automatic Closing")]
    [SerializeField] private float autoCloseDelay = 0.5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool doorOpen;
    private bool moving;

    private void Awake()
    {
        if (doorPanel == null)
            doorPanel = transform;

        closedPosition = doorPanel.localPosition;

        openPosition =
            closedPosition +
            openOffset;
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
            SlideDoor(
                doorOpen
                    ? openPosition
                    : closedPosition
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
            SlideDoor(
                closedPosition
            )
        );

        HidePrompt();

        Debug.Log(
            "Booth door automatically closed."
        );
    }

    // ==================================================
    // SLIDING
    // ==================================================

    private IEnumerator SlideDoor(
        Vector3 targetPosition)
    {
        moving = true;

        while (
            Vector3.Distance(
                doorPanel.localPosition,
                targetPosition
            ) > 0.01f)
        {
            doorPanel.localPosition =
                Vector3.MoveTowards(
                    doorPanel.localPosition,
                    targetPosition,
                    slideSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        doorPanel.localPosition =
            targetPosition;

        moving = false;
    }
}