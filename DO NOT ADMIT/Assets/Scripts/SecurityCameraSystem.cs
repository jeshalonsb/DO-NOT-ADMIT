using UnityEngine;

public class SecurityCameraSystem : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera facilityEntrance;
    [SerializeField] private Camera employeeParking;
    [SerializeField] private Camera perimeterGate;
    [SerializeField] private Camera facilityRear;

    [Header("Monitor Feeds")]
    [SerializeField] private GameObject facilityEntranceFeed;
    [SerializeField] private GameObject employeeParkingFeed;
    [SerializeField] private GameObject perimeterGateFeed;
    [SerializeField] private GameObject facilityRearFeed;

    [Header("No Signal")]
    [SerializeField] private GameObject facilityEntranceNoSignal;
    [SerializeField] private GameObject employeeParkingNoSignal;
    [SerializeField] private GameObject perimeterGateNoSignal;
    [SerializeField] private GameObject facilityRearNoSignal;

    private void Start()
    {
        // Game starts with every camera functioning.
        RestoreAllCameras();
    }

    public void DisableAllCameras()
    {
        SetCamera(facilityEntrance, false);
        SetCamera(employeeParking, false);
        SetCamera(perimeterGate, false);
        SetCamera(facilityRear, false);

        SetActive(facilityEntranceFeed, false);
        SetActive(employeeParkingFeed, false);
        SetActive(perimeterGateFeed, false);
        SetActive(facilityRearFeed, false);

        SetActive(facilityEntranceNoSignal, false);
        SetActive(employeeParkingNoSignal, false);
        SetActive(perimeterGateNoSignal, false);
        SetActive(facilityRearNoSignal, false);

        Debug.Log("CCTV power lost.");
    }

    public void RestoreAfterBlackout()
    {
        // CAM 01 survives.
        SetCamera(facilityEntrance, true);
        SetActive(facilityEntranceFeed, true);
        SetActive(facilityEntranceNoSignal, false);

        // CAM 02 fails.
        SetCamera(employeeParking, false);
        SetActive(employeeParkingFeed, false);
        SetActive(employeeParkingNoSignal, true);

        // CAM 03 fails.
        SetCamera(perimeterGate, false);
        SetActive(perimeterGateFeed, false);
        SetActive(perimeterGateNoSignal, true);

        // CAM 04 fails.
        SetCamera(facilityRear, false);
        SetActive(facilityRearFeed, false);
        SetActive(facilityRearNoSignal, true);

        Debug.Log("CCTV partially restored.");
    }

    public void RestoreAllCameras()
    {
        SetCamera(facilityEntrance, true);
        SetCamera(employeeParking, true);
        SetCamera(perimeterGate, true);
        SetCamera(facilityRear, true);

        SetActive(facilityEntranceFeed, true);
        SetActive(employeeParkingFeed, true);
        SetActive(perimeterGateFeed, true);
        SetActive(facilityRearFeed, true);

        SetActive(facilityEntranceNoSignal, false);
        SetActive(employeeParkingNoSignal, false);
        SetActive(perimeterGateNoSignal, false);
        SetActive(facilityRearNoSignal, false);

        Debug.Log("All CCTV cameras online.");
    }

    private void SetCamera(Camera cameraToSet, bool state)
    {
        if (cameraToSet != null)
            cameraToSet.enabled = state;
    }

    private void SetActive(GameObject obj, bool state)
    {
        if (obj != null)
            obj.SetActive(state);
    }
}