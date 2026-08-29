using System.Collections;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [Header("Lights That Restore")]
    [Tooltip(
        "These lights turn back on after the breaker is reset."
    )]
    [SerializeField]
    private Light[] restoringLights;

    [Header("Lights That Fail")]
    [Tooltip(
        "These lights turn off during the blackout and stay off."
    )]
    [SerializeField]
    private Light[] failedLights;

    [Header("Powered Displays")]
    [Tooltip(
        "Clock and other powered displays. Do NOT put CCTV feeds here."
    )]
    [SerializeField]
    private GameObject[] poweredDisplays;

    [Header("Cameras")]
    [SerializeField]
    private SecurityCameraSystem securityCameraSystem;

    [Header("Computer")]
    [SerializeField]
    private ComputerInteractable computer;

    [Header("Phone")]
    [SerializeField]
    private Phone phone;

    [Header("Flashlight")]
    [SerializeField]
    private FlashlightPickup flashlightPickup;

    [Header("Visitor Flow")]
    [SerializeField]
    private VisitorManager visitorManager;

    [Header("Shift Clock")]
    [SerializeField]
    private ShiftClock shiftClock;

    [Header("Game Flow")]
    [SerializeField]
    private GameFlowManager gameFlowManager;

    [Header("Player Dialogue")]
    [SerializeField]
    private PlayerDialogueController playerDialogue;

    [Header("Blackout Dialogue")]
    [SerializeField]
    private float visitorReactionDelay = 3.5f;

    [Header("Flickering Lights")]
    [SerializeField]
    private LightFlicker[] flickeringLights;

    private bool powerOn = true;

    public bool PowerOn => powerOn;

    // ==================================================
    // BLACKOUT
    // ==================================================

    public void CutPower()
    {
        if (!powerOn)
            return;

        powerOn = false;

        Debug.Log(
            "POWER FAILURE"
        );

        if (shiftClock != null)
            shiftClock.PauseClock(
                "Power"
            );

        if (gameFlowManager != null)
            gameFlowManager.BlackoutStarted();

        SetLights(
            restoringLights,
            false
        );

        SetLights(
            failedLights,
            false
        );

        SetPoweredDisplays(
            false
        );

        if (securityCameraSystem != null)
        {
            securityCameraSystem
                .DisableAllCameras();
        }

        if (computer != null)
            computer.SetPowered(false);

        if (phone != null)
            phone.SetPowered(false);

        if (flashlightPickup != null)
        {
            flashlightPickup
                .UnlockForBlackout();
        }

        if (visitorManager != null)
        {
            visitorManager
                .PauseVisitorSpawning();
        }

        // PLAYER SPEAKS FIRST.
        if (playerDialogue != null)
        {
            playerDialogue
                .SayPowerOut();
        }

        // Then visitor can react afterward.
        if (visitorManager != null)
        {
            StartCoroutine(
                DelayedVisitorBlackoutReaction()
            );
        }

        Debug.Log(
            "Security controls disabled during blackout."
        );
    }

    private IEnumerator
        DelayedVisitorBlackoutReaction()
    {
        yield return new WaitForSeconds(
            visitorReactionDelay
        );

        if (!powerOn)
        {
            visitorManager
                .HandleBlackoutVisitor();
        }
    }

    // ==================================================
    // RESTORE POWER
    // ==================================================

    public void RestorePower()
    {
        if (powerOn)
            return;

        powerOn = true;

        Debug.Log(
            "RESTORING POWER"
        );

        SetLights(
            restoringLights,
            true
        );

        // Permanently dead.
        SetLights(
            failedLights,
            false
        );

        SetPoweredDisplays(
            true
        );

        if (securityCameraSystem != null)
        {
            securityCameraSystem
                .RestoreAfterBlackout();
        }

        if (computer != null)
            computer.SetPowered(true);

        if (phone != null)
            phone.SetPowered(true);

        if (flickeringLights != null)
        {
            foreach (
                LightFlicker flicker
                in flickeringLights)
            {
                if (flicker != null)
                {
                    flicker
                        .SetFlickerEnabled(
                            true
                        );
                }
            }
        }

        if (shiftClock != null)
            shiftClock.ResumeClock(
                "Power"
            );

        if (gameFlowManager != null)
            gameFlowManager.PowerRestored();

        if (playerDialogue != null)
        {
            playerDialogue
                .SayPartialPower();
        }

        /*
         * Visitors still resume from
         * FlashlightPickup after the flashlight
         * is returned.
         */

        Debug.Log(
            "POWER RESTORED - PARTIAL SYSTEM FAILURE"
        );
    }

    // ==================================================
    // HELPERS
    // ==================================================

    private void SetLights(
        Light[] lights,
        bool state)
    {
        if (lights == null)
            return;

        foreach (
            Light lightSource
            in lights)
        {
            if (lightSource != null)
            {
                lightSource.enabled =
                    state;
            }
        }
    }

    private void SetPoweredDisplays(
        bool state)
    {
        if (poweredDisplays == null)
            return;

        foreach (
            GameObject display
            in poweredDisplays)
        {
            if (display != null)
            {
                display.SetActive(
                    state
                );
            }
        }
    }
}