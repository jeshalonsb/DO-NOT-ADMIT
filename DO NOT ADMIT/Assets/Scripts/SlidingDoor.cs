using System.Collections;
using UnityEngine;

public class SlidingDoor : Interactable
{
    [Header("Door")]
    [SerializeField] private Transform doorPanel;

    [Header("Sliding")]
    [SerializeField]
    private Vector3 openOffset =
        new Vector3(1.5f, 0f, 0f);

    [SerializeField] private float slideSpeed = 4f;

    [Header("Automatic Closing")]
    [SerializeField] private float autoCloseDelay = 0.5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool doorOpen;
    private bool moving;

    // Starts unlocked so the player can enter the booth
    // before clocking in.
    private bool doorLocked;

    private void Awake()
    {
        if (doorPanel == null)
        {
            doorPanel = transform;
        }

        closedPosition =
            doorPanel.localPosition;

        openPosition =
            closedPosition + openOffset;
    }

    private void Update()
    {
        // If the door becomes locked while the player
        // is already looking at it, hide the prompt.
        if (doorLocked)
        {
            HidePrompt();
        }
    }

    // ==================================================
    // PROMPT
    // ==================================================

    public override void ShowPrompt()
    {
        if (doorLocked)
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
        if (doorLocked)
        {
            HidePrompt();

            Debug.Log(
                "BOOTH DOOR LOCKED"
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
    // LOCK DOOR
    // ==================================================

    public void LockDoor()
    {
        doorLocked = true;

        HidePrompt();

        Debug.Log(
            "BOOTH DOOR HAS BEEN LOCKED"
        );

        // If the door is open when the shift starts,
        // close it automatically.
        if (doorOpen)
        {
            doorOpen = false;

            StopAllCoroutines();

            StartCoroutine(
                SlideDoor(
                    closedPosition
                )
            );
        }
    }

    // ==================================================
    // UNLOCK DOOR
    // ==================================================

    public void UnlockDoor()
    {
        doorLocked = false;

        Debug.Log(
            "BOOTH DOOR HAS BEEN UNLOCKED"
        );
    }

    // ==================================================
    // SHIFT END
    // ==================================================

    public void UnlockForShiftEnd()
    {
        doorLocked = false;

        HidePrompt();

        Debug.Log(
            "BOOTH DOOR UNLOCKED FOR SHIFT END"
        );

        // Automatically open the door at the end
        // of the shift so the player can leave.
        if (!doorOpen)
        {
            doorOpen = true;

            StopAllCoroutines();

            StartCoroutine(
                SlideDoor(
                    openPosition
                )
            );
        }
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