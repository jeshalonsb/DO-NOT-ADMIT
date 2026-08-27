using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    [Header("Visitor")]
    [SerializeField] private GameObject visitorPrefab;

    [Header("Visitor Queue")]
    [SerializeField] private VisitorData[] visitors;

    [Header("Visitor Points")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private Transform doorWaitPoint;
    [SerializeField] private Transform entryExitPoint;
    [SerializeField] private Transform denyExitPoint;

    [Header("Timing")]
    [SerializeField] private float timeBetweenVisitors = 2f;

    [Header("ID Card")]
    [SerializeField] private IDCard idCard;

    [Header("Dialogue")]
    [SerializeField] private VisitorDialogueUI dialogueUI;

    [Header("Shift")]
    [SerializeField] private ShiftManager shiftManager;

    [Header("Facility Doors")]
    [SerializeField] private FacilityDoors facilityDoors;

    [Header("Impostor Settings")]
    [SerializeField] private int minimumImpostors = 1;
    [SerializeField] private int maximumImpostors = 2;

    [SerializeField] private ShiftClock shiftClock;

    private bool visitorSpawningPaused;
    private bool waitingToSpawnVisitor;

    private readonly HashSet<int> impostorIndices =
        new HashSet<int>();

    private Visitor currentVisitor;
    private Visitor visitorWaitingAtDoor;

    private int currentVisitorIndex;

    private bool shiftStarted;

    private bool decisionMade;

    private void Start()
    {
        StartShift();
    }

    public void StartShift()
    {
        if (shiftStarted)
            return;

        shiftStarted = true;
        currentVisitorIndex = 0;

        GenerateImpostors();

        if (shiftClock != null)
            shiftClock.StartClock();

        Debug.Log("SHIFT STARTED");

        SpawnVisitor();
    }

    private void GenerateImpostors()
    {
        impostorIndices.Clear();

        if (visitors == null || visitors.Length == 0)
        {
            Debug.LogWarning("No visitors assigned.");
            return;
        }

        int minimum = Mathf.Clamp(
            minimumImpostors,
            1,
            visitors.Length
        );

        int maximum = Mathf.Clamp(
            maximumImpostors,
            minimum,
            visitors.Length
        );

        int impostorCount = Random.Range(
            minimum,
            maximum + 1
        );

        while (impostorIndices.Count < impostorCount)
        {
            int randomIndex =
                Random.Range(0, visitors.Length);

            impostorIndices.Add(randomIndex);
        }

        Debug.Log(
            "GENERATED " +
            impostorIndices.Count +
            " IMPOSTORS"
        );

        // DEVELOPMENT ONLY.
        // Remove these logs for the final build.
        foreach (int index in impostorIndices)
        {
            Debug.Log(
                "IMPOSTOR = " +
                visitors[index].visitorName
            );
        }
    }

    private void SpawnVisitor()
    {
        if (currentVisitorIndex >= visitors.Length)
        {
            Debug.Log("ALL VISITORS PROCESSED");
            return;
        }

        int visitorIndex = currentVisitorIndex;

        VisitorData nextVisitorData =
            visitors[visitorIndex];

        GameObject visitorObject = Instantiate(
            visitorPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        currentVisitor =
            visitorObject.GetComponent<Visitor>();

        if (currentVisitor == null)
        {
            Debug.LogError(
                "Visitor prefab does not contain Visitor.cs!"
            );

            Destroy(visitorObject);
            return;
        }

        // Give them their real identity first.
        currentVisitor.SetVisitorData(
            nextVisitorData
        );

        // Decide if THIS visitor is an impostor this shift.
        bool isImpostor =
            impostorIndices.Contains(visitorIndex);

        currentVisitor.SetImpostor(
            isImpostor
        );

        currentVisitor.Setup(
            inspectionPoint,
            doorWaitPoint,
            entryExitPoint,
            denyExitPoint,
            this
        );

        Debug.Log(
            "SPAWNING: " +
            nextVisitorData.visitorName +
            " | IMPOSTOR: " +
            isImpostor
        );

        currentVisitorIndex++;
    }

    public void VisitorReady(Visitor visitor)
    {
        currentVisitor = visitor;
        decisionMade = false;

        if (idCard != null)
        {
            idCard.DisplayVisitor(visitor);
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                visitor.Data.visitorName,
                visitor.Data.arrivalDialogue
            );
        }
    }

    public void ClearCurrentVisitor()
    {
        if (currentVisitor == null || decisionMade)
            return;

        decisionMade = true;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                currentVisitor.CorrectDecision,
                CorrectDecision.Clear
            );
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                currentVisitor.Data.visitorName,
                currentVisitor.Data.clearDialogue
            );
        }

        currentVisitor.Clear();
    }

    public void DenyCurrentVisitor()
    {
        if (currentVisitor == null || decisionMade)
            return;

        decisionMade = true;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                currentVisitor.CorrectDecision,
                CorrectDecision.Deny
            );
        }

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                currentVisitor.Data.visitorName,
                currentVisitor.Data.denyDialogue
            );
        }

        currentVisitor.Deny();
    }

    public void VisitorWaitingAtDoor(
        Visitor visitor)
    {
        visitorWaitingAtDoor = visitor;

        Debug.Log(
            "Visitor ready for facility door unlock."
        );
    }

    public void UnlockFacilityDoor()
    {
        if (visitorWaitingAtDoor == null)
        {
            Debug.LogWarning(
                "No visitor currently waiting at facility door."
            );

            return;
        }

        if (facilityDoors != null)
        {
            facilityDoors.OpenDoors();
        }

        visitorWaitingAtDoor.UnlockDoor();

        visitorWaitingAtDoor = null;
    }

    public void VisitorFinished()
    {
        if (idCard != null)
        {
            idCard.HideCard();
        }

        currentVisitor = null;

        StartCoroutine(
            SpawnNextVisitor()
        );
    }

    private IEnumerator SpawnNextVisitor()
    {
        yield return new WaitForSeconds(timeBetweenVisitors);

        if (visitorSpawningPaused)
        {
            waitingToSpawnVisitor = true;

            Debug.Log("Next visitor waiting because visitor spawning is paused.");

            yield break;
        }

        SpawnVisitor();
    }
    public void PauseVisitorSpawning()
    {
        visitorSpawningPaused = true;

        Debug.Log("Visitor spawning paused.");
    }

    public void ResumeVisitorSpawning()
    {
        visitorSpawningPaused = false;

        Debug.Log("Visitor spawning resumed.");

        if (waitingToSpawnVisitor)
        {
            waitingToSpawnVisitor = false;
            StartCoroutine(SpawnNextVisitor());
        }
    }
}