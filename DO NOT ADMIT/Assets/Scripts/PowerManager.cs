using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [Header("Facility Lights")]
    [SerializeField] private Light[] facilityLights;

    [Header("Powered Displays")]
    [Tooltip("Clock screen, CCTV monitor screen objects, etc.")]
    [SerializeField] private GameObject[] poweredDisplays;

    [Header("Cameras")]
    [SerializeField] private SecurityCameraSystem securityCameraSystem;

    [Header("Computer")]
    [SerializeField] private ComputerInteractable computer;

    [Header("Phone")]
    [SerializeField] private Phone phone;

    [Header("Flashlight")]
    [SerializeField] private FlashlightPickup flashlightPickup;

    [Header("Visitor Flow")]
    [SerializeField] private VisitorManager visitorManager;

    private bool powerOn = true;

    public bool PowerOn => powerOn;

    public void CutPower()
    {
        if (!powerOn)
            return;

        powerOn = false;

        // Kill every light
        SetLights(false);

        // Turn physical screens off
        SetPoweredDisplays(false);

        // Shut down CCTV
        if (securityCameraSystem != null)
            securityCameraSystem.DisableAllCameras();

        // Shut down computer safely
        if (computer != null)
            computer.SetPowered(false);

        if (phone != null)
            phone.SetPowered(false);

        if (flashlightPickup != null)
        {
            flashlightPickup.UnlockForBlackout();
        }

        if (visitorManager != null)
        {
            visitorManager.PauseVisitorSpawning();
        }

        Debug.Log("POWER FAILURE");
    }

    public void RestorePower()
    {
        if (powerOn)
            return;

        powerOn = true;

        // Restore lights
        SetLights(true);

        // Restore displays
        SetPoweredDisplays(true);

        // Main camera returns, others stay damaged
        if (securityCameraSystem != null)
            securityCameraSystem.RestoreAfterBlackout();

        // Computer returns
        if (computer != null)
            computer.SetPowered(true);

        if (phone != null)
            phone.SetPowered(true);

        Debug.Log("POWER RESTORED");
    }

    private void SetLights(bool state)
    {
        foreach (Light lightSource in facilityLights)
        {
            if (lightSource != null)
                lightSource.enabled = state;
        }
    }

    private void SetPoweredDisplays(bool state)
    {
        foreach (GameObject display in poweredDisplays)
        {
            if (display != null)
                display.SetActive(state);
        }
    }
}