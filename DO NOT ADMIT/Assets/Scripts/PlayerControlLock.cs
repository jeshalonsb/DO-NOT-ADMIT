using UnityEngine;

public class PlayerControlLock : MonoBehaviour
{
    [Header("Player Scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;

    public void LockControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        Debug.Log("Player controls locked.");
    }

    public void UnlockControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        Debug.Log("Player controls unlocked.");
    }
}