using System.Collections;
using UnityEngine;

public class FacilityDoors : MonoBehaviour
{
    [Header("Door References")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Open Position References")]
    [SerializeField] private Transform leftDoorOpenPoint;
    [SerializeField] private Transform rightDoorOpenPoint;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stayOpenTime = 3f;

    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;

    private Coroutine doorRoutine;

    private void Awake()
    {
        leftClosedPosition = leftDoor.position;
        rightClosedPosition = rightDoor.position;
    }

    public void OpenDoors()
    {
        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

        doorRoutine = StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        // Open
        while (
            Vector3.Distance(leftDoor.position, leftDoorOpenPoint.position) > 0.01f ||
            Vector3.Distance(rightDoor.position, rightDoorOpenPoint.position) > 0.01f
        )
        {
            leftDoor.position = Vector3.MoveTowards(
                leftDoor.position,
                leftDoorOpenPoint.position,
                moveSpeed * Time.deltaTime
            );

            rightDoor.position = Vector3.MoveTowards(
                rightDoor.position,
                rightDoorOpenPoint.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        leftDoor.position = leftDoorOpenPoint.position;
        rightDoor.position = rightDoorOpenPoint.position;

        yield return new WaitForSeconds(stayOpenTime);

        // Close
        while (
            Vector3.Distance(leftDoor.position, leftClosedPosition) > 0.01f ||
            Vector3.Distance(rightDoor.position, rightClosedPosition) > 0.01f
        )
        {
            leftDoor.position = Vector3.MoveTowards(
                leftDoor.position,
                leftClosedPosition,
                moveSpeed * Time.deltaTime
            );

            rightDoor.position = Vector3.MoveTowards(
                rightDoor.position,
                rightClosedPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        leftDoor.position = leftClosedPosition;
        rightDoor.position = rightClosedPosition;

        doorRoutine = null;
    }
}