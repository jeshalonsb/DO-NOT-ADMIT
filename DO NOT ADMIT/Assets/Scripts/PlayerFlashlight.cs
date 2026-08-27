using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField] private GameObject flashlightViewmodel;
    [SerializeField] private Light flashlightLight;

    public bool HasFlashlight { get; private set; }
    public bool FlashlightOn { get; private set; }

    private void Start()
    {
        flashlightViewmodel.SetActive(false);
        flashlightLight.enabled = false;
    }

    private void Update()
    {
        if (!HasFlashlight)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    public void EquipFlashlight()
    {
        HasFlashlight = true;
        FlashlightOn = true;

        flashlightViewmodel.SetActive(true);
        flashlightLight.enabled = true;

        Debug.Log("Flashlight equipped.");
    }

    public void PutAwayFlashlight()
    {
        HasFlashlight = false;
        FlashlightOn = false;

        flashlightViewmodel.SetActive(false);
        flashlightLight.enabled = false;

        Debug.Log("Flashlight returned to desk.");
    }

    private void ToggleFlashlight()
    {
        FlashlightOn = !FlashlightOn;
        flashlightLight.enabled = FlashlightOn;
    }
}