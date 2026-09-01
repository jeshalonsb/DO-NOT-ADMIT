using UnityEngine;

public class FacilityDoorButton : Interactable
{
    [Header("References")]
    [SerializeField] private VisitorManager visitorManager;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonSound;

    public override void Interact()
    {
        base.Interact();

        if (visitorManager == null)
            return;

        PlayButtonSound();

        visitorManager.UnlockFacilityDoor();
    }

    private void PlayButtonSound()
    {
        if (buttonAudioSource == null || buttonSound == null)
            return;

        buttonAudioSource.PlayOneShot(buttonSound);
    }
}