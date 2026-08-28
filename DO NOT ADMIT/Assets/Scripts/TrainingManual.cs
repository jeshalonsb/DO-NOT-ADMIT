using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrainingManual : Interactable
{
    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager;

    [Header("Manual UI")]
    [SerializeField] private GameObject manualCanvas;

    [Header("Manual Object")]
    [SerializeField] private GameObject manualModel;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;

    private bool manualOpen;
    private bool canCloseManual;
    private bool hasBeenRead;

    protected override void Start()
    {
        base.Start();

        if (manualCanvas != null)
            manualCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!manualOpen || !canCloseManual)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CloseManual();
        }
    }

    public override void ShowPrompt()
    {
        if (hasBeenRead || manualOpen)
        {
            HidePrompt();
            return;
        }

        if (gameFlowManager != null &&
            gameFlowManager.ShiftStarted)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }

    public override void Interact()
    {
        if (manualOpen || hasBeenRead)
            return;

        if (gameFlowManager == null)
            return;

        OpenManual();
    }

    private void OpenManual()
    {
        manualOpen = true;
        canCloseManual = false;

        HidePrompt();

        if (manualModel != null)
            manualModel.SetActive(false);

        if (manualCanvas != null)
            manualCanvas.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        Debug.Log("Manual opened.");

        StartCoroutine(EnableClosingNextFrame());
    }

    private IEnumerator EnableClosingNextFrame()
    {
        // Wait until the E press that opened the manual is over.
        yield return null;

        canCloseManual = true;
    }

    private void CloseManual()
    {
        manualOpen = false;
        canCloseManual = false;
        hasBeenRead = true;

        if (manualCanvas != null)
            manualCanvas.SetActive(false);

        if (manualModel != null)
            manualModel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        if (gameFlowManager != null)
            gameFlowManager.ReadManual();

        HidePrompt();

        Debug.Log("Manual read. Clock-in unlocked.");
    }
}