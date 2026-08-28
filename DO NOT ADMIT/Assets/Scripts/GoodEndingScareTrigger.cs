using UnityEngine;

public class GoodEndingScareTrigger : MonoBehaviour
{
    [SerializeField]
    private EndingController endingController;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        if (endingController != null)
            endingController.TriggerSupervisorScare();
    }
}