using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShiftClock : MonoBehaviour
{
    [Header("Clock")]
    [SerializeField] private TMP_Text clockText;

    [Header("Timing")]
    [SerializeField] private float secondsPerHour = 60f;

    private int currentHour = 22;
    private float timer;

    private bool clockStarted;
    private bool shiftFinished;

    private readonly HashSet<string> pauseReasons =
        new HashSet<string>();

    public event Action<int> OnHourChanged;

    public int CurrentHour => currentHour;

    public bool IsPaused =>
        pauseReasons.Count > 0;

    private void Start()
    {
        UpdateClockDisplay();
    }

    public void StartClock()
    {
        currentHour = 22;
        timer = 0f;

        pauseReasons.Clear();

        shiftFinished = false;
        clockStarted = true;

        UpdateClockDisplay();

        Debug.Log("Shift clock started.");
    }

    private void Update()
    {
        if (!clockStarted)
            return;

        if (shiftFinished)
            return;

        if (IsPaused)
            return;

        timer += Time.deltaTime;

        if (timer >= secondsPerHour)
        {
            timer -= secondsPerHour;

            AdvanceHour();
        }
    }

    private void AdvanceHour()
    {
        currentHour++;

        if (currentHour >= 24)
            currentHour = 0;

        UpdateClockDisplay();

        OnHourChanged?.Invoke(currentHour);

        Debug.Log(
            "Current shift hour: " +
            clockText.text
        );

        if (currentHour == 6)
        {
            shiftFinished = true;

            Debug.Log(
                "6:00 AM - SHIFT COMPLETE"
            );
        }
    }

    // ==================================================
    // PAUSING
    // ==================================================

    public void PauseClock(string reason)
    {
        if (!clockStarted || shiftFinished)
            return;

        if (string.IsNullOrEmpty(reason))
            reason = "Unknown";

        pauseReasons.Add(reason);

        Debug.Log(
            "Clock paused: " + reason
        );
    }

    public void ResumeClock(string reason)
    {
        if (!clockStarted || shiftFinished)
            return;

        if (string.IsNullOrEmpty(reason))
            reason = "Unknown";

        pauseReasons.Remove(reason);

        Debug.Log(
            "Clock pause removed: " + reason
        );

        if (!IsPaused)
            Debug.Log("Clock resumed.");
    }

    // Compatibility if anything old still calls these
    public void PauseClock()
    {
        PauseClock("Generic");
    }

    public void ResumeClock()
    {
        ResumeClock("Generic");
    }

    // ==================================================
    // DISPLAY
    // ==================================================

    private void UpdateClockDisplay()
    {
        int hour12 = currentHour % 12;

        if (hour12 == 0)
            hour12 = 12;

        string amPM =
            currentHour >= 12 ? "PM" : "AM";

        clockText.text =
            hour12.ToString("00") +
            ":00 " +
            amPM;
    }
}