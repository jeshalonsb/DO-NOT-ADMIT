using UnityEngine;

public class FlashlightPickup : Interactable
{
    [Header("Player")]
    [SerializeField] private PlayerFlashlight playerFlashlight;

    [Header("Desk Flashlight")]
    [SerializeField] private GameObject flashlightModel;

    [Header("Game Systems")]
    [SerializeField] private PowerManager powerManager;
    [SerializeField] private VisitorManager visitorManager;

    private bool flashlightUnlocked = false;
    private bool blackoutSequenceActive = false;

    public void UnlockForBlackout()
    {
        flashlightUnlocked = true;
        blackoutSequenceActive = true;

        Debug.Log("Flashlight is now available.");
    }

    public override void Interact()
    {
        if (!flashlightUnlocked)
            return;

        if (playerFlashlight == null)
            return;

        if (!playerFlashlight.HasFlashlight)
        {
            PickUpFlashlight();
        }
        else
        {
            ReturnFlashlight();
        }
    }

    public override void ShowPrompt()
    {
        if (!flashlightUnlocked)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }

    private void PickUpFlashlight()
    {
        playerFlashlight.EquipFlashlight();

        if (flashlightModel != null)
            flashlightModel.SetActive(false);

        Debug.Log("Player picked up flashlight.");
    }

    private void ReturnFlashlight()
    {
        if (blackoutSequenceActive &&
            powerManager != null &&
            !powerManager.PowerOn)
        {
            Debug.Log("Restore power before returning the flashlight.");
            return;
        }

        playerFlashlight.PutAwayFlashlight();

        if (flashlightModel != null)
            flashlightModel.SetActive(true);

        Debug.Log("Player placed flashlight back on desk.");

        if (blackoutSequenceActive)
        {
            blackoutSequenceActive = false;

            if (visitorManager != null)
                visitorManager.ResumeVisitorSpawning();

            Debug.Log("Blackout sequence complete. Shift resumed.");
        }
    }
}