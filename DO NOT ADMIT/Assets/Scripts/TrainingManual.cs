using UnityEngine;

public class TrainingManual : Interactable
{
    [SerializeField] private GameFlowManager gameFlowManager;

    private bool hasBeenRead;

    public override void Interact()
    {
        if (hasBeenRead)
            return;

        if (gameFlowManager == null)
            return;

        gameFlowManager.ReadManual();

        hasBeenRead = true;

        Debug.Log("Player read the training manual.");
    }
}