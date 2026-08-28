using UnityEngine;

public class CarEndingTrigger : MonoBehaviour
{
    [SerializeField]
    private EndingController endingController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (endingController != null)
            endingController.PlayerReachedCar();
    }
}