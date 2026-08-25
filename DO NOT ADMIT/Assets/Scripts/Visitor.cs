using UnityEngine;

public class Visitor : MonoBehaviour
{
    [Header("Visitor Information")]
    [SerializeField] private VisitorData visitorData; 

    public VisitorData Data => visitorData;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform inspectionPoint;
    private Transform admitExitPoint;
    private Transform denyExitPoint;

    private VisitorManager visitorManager;

    private Vector3 targetPosition;
    private bool isMoving;
    private bool waitingForDecision;
    private bool isLeaving;

    public void Setup(
        Transform inspection,
        Transform admitExit,
        Transform denyExit,
        VisitorManager manager)
    {
        inspectionPoint = inspection;
        admitExitPoint = admitExit;
        denyExitPoint = denyExit;
        visitorManager = manager;

        targetPosition = inspectionPoint.position;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                8f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            ArrivedAtTarget();
        }
    }

    private void ArrivedAtTarget()
    {
        isMoving = false;

        // Visitor arrived at inspection window
        if (!isLeaving)
        {
            waitingForDecision = true;

            Debug.Log("Visitor is waiting for inspection.");

            if (visitorManager != null)
            {
                Debug.Log("Telling VisitorManager that visitor is ready.");
                visitorManager.VisitorReady(this);
            }
            else
            {
                Debug.LogError("VISITOR MANAGER IS NULL!");
            }

            return;
        }

        // Visitor finished leaving
        if (visitorManager != null)
        {
            visitorManager.VisitorFinished();
        }

        Destroy(gameObject);
    }

    public void Admit()
    {
        if (!waitingForDecision)
            return;

        waitingForDecision = false;
        isLeaving = true;

        targetPosition = admitExitPoint.position;
        isMoving = true;

        Debug.Log("Visitor admitted.");
    }

    public void Deny()
    {
        if (!waitingForDecision)
            return;

        waitingForDecision = false;
        isLeaving = true;

        targetPosition = denyExitPoint.position;
        isMoving = true;

        Debug.Log("Visitor denied.");
    }
}