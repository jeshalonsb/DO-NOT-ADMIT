using UnityEngine;

public class ShiftEventManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShiftClock shiftClock;
    [SerializeField] private Phone phone;

    [SerializeField] private PowerManager powerManager;
    [SerializeField] private VisitorManager visitorManager;

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

            case 5:
                BeginEndOfShift();
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
    private void BeginEndOfShift()
    {
        if (visitorManager != null)
        {
            visitorManager.BeginShiftEnding();
        }

        Debug.Log("5 AM - No more visitors will arrive.");
    }
}