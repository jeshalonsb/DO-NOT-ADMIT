using UnityEngine;

public class Visitor : MonoBehaviour
{
    private enum VisitorState
    {
        MovingToInspection,
        WaitingForDecision,
        MovingToDoor,
        WaitingAtDoor,
        LeavingDenied,
        EnteringFacility
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;

    private VisitorData visitorData;

    public VisitorData Data => visitorData;

    public bool IsImpostor
    {
        get;
        private set;
    }

    public CorrectDecision CorrectDecision
    {
        get;
        private set;
    }

    public string DisplayName
    {
        get;
        private set;
    }

    public string DisplayEmployeeID
    {
        get;
        private set;
    }

    public string DisplayDepartment
    {
        get;
        private set;
    }

    public string DisplayClearance
    {
        get;
        private set;
    }

    public string DisplayStatus
    {
        get;
        private set;
    }

    // ==================================================
    // STATE INFO FOR VISITOR MANAGER
    // ==================================================

    public bool IsUndecided =>
        currentState == VisitorState.MovingToInspection ||
        currentState == VisitorState.WaitingForDecision;

    public bool IsLeavingDenied =>
        currentState == VisitorState.LeavingDenied;

    public bool IsPendingFacilityEntry =>
        currentState == VisitorState.MovingToDoor ||
        currentState == VisitorState.WaitingAtDoor ||
        currentState == VisitorState.EnteringFacility;

    public bool IsWaitingAtFacilityDoor =>
        currentState == VisitorState.WaitingAtDoor;

    public bool IsEnteringFacility =>
        currentState == VisitorState.EnteringFacility;

    private Transform inspectionPoint;
    private Transform doorWaitPoint;
    private Transform entryExitPoint;
    private Transform denyExitPoint;

    private VisitorManager visitorManager;

    private Vector3 targetPosition;

    private VisitorState currentState;

    // ==================================================
    // DATA
    // ==================================================

    public void SetVisitorData(
        VisitorData data)
    {
        visitorData = data;

        DisplayName =
            data.visitorName;

        DisplayEmployeeID =
            data.employeeID;

        DisplayDepartment =
            data.department;

        DisplayClearance =
            data.clearanceLevel;

        DisplayStatus =
            data.employeeStatus;
    }

    public void SetImpostor(
        bool isImpostor)
    {
        IsImpostor = isImpostor;

        CorrectDecision =
            isImpostor
                ? CorrectDecision.Deny
                : CorrectDecision.Clear;

        if (isImpostor)
            GenerateFakeCredential();

        Debug.Log(
            visitorData.visitorName +
            " | Impostor: " +
            IsImpostor
        );
    }

    // ==================================================
    // FAKE CREDENTIAL
    // ==================================================

    private void GenerateFakeCredential()
    {
        int discrepancy =
            Random.Range(0, 4);

        switch (discrepancy)
        {
            case 0:

                DisplayEmployeeID =
                    GenerateWrongID();

                Debug.Log(
                    visitorData.visitorName +
                    " has WRONG ID"
                );

                break;

            case 1:

                DisplayDepartment =
                    GetWrongDepartment();

                Debug.Log(
                    visitorData.visitorName +
                    " has WRONG DEPARTMENT"
                );

                break;

            case 2:

                DisplayClearance =
                    GetWrongClearance();

                Debug.Log(
                    visitorData.visitorName +
                    " has WRONG CLEARANCE"
                );

                break;

            case 3:

                DisplayStatus =
                    "TERMINATED";

                Debug.Log(
                    visitorData.visitorName +
                    " has WRONG STATUS"
                );

                break;
        }
    }

    private string GenerateWrongID()
    {
        if (int.TryParse(
            visitorData.employeeID,
            out int originalID))
        {
            int fakeID =
                originalID;

            while (fakeID == originalID)
            {
                fakeID =
                    originalID +
                    Random.Range(1, 10);
            }

            return fakeID.ToString();
        }

        return
            visitorData.employeeID +
            "9";
    }

    private string GetWrongDepartment()
    {
        string[] departments =
        {
            "Engineering",
            "Research",
            "Security",
            "Maintenance",
            "Administration"
        };

        string fakeDepartment =
            visitorData.department;

        while (
            fakeDepartment ==
            visitorData.department)
        {
            fakeDepartment =
                departments[
                    Random.Range(
                        0,
                        departments.Length
                    )
                ];
        }

        return fakeDepartment;
    }

