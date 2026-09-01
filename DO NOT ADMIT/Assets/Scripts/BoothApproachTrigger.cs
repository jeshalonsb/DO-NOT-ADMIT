using UnityEngine;

public class BoothApproachTrigger : MonoBehaviour
{
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        CharacterController playerController =
            other.GetComponentInParent<CharacterController>();

        if (playerController == null)
            return;

        Debug.Log("BOOTH APPROACH TRIGGER WORKED");

        triggered = true;

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance
                .PlayerApproachedBooth();
        }
    }
}