using TMPro;
using UnityEngine;
using System;

public class ShiftClock : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text clockText;

    [Header("Timing")]
    [Tooltip("Real seconds between each in-game hour.")]
    [SerializeField] private float secondsPerHour = 60f;

    private int currentHour = 22; // 10 PM
    private float timer;
    private bool clockRunning;

    public event Action<int> OnHourChanged;

    public int CurrentHour => currentHour;

    private void Start()
    {
        UpdateClockDisplay();
    }

    public void StartClock()
    {
        currentHour = 22;
        timer = 0f;
        clockRunning = true;

        UpdateClockDisplay();

        Debug.Log("Shift clock started.");
    }

    private void Update()
    {
        if (!clockRunning)
            return;

        timer += Time.deltaTime;

        if (timer >= secondsPerHour)
        {
            timer = 0f;

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

        Debug.Log("Current shift hour: " + clockText.text);

        // End shift at 6 AM
        if (currentHour == 6)
        {
            clockRunning = false;

            Debug.Log("6:00 AM - SHIFT COMPLETE");
        }
    }

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