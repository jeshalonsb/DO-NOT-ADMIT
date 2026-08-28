using System.Collections;
using UnityEngine;

public class SwingDoor : Interactable
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;

    [Header("Rotation")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 4f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool doorOpen;
    private bool moving;

    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localRotation;

        openRotation =
            closedRotation *
            Quaternion.Euler(
                0f,
                openAngle,
                0f
            );
    }

    public override void Interact()
    {
        Debug.Log("BOOTH DOOR INTERACTED");

        if (moving)
            return;

        doorOpen = !doorOpen;

        StopAllCoroutines();

        StartCoroutine(
            RotateDoor(
                doorOpen
                    ? openRotation
                    : closedRotation
            )
        );
    }

    private IEnumerator RotateDoor(
        Quaternion targetRotation)
    {
        moving = true;

        while (
            Quaternion.Angle(
                doorPivot.localRotation,
                targetRotation
            ) > 0.5f)
        {
            doorPivot.localRotation =
                Quaternion.Slerp(
                    doorPivot.localRotation,
                    targetRotation,
                    openSpeed * Time.deltaTime
                );

            yield return null;
        }

        doorPivot.localRotation =
            targetRotation;

        moving = false;
    }
}
