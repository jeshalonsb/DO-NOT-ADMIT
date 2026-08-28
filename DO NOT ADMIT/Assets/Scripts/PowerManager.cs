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

        Debug.Log("POWER FAILURE");

        // ----------------------------------------------
        // PAUSE CLOCK
        // ----------------------------------------------

        if (shiftClock != null)
            shiftClock.PauseClock("Power");

        // ----------------------------------------------
        // OBJECTIVE
        // ----------------------------------------------

        if (gameFlowManager != null)
            gameFlowManager.BlackoutStarted();

        // ----------------------------------------------
        // LIGHTS
        // ----------------------------------------------

        SetLights(
            restoringLights,
            false
        );

        SetLights(
            failedLights,
            false
        );

        // ----------------------------------------------
        // DISPLAYS
        // ----------------------------------------------

        SetPoweredDisplays(false);

        // ----------------------------------------------
        // CCTV
        // ----------------------------------------------

        if (securityCameraSystem != null)
        {
            securityCameraSystem
                .DisableAllCameras();
        }

        // ----------------------------------------------
        // COMPUTER
        // ----------------------------------------------

        if (computer != null)
            computer.SetPowered(false);

        // ----------------------------------------------
        // PHONE
        // ----------------------------------------------

        if (phone != null)
            phone.SetPowered(false);

        // ----------------------------------------------
        // FLASHLIGHT
        // ----------------------------------------------

        if (flashlightPickup != null)
        {
            flashlightPickup
                .UnlockForBlackout();
        }

        // ----------------------------------------------
        // VISITORS
        // ----------------------------------------------

        if (visitorManager != null)
        {
            visitorManager
                .PauseVisitorSpawning();

            visitorManager
                .HandleBlackoutVisitor();
        }

        Debug.Log(
            "Security controls disabled during blackout."
        );
    }

    // ==================================================
    // RESTORE POWER
    // ==================================================

    public void RestorePower()
    {
        if (powerOn)
            return;

        powerOn = true;

        Debug.Log("RESTORING POWER");

        // ----------------------------------------------
        // LIGHTS
        // ----------------------------------------------

        SetLights(
            restoringLights,
            true
        );

        // These permanently stay dead
        SetLights(
            failedLights,
            false
        );

        // ----------------------------------------------
        // DISPLAYS
        // ----------------------------------------------

        SetPoweredDisplays(true);

        // ----------------------------------------------
        // CCTV
        // ----------------------------------------------

        if (securityCameraSystem != null)
        {
            securityCameraSystem
                .RestoreAfterBlackout();
        }

        // ----------------------------------------------
        // COMPUTER
        // ----------------------------------------------

        if (computer != null)
            computer.SetPowered(true);

        // ----------------------------------------------
        // PHONE
        // ----------------------------------------------

        if (phone != null)
            phone.SetPowered(true);

        // ----------------------------------------------
        // POST-BLACKOUT FLICKERING
        // ----------------------------------------------

        if (flickeringLights != null)
        {
            foreach (
                LightFlicker flicker
                in flickeringLights)
            {
                if (flicker != null)
                {
                    flicker
                        .SetFlickerEnabled(true);
                }
            }
        }

        // ----------------------------------------------
        // RESUME CLOCK
        // ----------------------------------------------

        if (shiftClock != null)
            shiftClock.ResumeClock("Power");

        // ----------------------------------------------
        // OBJECTIVE
        // ----------------------------------------------

        if (gameFlowManager != null)
            gameFlowManager.PowerRestored();

        /*
         * DO NOT resume visitors here.
         *
         * Your FlashlightPickup already resumes visitors
         * after the player returns the flashlight.
         *
         * This keeps the blackout sequence controlled.
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