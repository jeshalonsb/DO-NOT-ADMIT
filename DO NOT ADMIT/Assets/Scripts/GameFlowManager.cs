using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform carSpawnPoint;

    [Header("Game Systems")]
    [SerializeField] private ShiftClock shiftClock;
    [SerializeField] private VisitorManager visitorManager;
    [SerializeField] private PlayerDialogueController playerDialogue;
    [SerializeField] private SlidingDoor boothDoor;

    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;

    private bool playerReachedBooth;
    private bool boothApproachDialoguePlayed;

    private bool manualRead;
    private bool shiftStarted;

    private bool blackoutActive;
    private bool waitingForBoothReturn;

    private bool shiftComplete;

    public bool ManualRead => manualRead;
    public bool ShiftStarted => shiftStarted;
    public bool BlackoutActive => blackoutActive;
    public bool ShiftComplete => shiftComplete;

    public bool SuppressNormalInteractionPrompts =>
        blackoutActive;

    public bool CanUseBoothDoor
    {
        get
        {
            // Before clocking in, the player can freely
            // enter and leave the booth.
            if (!shiftStarted)
                return true;

            // During blackout, the player must be able
            // to leave the booth.
            if (blackoutActive)
                return true;

            // After restoring the breaker, allow the
            // player to get back inside.
            if (waitingForBoothReturn)
                return true;

            // After 6 AM, permanently allow the player out.
            if (shiftComplete)
                return true;

            // During the active normal shift,
            // keep the booth door locked.
            return false;
        }
    }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SpawnPlayerAtCar();

        SetObjective(
            "GO TO THE SECURITY BOOTH"
        );
    }

    // ==================================================
    // SPAWN
    // ==================================================

    private void SpawnPlayerAtCar()
    {
        if (player == null ||
            carSpawnPoint == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.transform.position =
            carSpawnPoint.position;

        player.transform.rotation =
            carSpawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;
    }

    // ==================================================
    // BOOTH APPROACH
    // ==================================================

    public void PlayerApproachedBooth()
    {
        if (boothApproachDialoguePlayed)
            return;

        if (playerReachedBooth)
            return;

        boothApproachDialoguePlayed = true;

        if (playerDialogue != null)
        {
            playerDialogue.SayBoothApproach();
        }

        Debug.Log(
            "Player noticed security booth."
        );
    }

    // ==================================================
    // BOOTH
    // ==================================================

    public void PlayerEnteredBooth()
    {
        // Returning from blackout.
        if (waitingForBoothReturn)
        {
            waitingForBoothReturn = false;
            blackoutActive = false;

            SetObjective(
                "SHIFT STARTED"
            );

            Debug.Log(
                "Player returned to booth."
            );

            return;
        }

        // First time entering booth.
        if (!shiftStarted &&
    !playerReachedBooth)
        {
            playerReachedBooth = true;

            if (playerDialogue != null)
            {
                playerDialogue.SayBoothApproach();
            }

            SetObjective(
                "READ THE FIRST DAY MANUAL"
            );

            Debug.Log(
                "Player reached booth."
            );
        }
    }

    // ==================================================
    // MANUAL
    // ==================================================

    public void ReadManual()
    {
        if (shiftStarted)
            return;

        if (!playerReachedBooth)
            return;

        if (manualRead)
            return;

        manualRead = true;

        SetObjective(
            "CLOCK IN"
        );

        Debug.Log(
            "First day manual completed."
        );
    }

    // ==================================================
    // CLOCK IN
    // ==================================================

    public void ClockIn()
    {
        if (shiftStarted)
            return;

        if (!manualRead)
        {
            Debug.Log(
                "Player must read the manual first."
            );

            return;
        }

        shiftStarted = true;

        // Lock player inside the booth.
        if (boothDoor != null)
        {
            boothDoor.LockDoor();
        }
        else
        {
            Debug.LogWarning(
                "GameFlowManager has no Booth Door assigned!"
            );
        }

        SetObjective(
            "SHIFT STARTED"
        );

        if (shiftClock != null)
            shiftClock.StartClock();

        if (visitorManager != null)
            visitorManager.StartShift();

        Debug.Log(
            "PLAYER CLOCKED IN - SHIFT STARTED"
        );
    }

    // ==================================================
    // BLACKOUT
    // ==================================================

    public void BlackoutStarted()
    {
        if (!shiftStarted)
            return;

        blackoutActive = true;

        if (boothDoor != null)
        {
            boothDoor.UnlockDoor();
        }

        SetObjective(
            "POWER FAILURE - RESET BREAKER\nHINT: SECURITY PARKING"
        );

        Debug.Log(
            "BLACKOUT OBJECTIVE ACTIVE"
        );
    }

    public void PowerRestored()
    {
        if (!shiftStarted)
            return;

        waitingForBoothReturn = true;

        SetObjective(
            "RETURN TO THE SECURITY BOOTH"
        );

        Debug.Log(
            "Power restored. Return to booth."
        );
    }

    // ==================================================
    // END SHIFT
    // ==================================================

    public void CompleteShift()
    {
        if (shiftComplete)
            return;

        shiftComplete = true;

        blackoutActive = false;
        waitingForBoothReturn = false;

        if (playerDialogue != null)
        {
            playerDialogue.SayShiftOver();
        }

        // Permanently unlock and open the booth door.
        if (boothDoor != null)
        {
            boothDoor.UnlockForShiftEnd();
        }

        SetObjective(
            "SHIFT COMPLETE - RETURN TO YOUR CAR"
        );

        Debug.Log(
            "Shift complete."
        );
    }

    // ==================================================
    // OBJECTIVE
    // ==================================================

    public void SetObjective(
        string message)
    {
        if (objectiveText == null)
            return;

        objectiveText.text = message;
    }
}