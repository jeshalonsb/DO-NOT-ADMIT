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

    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;

    private bool playerReachedBooth;
    private bool manualRead;
    private bool shiftStarted;

    private bool blackoutActive;
    private bool waitingForBoothReturn;

    private bool shiftComplete;

    public bool ManualRead => manualRead;
    public bool ShiftStarted => shiftStarted;
    public bool BlackoutActive => blackoutActive;
    public bool ShiftComplete => shiftComplete;

    /*
     * Used by Interactable.
     *
     * During blackout, ordinary interaction prompts
     * are hidden unless that object specifically
     * allows blackout interaction.
     */
    public bool SuppressNormalInteractionPrompts =>
        blackoutActive;

    /*
     * Booth door rules:
     *
     * 1. Usable at the beginning before entering booth.
     * 2. Usable during blackout.
     * 3. Usable while returning after breaker reset.
     * 4. Usable after shift completes.
     */
    public bool CanUseBoothDoor
    {
        get
        {
            if (!playerReachedBooth)
                return true;

            if (blackoutActive)
                return true;

            if (waitingForBoothReturn)
                return true;

            if (shiftComplete)
                return true;

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
    // BOOTH
    // ==================================================

    public void PlayerEnteredBooth()
    {
        /*
         * Returning from blackout.
         */
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

        /*
         * First time entering booth.
         */
        if (!shiftStarted &&
            !playerReachedBooth)
        {
            playerReachedBooth = true;

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

        /*
         * Keep blackout interaction restrictions
         * active until player gets back inside.
         */
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