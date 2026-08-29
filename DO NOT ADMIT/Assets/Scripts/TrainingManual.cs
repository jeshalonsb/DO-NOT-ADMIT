using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrainingManual : Interactable
{
    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager;

    [Header("Manual UI")]
    [SerializeField] private GameObject manualCanvas;

    [Header("Physical Manual")]
    [SerializeField] private GameObject manualModel;

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject manualPrompt;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;

    private bool manualOpen;
    private bool canCloseManual;
    private bool hasBeenReadOnce;
    private bool playerLooking;

    protected override void Start()
    {
        // Don't use the normal Interactable prompt system
        // for this object.

        if (manualCanvas != null)
            manualCanvas.SetActive(false);

        if (manualPrompt != null)
            manualPrompt.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // ----------------------------
        // MANUAL IS OPEN
        // ----------------------------

        if (manualOpen)
        {
            if (canCloseManual &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                CloseManual();
            }

            return;
        }

        // ----------------------------
        // MANUAL IS ON DESK
        // ----------------------------

        UpdatePrompt();
    }

    // ==================================================
    // PLAYER LOOKING
    // ==================================================

    public void SetPlayerLooking(bool looking)
    {
        playerLooking = looking;

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (manualPrompt == null)
            return;

        if (manualOpen)
        {
            manualPrompt.SetActive(false);
            return;
        }

        manualPrompt.SetActive(playerLooking);
    }

    // ==================================================
    // INTERACT
    // ==================================================

    public override void Interact()
    {
        if (manualOpen)
            return;

        OpenManual();
    }

    // ==================================================
    // OPEN MANUAL
    // ==================================================

    private void OpenManual()
    {
        manualOpen = true;
        canCloseManual = false;

        if (manualPrompt != null)
            manualPrompt.SetActive(false);

        // Hide ONLY the physical book.
        if (manualModel != null)
            manualModel.SetActive(false);

        // Show fullscreen/manual UI.
        if (manualCanvas != null)
            manualCanvas.SetActive(true);

        // Stop player movement while reading.
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        StartCoroutine(WaitForERelease());

        Debug.Log("Manual opened.");
    }

    private IEnumerator WaitForERelease()
    {
        while (Keyboard.current != null &&
               Keyboard.current.eKey.isPressed)
        {
            yield return null;
        }

        canCloseManual = true;
    }

    // ==================================================
    // CLOSE MANUAL
    // ==================================================

    private void CloseManual()
    {
        manualOpen = false;
        canCloseManual = false;

        if (manualCanvas != null)
            manualCanvas.SetActive(false);

        if (manualModel != null)
            manualModel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        // Only first reading advances opening objective.
        if (!hasBeenReadOnce)
        {
            hasBeenReadOnce = true;

            if (gameFlowManager != null)
                gameFlowManager.ReadManual();

            Debug.Log("Manual read. Clock-in unlocked.");
        }

        // Don't automatically show prompt here.
        // PlayerInteraction will handle it when looking.
        UpdatePrompt();
    }

    // ==================================================
    // IGNORE NORMAL PROMPT SYSTEM
    // ==================================================

    public override void ShowPrompt()
    {
        // Intentionally empty.
    }

    public override void HidePrompt()
    {
        // Intentionally empty.
    }
}