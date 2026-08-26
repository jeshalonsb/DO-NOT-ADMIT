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

    [Header("Visitor Information")]
    private VisitorData visitorData;

    public VisitorData Data => visitorData;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;

    private Transform inspectionPoint;
    private Transform doorWaitPoint;
    private Transform entryExitPoint;
    private Transform denyExitPoint;

    private VisitorManager visitorManager;

    private Vector3 targetPosition;

    private VisitorState currentState;

    public void SetVisitorData(VisitorData data)
    {
        visitorData = data;
    }

    public void Setup(
        Transform inspection,
        Transform doorWait,
        Transform entryExit,
        Transform denyExit,
        VisitorManager manager)
    {
        inspectionPoint = inspection;
        doorWaitPoint = doorWait;
        entryExitPoint = entryExit;
        denyExitPoint = denyExit;

        visitorManager = manager;

        currentState = VisitorState.MovingToInspection;

        targetPosition = inspectionPoint.position;
    }

    private void Update()
    {
        if (!IsMoving())
            return;

        MoveVisitor();
    }

    private bool IsMoving()
    {
        return currentState == VisitorState.MovingToInspection ||
               currentState == VisitorState.MovingToDoor ||
               currentState == VisitorState.LeavingDenied ||
               currentState == VisitorState.EnteringFacility;
    }

    private void MoveVisitor()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            ArrivedAtTarget();
        }
    }

    private void ArrivedAtTarget()
    {
        switch (currentState)
        {
            // Reached booth window
            case VisitorState.MovingToInspection:

                currentState = VisitorState.WaitingForDecision;

                Debug.Log("Visitor is waiting for inspection.");

                visitorManager.VisitorReady(this);

                break;


            // Reached facility entrance
            case VisitorState.MovingToDoor:

                currentState = VisitorState.WaitingAtDoor;

                Debug.Log("Visitor is waiting at facility entrance.");

                visitorManager.VisitorWaitingAtDoor(this);

                break;


            // Finished walking away after denial
            case VisitorState.LeavingDenied:

                Debug.Log("Denied visitor has left.");

                visitorManager.VisitorFinished();

                Destroy(gameObject);

                break;


            // Finished entering facility
            case VisitorState.EnteringFacility:

                Debug.Log("Visitor entered facility.");

                visitorManager.VisitorFinished();

                Destroy(gameObject);

                break;
        }
    }

    public void Clear()
    {
        if (currentState != VisitorState.WaitingForDecision)
            return;

        Debug.Log("Visitor cleared. Walking to facility entrance.");

        currentState = VisitorState.MovingToDoor;

        targetPosition = doorWaitPoint.position;
    }

    public void Deny()
    {
        if (currentState != VisitorState.WaitingForDecision)
            return;

        Debug.Log("Visitor denied.");

        currentState = VisitorState.LeavingDenied;

        targetPosition = denyExitPoint.position;
    }

    public void UnlockDoor()
    {
        if (currentState != VisitorState.WaitingAtDoor)
        {
            Debug.LogWarning(
                "Visitor cannot enter because they are not waiting at the facility door."
            );

            return;
        }

        Debug.Log("Visitor entering facility.");

        currentState = VisitorState.EnteringFacility;

        targetPosition = entryExitPoint.position;
    }
}