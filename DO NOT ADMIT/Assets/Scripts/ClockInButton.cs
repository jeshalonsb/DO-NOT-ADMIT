using UnityEngine;

public class ClockInButton : Interactable
{
    [Header("References")]
    [SerializeField] private GameFlowManager gameFlowManager;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonSound;

    private bool clockedIn;

    public override void Interact()
    {
        if (clockedIn)
            return;

        if (gameFlowManager == null)
            return;

        if (!gameFlowManager.ManualRead)
        {
            Debug.Log("Read the first day manual before clocking in.");
            return;
        }

        PlayButtonSound();

        gameFlowManager.ClockIn();

        clockedIn = true;

        Debug.Log("Clock-in button pressed.");
    }

    private void PlayButtonSound()
    {
        if (buttonAudioSource == null || buttonSound == null)
            return;

        buttonAudioSource.PlayOneShot(buttonSound);
    }
}