using System.Collections;
using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    [Header("Visitor")]
    [SerializeField] private GameObject visitorPrefab;

    [Header("Visitor Points")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private Transform doorWaitPoint;
    [SerializeField] private Transform entryExitPoint;
    [SerializeField] private Transform denyExitPoint;

    private Visitor visitorWaitingAtDoor;

    [Header("Timing")]
    [SerializeField] private float timeBetweenVisitors = 2f;

    [Header("ID Card")]
    [SerializeField] private IDCard idCard;

    [Header("Visitor Queue")]
    [SerializeField] private VisitorData[] visitors;

    [Header("Facility Door")]
    [SerializeField] private FacilityDoors facilityDoors;

    [Header("Shift")]
    [SerializeField] private ShiftManager shiftManager;

    private int currentVisitorIndex = 0;

    private Visitor currentVisitor;

    private void Start()
    {
        SpawnVisitor();
    }

    private void SpawnVisitor()
    {
        if (currentVisitorIndex >= visitors.Length)
        {
            Debug.Log("SHIFT COMPLETE - All visitors processed.");
            return;
        }

        VisitorData nextVisitorData = visitors[currentVisitorIndex];

        GameObject visitorObject = Instantiate(
            visitorPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        currentVisitor = visitorObject.GetComponent<Visitor>();

        currentVisitor.SetVisitorData(nextVisitorData);

        currentVisitor.Setup(
            inspectionPoint,
            doorWaitPoint,
            entryExitPoint,
            denyExitPoint,
            this
        );

        Debug.Log("Visitor arriving: " + nextVisitorData.visitorName);

        currentVisitorIndex++;
    }

    public void VisitorReady(Visitor visitor)
    {
        currentVisitor = visitor;

        if ( idCard != null ) 
            idCard.DisplayVisitor(visitor.Data);
    }

    public void ClearCurrentVisitor()
    {
        if (currentVisitor == null)
            return;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                CorrectDecision.Clear
            );
        }

        currentVisitor.Clear();
    }

    public void VisitorWaitingAtDoor(Visitor visitor)
    {
        visitorWaitingAtDoor = visitor;

        Debug.Log("Visitor ready for facility door unlock.");
    }

    public void DenyCurrentVisitor()
    {
        if (currentVisitor == null)
            return;

        if (shiftManager != null)
        {
            shiftManager.RegisterDecision(
                currentVisitor.Data,
                CorrectDecision.Deny
            );
        }

        currentVisitor.Deny();
    }

    public void UnlockFacilityDoor()
    {
        if (visitorWaitingAtDoor == null)
        {
            Debug.Log("No visitor is currently waiting at the facility door.");
            return;
        }

        // Open physical doors
        if (facilityDoors != null)
        {
            facilityDoors.OpenDoors();
        }

        // Send visitor inside
        visitorWaitingAtDoor.UnlockDoor();

        visitorWaitingAtDoor = null;
    }

    public void VisitorFinished()
    {
        if (idCard != null)
            idCard.HideCard();

        currentVisitor = null;

        StartCoroutine(SpawnNextVisitor());
    }

    private IEnumerator SpawnNextVisitor()
    {
        yield return new WaitForSeconds(timeBetweenVisitors);

        SpawnVisitor();
    }
}