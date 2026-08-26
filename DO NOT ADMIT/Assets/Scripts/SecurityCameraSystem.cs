using UnityEngine;

public class SecurityCameraSystem : MonoBehaviour
{
    [Header("Security Cameras")]
    [SerializeField] private Camera facilityEntranceCamera;
    [SerializeField] private Camera employeeParkingCamera;
    [SerializeField] private Camera perimeterGateCamera;
    [SerializeField] private Camera rearFacilityCamera;

    public void DisableAllCameras()
    {
        facilityEntranceCamera.enabled = false;
        employeeParkingCamera.enabled = false;
        perimeterGateCamera.enabled = false;
        rearFacilityCamera.enabled = false;

        Debug.Log("All security cameras offline.");
    }

    public void RestoreAfterBlackout()
    {
        // Gameplay-critical camera returns.
        facilityEntranceCamera.enabled = true;

        // Other cameras remain damaged.
        employeeParkingCamera.enabled = false;
        perimeterGateCamera.enabled = false;
        rearFacilityCamera.enabled = false;

        Debug.Log("CAM 01 restored. Remaining cameras offline.");
    }

    public void RestoreAllCameras()
    {
        facilityEntranceCamera.enabled = true;
        employeeParkingCamera.enabled = true;
        perimeterGateCamera.enabled = true;
        rearFacilityCamera.enabled = true;
    }
}