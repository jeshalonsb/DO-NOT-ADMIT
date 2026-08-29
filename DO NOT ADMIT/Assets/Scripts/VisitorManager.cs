using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    [Header("Visitor")]
    [SerializeField] private GameObject visitorPrefab;
    [SerializeField] private VisitorData[] visitors;

    [Header("Visitor Points")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private Transform doorWaitPoint;
    [SerializeField] private Transform entryExitPoint;
    [SerializeField] private Transform denyExitPoint;

    [Header("Timing")]
    [SerializeField] private float timeBetweenVisitors = 3f;

    [Header("UI")]
    [SerializeField] private IDCard idCard;
    [SerializeField] private VisitorDialogueUI dialogueUI;

    [Header("Game Systems")]
    [SerializeField] private ShiftManager shiftManager;
    [SerializeField] private FacilityDoors facilityDoors;
    [SerializeField] private PowerManager powerManager;

    [Header("Impostor Settings")]
    [SerializeField] private int minimumImpostors = 1;
    [SerializeField] private int maximumImpostors = 2;

    private readonly HashSet<int> impostorIndices =
        new HashSet<int>();

    private Visitor currentVisitor;
    private Visitor visitorWaitingAtDoor;

    private int currentVisitorIndex;

    private bool shiftStarted;
    private bool decisionMade;

    private bool visitorSpawningPaused;
    private bool waitingToSpawnVisitor;

    private bool shiftEnding;
    private bool shiftClosed;

    // If 6 AM happens while a cleared visitor
    // still needs to enter the facility.
    private bool waitingForFinalClearedVisitor;

    public event Action OnShiftReadyToFinish;

    // ==================================================
    // SHIFT
    // ==================================================

    public void StartShift()
    {
        if (shiftStarted)
            return;

        shiftStarted = true;
        shiftEnding = false;
        shiftClosed = false;
        waitingForFinalClearedVisitor = false;

        currentVisitorIndex = 0;

        GenerateImpostors();

        Debug.Log("VISITOR SHIFT STARTED");

        SpawnVisitor();
    }

    // Called around 5 AM.
    public void BeginShiftEnding()
    {
        if (shiftEnding)
            return;

        shiftEnding = true;
        waitingToSpawnVisitor = false;

        Debug.Log(
            "Shift ending soon. No more visitors will spawn."
        );
    }

    // ==================================================
    // 6 AM
    // ==================================================

    /*
     * Returns TRUE if the shift can finish immediately.
     *
     * Returns FALSE if a cleared visitor still needs
     * to finish entering first.
     */
    public bool CloseShift()
    {
        if (shiftClosed)
            return true;

        shiftEnding = true;
        waitingToSpawnVisitor = false;

        Debug.Log(
            "6 AM - CHECKING FINAL VISITOR STATE"
        );

        if (idCard != null)
            idCard.HideCard();

        // --------------------------------------------------
        // CLEARED VISITOR
        //
        // If they were already cleared before 6 AM,
        // they must finish entering.
        // --------------------------------------------------

        if (currentVisitor != null &&
            currentVisitor.IsPendingFacilityEntry)
        {
            waitingForFinalClearedVisitor = true;

            Debug.Log(
                "Shift waiting for final cleared visitor to enter."
            );

            return false;
        }

        if (visitorWaitingAtDoor != null)
        {
            waitingForFinalClearedVisitor = true;

            Debug.Log(
                "Shift waiting for visitor at facility door."
            );

            return false;
        }

        // --------------------------------------------------
        // ALREADY DENIED
        //
        // Keep their denial dialogue.
        // Don't replace it with the 6 AM dialogue.
        // --------------------------------------------------

        if (currentVisitor != null &&
            currentVisitor.IsLeavingDenied)
        {
            Debug.Log(
                "Visitor was already denied. Keeping original dialogue."
            );

            FinalizeShiftClosure();
            return true;
        }

        // --------------------------------------------------
        // UNDECIDED VISITOR
        //
        // Only undecided visitors get the
        // "shift's over" dialogue.
        // --------------------------------------------------

        if (currentVisitor != null &&
            currentVisitor.IsUndecided)
        {
            if (dialogueUI != null &&
                currentVisitor.Data != null)
            {
                dialogueUI.ShowDialogue(
                    currentVisitor.Data.visitorName,
                    "Looks like your shift's over. I'll come back later."
                );
            }

            currentVisitor.LeaveForShiftEnd();

            FinalizeShiftClosure();
            return true;
        }

        // Nobody is currently being processed.
        FinalizeShiftClosure();

        return true;
    }

    private void FinalizeShiftClosure()
    {
        if (shiftClosed)
            return;

        shiftClosed = true;
        shiftEnding = true;
        waitingForFinalClearedVisitor = false;
        waitingToSpawnVisitor = false;

        if (idCard != null)
            idCard.HideCard();

        Debug.Log(
            "SHIFT CLOSED - VISITOR PROCESSING DISABLED"
        );
    }

    // ==================================================
    // IMPOSTORS
    // ==================================================

    private void GenerateImpostors()
    {
        impostorIndices.Clear();

        if (visitors == null ||
            visitors.Length == 0)
        {
            return;
        }

        int minimum =
            Mathf.Clamp(
                minimumImpostors,
                0,
                visitors.Length
            );

        int maximum =
            Mathf.Clamp(
                maximumImpostors,
                minimum,
                visitors.Length
            );

        int impostorCount =
            UnityEngine.Random.Range(
                minimum,
                maximum + 1
            );

        while (
            impostorIndices.Count <
            impostorCount)
        {
            int randomIndex =
                UnityEngine.Random.Range(
                    0,
                    visitors.Length
                );

            impostorIndices.Add(
                randomIndex
            );
        }

        Debug.Log(
            "Generated " +
            impostorIndices.Count +
            " impostor visitors."
        );
    }

    // ==================================================
    // SPAWNING
    // ==================================================

    private void SpawnVisitor()
    {
        if (!shiftStarted)
            return;

        if (shiftClosed)
            return;

        if (shiftEnding)
        {
            Debug.Log(
                "Visitor not spawned because shift is ending."
            );

            return;
        }

        if (visitorSpawningPaused)
        {
            waitingToSpawnVisitor = true;
            return;
        }

        if (visitorPrefab == null ||
            visitors == null ||
            visitors.Length == 0 ||
            spawnPoint == null)
        {
            Debug.LogWarning(
                "VisitorManager is missing visitor setup."
            );

            return;
        }

        if (currentVisitorIndex >= visitors.Length)
        {
            currentVisitorIndex = 0;
        }

        VisitorData visitorData =
            visitors[currentVisitorIndex];

        bool isImpostor =
            impostorIndices.Contains(
                currentVisitorIndex
            );

        GameObject visitorObject =
            Instantiate(
                visitorPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        Visitor visitor =
            visitorObject.GetComponent<Visitor>();

        if (visitor == null)
        {
            Debug.LogWarning(
                "Visitor prefab has no Visitor component."
            );

            Destroy(visitorObject);
            return;
        }

        visitor.SetVisitorData(
            visitorData
        );

        visitor.SetImpostor(
            isImpostor
        );

        visitor.Setup(
            inspectionPoint,
            doorWaitPoint,
            entryExitPoint,
            denyExitPoint,
            this
        );

        currentVisitor = visitor;

        currentVisitorIndex++;

        Debug.Log(
            "Spawned visitor: " +
            visitorData.visitorName
        );
    }

    private IEnumerator SpawnNextVisitor()
    {
        yield return new WaitForSeconds(
            timeBetweenVisitors
        );

        if (shiftClosed)
            yield break;

        if (shiftEnding)
        {
            Debug.Log(
                "No next visitor because shift is ending."
            );

            yield break;
        }

        if (visitorSpawningPaused)
        {
            waitingToSpawnVisitor = true;

            Debug.Log(
                "Next visitor waiting because spawning is paused."
            );

            yield break;
        }

        SpawnVisitor();
    }

    // ==================================================
    // VISITOR ARRIVAL
    // ==================================================

    public void VisitorReady(
        Visitor visitor)
    {
        if (shiftClosed)
        {
            visitor.LeaveForShiftEnd();
            return;
        }

        currentVisitor = visitor;
        decisionMade = false;

        if (idCard != null)
        {
            idCard.DisplayVisitor(
                visitor
            );
        }

        if (dialogueUI != null &&
            visitor.Data != null)
        {
            dialogueUI.ShowDialogue(
                visitor.Data.visitorName,
                visitor.Data.arrivalDialogue
            );
        }

        Debug.Log(
            "Visitor ready for inspection."
        );
    }

    // ==================================================
    // CLEAR
    // ==================================================

    public void ClearCurrentVisitor()
    {
        if (shiftClosed)
        {
            Debug.Log(
                "Checkpoint is closed."
            );

            return;
        }

        if (powerManager != null &&
            !powerManager.PowerOn)
        {
            Debug.Log(
                "CLEAR unavailable during power outage."
            );

            return;
        }

        if (currentVisitor == null)
        {
            Debug.Log(
                "No visitor available to clear."
            );

            return;
        }

        if (decisionMade)
        {
            Debug.Log(
                "Decision already made for this visitor."
            );

            return;
        }

        decisionMade = true;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                currentVisitor.CorrectDecision,
                CorrectDecision.Clear
            );
        }

        if (dialogueUI != null &&
            currentVisitor.Data != null)
        {
            dialogueUI.ShowDialogue(
                currentVisitor.Data.visitorName,
                currentVisitor.Data.clearDialogue
            );
        }

        currentVisitor.Clear();

        Debug.Log(
            "PLAYER CHOSE CLEAR"
        );
    }

    // ==================================================
    // DENY
    // ==================================================

    public void DenyCurrentVisitor()
    {
        if (shiftClosed)
        {
            Debug.Log(
                "Checkpoint is closed."
            );

            return;
        }

        if (powerManager != null &&
            !powerManager.PowerOn)
        {
            Debug.Log(
                "DENY unavailable during power outage."
            );

            return;
        }

        if (currentVisitor == null)
        {
            Debug.Log(
                "No visitor available to deny."
            );

            return;
        }

        if (decisionMade)
        {
            Debug.Log(
                "Decision already made for this visitor."
            );

            return;
        }

        decisionMade = true;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                currentVisitor.CorrectDecision,
                CorrectDecision.Deny
            );

            shiftManager.RegisterDeniedVisitor(
                currentVisitor
            );
        }

        if (dialogueUI != null &&
            currentVisitor.Data != null)
        {
            dialogueUI.ShowDialogue(
                currentVisitor.Data.visitorName,
                currentVisitor.Data.denyDialogue
            );
        }

        currentVisitor.Deny();

        Debug.Log(
            "PLAYER CHOSE DENY"
        );
    }

    // ==================================================
    // FACILITY DOOR
    // ==================================================

    public void VisitorWaitingAtDoor(
        Visitor visitor)
    {
        if (shiftClosed)
        {
            visitor.LeaveForShiftEnd();
            return;
        }

        visitorWaitingAtDoor =
            visitor;

        Debug.Log(
            "Visitor waiting for facility door unlock."
        );
    }

    public void UnlockFacilityDoor()
    {
        if (shiftClosed)
        {
            Debug.Log(
                "Checkpoint is closed."
            );

            return;
        }

        if (powerManager != null &&
            !powerManager.PowerOn)
        {
            Debug.Log(
                "FACILITY UNLOCK unavailable during power outage."
            );

            return;
        }

        if (visitorWaitingAtDoor == null)
        {
            Debug.Log(
                "No visitor waiting at facility entrance."
            );

            return;
        }

        Visitor admittedVisitor =
            visitorWaitingAtDoor;

        if (facilityDoors != null)
        {
            facilityDoors.OpenDoors();
        }

        if (shiftManager != null)
        {
            shiftManager.RegisterAdmittedVisitor(
                admittedVisitor
            );
        }

        admittedVisitor.UnlockDoor();

        visitorWaitingAtDoor = null;

        Debug.Log(
            "Facility entrance unlocked."
        );
    }

    // ==================================================
    // VISITOR FINISHED
    // ==================================================

    public void VisitorFinished(
        Visitor visitor)
    {
        if (idCard != null)
            idCard.HideCard();

        if (currentVisitor == visitor)
            currentVisitor = null;

        if (visitorWaitingAtDoor == visitor)
            visitorWaitingAtDoor = null;

        decisionMade = false;

        // ----------------------------------------------
        // SPECIAL 6 AM CASE
        //
        // The last cleared visitor has now entered,
        // so the shift is finally allowed to finish.
        // ----------------------------------------------

        if (waitingForFinalClearedVisitor)
        {
            Debug.Log(
                "Final cleared visitor finished entering. Shift can now end."
            );

            FinalizeShiftClosure();

            OnShiftReadyToFinish?.Invoke();

            return;
        }

        if (shiftClosed)
            return;

        if (shiftEnding)
        {
            Debug.Log(
                "Visitor finished. Shift is ending, so no replacement visitor."
            );

            return;
        }

        StartCoroutine(
            SpawnNextVisitor()
        );
    }

    // ==================================================
    // PAUSE / RESUME SPAWNING
    // ==================================================

    public void PauseVisitorSpawning()
    {
        visitorSpawningPaused = true;

        Debug.Log(
            "Visitor spawning paused."
        );
    }

    public void ResumeVisitorSpawning()
    {
        if (shiftEnding ||
            shiftClosed)
        {
            return;
        }

        visitorSpawningPaused =
            false;

        Debug.Log(
            "Visitor spawning resumed."
        );

        if (waitingToSpawnVisitor)
        {
            waitingToSpawnVisitor =
                false;

            StartCoroutine(
                SpawnNextVisitor()
            );
        }
    }

    // ==================================================
    // BLACKOUT REACTION
    // ==================================================

    public void HandleBlackoutVisitor()
    {
        if (shiftClosed)
            return;

        if (currentVisitor == null)
            return;

        if (currentVisitor.Data == null)
            return;

        if (dialogueUI == null)
            return;

        string line;

        if (currentVisitor.IsImpostor)
        {
            string[] impostorLines =
            {
                "Convenient timing.",
                "Guess you'll have to take my word for it.",
                "Power issues? That's unfortunate.",
                "...Everything okay in there?"
            };

            line =
                impostorLines[
                    UnityEngine.Random.Range(
                        0,
                        impostorLines.Length
                    )
                ];
        }
        else
        {
            string[] normalLines =
            {
                "Whoa. What happened?",
                "Did the power just go out?",
                "Uh... is that normal?",
                "Great. Perfect timing."
            };

            line =
                normalLines[
                    UnityEngine.Random.Range(
                        0,
                        normalLines.Length
                    )
                ];
        }

        dialogueUI.ShowDialogue(
            currentVisitor.Data.visitorName,
            line
        );

        Debug.Log(
            "Visitor reacted to blackout."
        );
    }
}