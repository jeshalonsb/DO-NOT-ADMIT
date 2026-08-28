using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
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
    private bool waitingForBoothReturn;
    private bool shiftComplete;

    public bool ManualRead => manualRead;
    public bool ShiftStarted => shiftStarted;
    public bool ShiftComplete => shiftComplete;

    private void Start()
    {
        SpawnPlayerAtCar();

        SetObjective("GO TO THE SECURITY BOOTH");
    }

    // ==================================================
    // OPENING
    // ==================================================

    private void SpawnPlayerAtCar()
    {
        if (player == null || carSpawnPoint == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.transform.position = carSpawnPoint.position;
        player.transform.rotation = carSpawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;
    }

    public void PlayerEnteredBooth()
    {
        // During blackout recovery
        if (waitingForBoothReturn)
        {
            waitingForBoothReturn = false;

            SetObjective("SHIFT STARTED");

            Debug.Log("Player returned to booth.");
            return;
        }

        // Opening sequence
        if (!shiftStarted && !playerReachedBooth)
        {
            playerReachedBooth = true;

            SetObjective("READ THE FIRST DAY MANUAL");

            Debug.Log("Player reached booth.");
        }
    }

    public void ReadManual()
    {
        if (shiftStarted)
            return;

        if (!playerReachedBooth)
            return;

        if (manualRead)
            return;

        manualRead = true;

        SetObjective("CLOCK IN");

        Debug.Log("First day manual read.");
    }

    public void ClockIn()
    {
        if (shiftStarted)
            return;

        if (!manualRead)
        {
            Debug.Log("Player must read the manual first.");
            return;
        }

        shiftStarted = true;

        SetObjective("SHIFT STARTED");

        if (shiftClock != null)
            shiftClock.StartClock();

        if (visitorManager != null)
            visitorManager.StartShift();

        Debug.Log("PLAYER CLOCKED IN - SHIFT STARTED");
    }

    // ==================================================
    // BLACKOUT
    // ==================================================

    public void BlackoutStarted()
    {
        if (!shiftStarted)
            return;

        SetObjective(
            "RESET THE BREAKER - SECURITY PARKING"
        );

        Debug.Log("Objective: Reset breaker.");
    }

    public void PowerRestored()
    {
        if (!shiftStarted)
            return;

        waitingForBoothReturn = true;

        SetObjective("RETURN TO THE BOOTH");

        Debug.Log("Objective: Return to booth.");
    }

    // ==================================================
    // END SHIFT
    // ==================================================

    public void CompleteShift()
    {
        if (shiftComplete)
            return;

        shiftComplete = true;
        waitingForBoothReturn = false;

        SetObjective(
            "SHIFT COMPLETE - RETURN TO YOUR CAR"
        );

        Debug.Log("Shift complete. Return to car.");
    }

    // ==================================================
    // OBJECTIVE
    // ==================================================

    private void SetObjective(string message)
    {
        if (objectiveText != null)
            objectiveText.text = message;
    }
}