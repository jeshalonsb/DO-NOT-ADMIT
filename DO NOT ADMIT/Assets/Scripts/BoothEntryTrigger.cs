using UnityEngine;

public class BoothEntryTrigger : MonoBehaviour
{
    [Header("Game Flow")]
    [SerializeField]
    private GameFlowManager gameFlowManager;

    [Header("Booth Door")]
    [SerializeField]
    private SlidingDoor boothDoor;

    private void OnTriggerEnter(
        Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (gameFlowManager != null)
        {
            gameFlowManager.PlayerEnteredBooth();
        }

        if (boothDoor != null)
        {
            boothDoor.AutoCloseDoor();
        }
    }
}