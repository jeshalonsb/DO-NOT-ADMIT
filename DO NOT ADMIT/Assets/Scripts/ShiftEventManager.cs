using UnityEngine;

public class ShiftEventManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShiftClock shiftClock;
    [SerializeField] private Phone phone;

    [SerializeField] private PowerManager powerManager;

    private void OnEnable()
    {
        if (shiftClock != null)
            shiftClock.OnHourChanged += HandleHourChanged;
    }

    private void OnDisable()
    {
        if (shiftClock != null)
            shiftClock.OnHourChanged -= HandleHourChanged;
    }

    private void HandleHourChanged(int hour)
    {
        Debug.Log("Checking shift events for hour: " + hour);

        switch (hour)
        {
            case 0: // 12 AM
                TriggerMidnightCall();
                break;

            case 3: // 3 AM
                TriggerBlackout();
                break;
        }
    }

    private void TriggerMidnightCall()
    {
        if (phone != null)
        {
            phone.StartCall();

            Debug.Log("Midnight phone event triggered.");
        }
    }

    private void TriggerBlackout()
    {
        if (powerManager != null)
        {
            powerManager.CutPower();

            Debug.Log("3 AM blackout triggered.");
        }
    }
}