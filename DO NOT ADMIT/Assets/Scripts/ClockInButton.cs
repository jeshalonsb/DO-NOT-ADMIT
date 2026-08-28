using UnityEngine;

public class ClockInButton : Interactable
{
    [SerializeField] private GameFlowManager gameFlowManager;

    private bool clockedIn;

    public override void Interact()
    {
        if (clockedIn)
            return;

        if (gameFlowManager == null)
            return;

        if (!gameFlowManager.ManualRead)
        {
            Debug.Log("Read the first day manual before clocking in.");
            return;
        }

        gameFlowManager.ClockIn();

        clockedIn = true;

        Debug.Log("Clock-in button pressed.");
    }
}