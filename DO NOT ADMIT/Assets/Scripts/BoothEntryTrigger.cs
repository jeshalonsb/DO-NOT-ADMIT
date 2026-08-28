using UnityEngine;

public class BoothEntryTrigger : MonoBehaviour
{
    [SerializeField] private GameFlowManager gameFlowManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (gameFlowManager != null)
            gameFlowManager.PlayerEnteredBooth();
    }
}