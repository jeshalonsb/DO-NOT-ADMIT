using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BreakerBox : Interactable
{
    [Header("Power")]
    [SerializeField] private PowerManager powerManager;

    [Header("Hold Interaction")]
    [SerializeField] private float holdDuration = 3f;

    [Header("UI")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private Image progressFill;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip repairLoopSound;
    [SerializeField] private AudioClip resetSound;

    private float holdTimer;
    private bool playerLookingAtBreaker;
    private bool breakerReset;

    private void Start()
    {
        HidePrompt();

        if (progressUI != null)
            progressUI.SetActive(false);

        if (progressFill != null)
            progressFill.fillAmount = 0f;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public override void ShowPrompt()
    {
        if (powerManager == null ||
            powerManager.PowerOn ||
            breakerReset)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }

    public override void Interact()
    {
        // We aren't using normal press interaction for this object.
    }

    public void SetPlayerLooking(bool looking)
    {
        playerLookingAtBreaker = looking;

        if (!looking)
        {
            CancelHold();
            HidePrompt();
        }
    }

    private void Update()
    {
        if (!playerLookingAtBreaker)
            return;

        if (breakerReset)
            return;

        if (powerManager == null || powerManager.PowerOn)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.isPressed)
        {
            if (progressUI != null)
                progressUI.SetActive(true);

            StartRepairSound();

            holdTimer += Time.deltaTime;

            float progress =
                holdTimer / holdDuration;

            if (progressFill != null)
                progressFill.fillAmount = progress;

            if (holdTimer >= holdDuration)
            {
                ResetBreaker();
            }
        }
        else if (holdTimer > 0f)
        {
            CancelHold();
        }
    }

    private void StartRepairSound()
    {
        if (audioSource == null)
            return;

        if (repairLoopSound == null)
            return;

        if (audioSource.isPlaying)
            return;

        audioSource.clip = repairLoopSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopRepairSound()
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.loop = false;
        audioSource.clip = null;
    }

    private void CancelHold()
    {
        holdTimer = 0f;

        StopRepairSound();

        if (progressFill != null)
            progressFill.fillAmount = 0f;

        if (progressUI != null)
            progressUI.SetActive(false);
    }

    private void ResetBreaker()
    {
        breakerReset = true;

        holdTimer = holdDuration;

        if (progressFill != null)
            progressFill.fillAmount = 1f;

        StopRepairSound();

        if (audioSource != null &&
            resetSound != null)
        {
            audioSource.PlayOneShot(resetSound);
        }

        Debug.Log("BREAKER RESET");

        if (powerManager != null)
            powerManager.RestorePower();

        if (progressUI != null)
            progressUI.SetActive(false);

        HidePrompt();
    }
}