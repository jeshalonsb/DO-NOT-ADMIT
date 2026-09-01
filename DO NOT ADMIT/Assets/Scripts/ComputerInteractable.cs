using UnityEngine;

public class ComputerInteractable : Interactable
{
    [Header("Computer")]
    [SerializeField] private GameObject computerCanvas;

    [Header("References")]
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Audio")]
    [SerializeField] private AudioSource computerAudioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool computerOpen;
    private bool powered = true;

    public override void Interact()
    {
        if (!powered)
        {
            Debug.Log("Computer has no power.");
            return;
        }

        if (computerOpen)
            return;

        OpenComputer();
    }

    public void OpenComputer()
    {
        if (!powered)
            return;

        computerOpen = true;

        PlaySound(openSound);

        if (computerCanvas != null)
            computerCanvas.SetActive(true);

        if (playerLook != null)
            playerLook.enabled = false;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseComputer()
    {
        if (!computerOpen)
            return;

        computerOpen = false;

        PlaySound(closeSound);

        if (computerCanvas != null)
            computerCanvas.SetActive(false);

        if (playerLook != null)
            playerLook.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetPowered(bool state)
    {
        powered = state;

        // If blackout happens while player is using computer,
        // safely kick them back into gameplay.
        if (!powered && computerOpen)
        {
            CloseComputer();
        }

        Debug.Log(
            powered
                ? "Computer powered on."
                : "Computer powered off."
        );
    }

    private void PlaySound(AudioClip sound)
    {
        if (computerAudioSource == null)
            return;

        if (sound == null)
            return;

        computerAudioSource.PlayOneShot(sound);
    }
}