    private string GetWrongClearance()
    {
        string[] clearances =
        {
            "A",
            "B",
            "C",
            "D"
        };

        string fakeClearance =
            visitorData.clearanceLevel;

        while (
            fakeClearance ==
            visitorData.clearanceLevel)
        {
            fakeClearance =
                clearances[
                    Random.Range(
                        0,
                        clearances.Length
                    )
                ];
        }

        return fakeClearance;
    }

    // ==================================================
    // SETUP
    // ==================================================

    public void Setup(
        Transform inspection,
        Transform doorWait,
        Transform entryExit,
        Transform denyExit,
        VisitorManager manager)
    {
        inspectionPoint =
            inspection;

        doorWaitPoint =
            doorWait;

        entryExitPoint =
            entryExit;

        denyExitPoint =
            denyExit;

        visitorManager =
            manager;

        currentState =
            VisitorState.MovingToInspection;

        targetPosition =
            inspectionPoint.position;
    }

    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        if (!IsMoving())
            return;

        MoveVisitor();
    }

    private bool IsMoving()
    {
        return
            currentState ==
            VisitorState.MovingToInspection ||

            currentState ==
            VisitorState.MovingToDoor ||

            currentState ==
            VisitorState.LeavingDenied ||

            currentState ==
            VisitorState.EnteringFacility;
    }

    // ==================================================
    // MOVEMENT
    // ==================================================

    private void MoveVisitor()
    {
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed *
                Time.deltaTime
            );

        Vector3 direction =
            targetPosition -
            transform.position;

        direction.y = 0f;

        if (
            direction.sqrMagnitude >
            0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );
        }

        if (
            Vector3.Distance(
                transform.position,
                targetPosition
            ) < 0.05f)
        {
            ArrivedAtTarget();
        }
    }

    // ==================================================
    // ARRIVAL
    // ==================================================

    private void ArrivedAtTarget()
    {
        switch (currentState)
        {
            case VisitorState
                .MovingToInspection:

                currentState =
                    VisitorState
                        .WaitingForDecision;

                Debug.Log(
                    "Visitor waiting for inspection."
                );

                if (visitorManager != null)
                {
                    visitorManager
                        .VisitorReady(this);
                }

                break;

            case VisitorState
                .MovingToDoor:

                currentState =
                    VisitorState
                        .WaitingAtDoor;

                Debug.Log(
                    "Visitor waiting at facility entrance."
                );

                if (visitorManager != null)
                {
                    visitorManager
                        .VisitorWaitingAtDoor(this);
                }

                break;

            case VisitorState
                .LeavingDenied:

                Debug.Log(
                    "Visitor has left."
                );

                if (visitorManager != null)
                {
                    visitorManager
                        .VisitorFinished(this);
                }

                Destroy(gameObject);

                break;

            case VisitorState
                .EnteringFacility:

                Debug.Log(
                    "Visitor entered facility."
                );

                if (visitorManager != null)
                {
                    visitorManager
                        .VisitorFinished(this);
                }

                Destroy(gameObject);

                break;
        }
    }

    // ==================================================
    // PLAYER DECISIONS
    // ==================================================

    public void Clear()
    {
        if (
            currentState !=
            VisitorState.WaitingForDecision)
            return;

        Debug.Log(
            "Visitor cleared."
        );

        currentState =
            VisitorState.MovingToDoor;

        targetPosition =
            doorWaitPoint.position;
    }

    public void Deny()
    {
        if (
            currentState !=
            VisitorState.WaitingForDecision)
            return;

        Debug.Log(
            "Visitor denied."
        );

        currentState =
            VisitorState.LeavingDenied;

        targetPosition =
            denyExitPoint.position;
    }

    public void UnlockDoor()
    {
        if (
            currentState !=
            VisitorState.WaitingAtDoor)
        {
            Debug.LogWarning(
                "Visitor is not waiting at the facility door."
            );

            return;
        }

        currentState =
            VisitorState.EnteringFacility;

        targetPosition =
            entryExitPoint.position;

        Debug.Log(
            "Visitor entering facility."
        );
    }

    // ==================================================
    // SHIFT END
    // ==================================================

    public void LeaveForShiftEnd()
    {
        if (denyExitPoint == null)
        {
            Debug.LogWarning(
                "Visitor has no deny exit point."
            );

            Destroy(gameObject);
            return;
        }

        currentState =
            VisitorState.LeavingDenied;

        targetPosition =
            denyExitPoint.position;

        Debug.Log(
            visitorData.visitorName +
            " is leaving because the shift ended."
        );
    }
}