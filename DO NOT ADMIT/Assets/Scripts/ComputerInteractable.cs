using UnityEngine;

public class ComputerInteractable : Interactable
{
    [Header("Computer")]
    [SerializeField] private GameObject computerCanvas;

    [SerializeField] private GameObject image; 

    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMovement playerMovement;

    private bool computerOpen;

    public override void Interact()
    {
        if (computerOpen)
            return;

        OpenComputer();
    }

    public void OpenComputer()
    {
        computerOpen = true;

        computerCanvas.SetActive(true);

        image.SetActive(false);

        if (playerLook != null)
            playerLook.enabled = false;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseComputer()
    {
        computerOpen = false;

        computerCanvas.SetActive(false);

        image.SetActive(true);

        if (playerLook != null)
            playerLook.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}