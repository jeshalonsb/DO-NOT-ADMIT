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

        if (Keyboard.current.eKey.isPressed)
        {
            if (progressUI != null)
                progressUI.SetActive(true);

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

    private void CancelHold()
    {
        holdTimer = 0f;

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

        Debug.Log("BREAKER RESET");

        powerManager.RestorePower();

        if (progressUI != null)
            progressUI.SetActive(false);

        HidePrompt();
    }
}