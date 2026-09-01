using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Blackout Visuals")]
    [SerializeField]
    private Volume blackoutVolume;

    [SerializeField]
    private float blackoutFadeSpeed = 1.5f;

    [Header("Power Audio")]
    [SerializeField]
    private AudioSource powerAudioSource;

    [SerializeField]
    private AudioClip powerFailureSound;

    [SerializeField]
    private AudioClip powerRestoreSound;

    private bool powerOn = true;

    private Coroutine blackoutVolumeCoroutine;

    public bool PowerOn => powerOn;

    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        if (blackoutVolume != null)
        {
            blackoutVolume.weight = 0f;
        }

        if (powerAudioSource != null)
        {
            powerAudioSource.playOnAwake = false;
            powerAudioSource.loop = false;
        }
    }

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

        // Play blackout sound.
        PlayPowerSound(
            powerFailureSound
        );

        if (shiftClock != null)
        {
            shiftClock.PauseClock(
                "Power"
            );
        }

        if (gameFlowManager != null)
        {
            gameFlowManager.BlackoutStarted();
        }

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
        {
            computer.SetPowered(false);
        }

        if (phone != null)
        {
            phone.SetPowered(false);
        }

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

        // Darken the entire scene.
        FadeBlackoutVolume(
            1f
        );

        // Player speaks first.
        if (playerDialogue != null)
        {
            playerDialogue
                .SayPowerOut();
        }

        // Visitor reacts afterward.
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

    private IEnumerator DelayedVisitorBlackoutReaction()
    {
        yield return new WaitForSeconds(
            visitorReactionDelay
        );

        if (!powerOn &&
            visitorManager != null)
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

        // Play power restoration sound.
        PlayPowerSound(
            powerRestoreSound
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
        {
            computer.SetPowered(true);
        }

        if (phone != null)
        {
            phone.SetPowered(true);
        }

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
        {
            shiftClock.ResumeClock(
                "Power"
            );
        }

        if (gameFlowManager != null)
        {
            gameFlowManager.PowerRestored();
        }

        if (playerDialogue != null)
        {
            playerDialogue
                .SayPartialPower();
        }

        // Return scene exposure to normal.
        FadeBlackoutVolume(
            0f
        );

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
    // POWER AUDIO
    // ==================================================

    private void PlayPowerSound(
        AudioClip sound)
    {
        if (powerAudioSource == null)
            return;

        if (sound == null)
            return;

        powerAudioSource.PlayOneShot(
            sound
        );
    }

    // ==================================================
    // BLACKOUT VOLUME
    // ==================================================

    private void FadeBlackoutVolume(
        float targetWeight)
    {
        if (blackoutVolume == null)
            return;

        if (blackoutVolumeCoroutine != null)
        {
            StopCoroutine(
                blackoutVolumeCoroutine
            );
        }

        blackoutVolumeCoroutine =
            StartCoroutine(
                FadeBlackoutVolumeRoutine(
                    targetWeight
                )
            );
    }

    private IEnumerator FadeBlackoutVolumeRoutine(
        float targetWeight)
    {
        while (
            Mathf.Abs(
                blackoutVolume.weight -
                targetWeight
            ) > 0.01f)
        {
            blackoutVolume.weight =
                Mathf.MoveTowards(
                    blackoutVolume.weight,
                    targetWeight,
                    blackoutFadeSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        blackoutVolume.weight =
            targetWeight;

        blackoutVolumeCoroutine = null;
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