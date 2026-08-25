using UnityEngine;

public class DecisionButton : Interactable
{
    public enum DecisionType
    {
        Admit,
        Deny
    }

    [Header("Decision")]
    [SerializeField] private DecisionType decisionType;

    [Header("References")]
    [SerializeField] private VisitorManager visitorManager;

    public override void Interact()
    {
        base.Interact();

        if (visitorManager == null)
        {
            Debug.LogWarning("VisitorManager is not assigned.");
            return;
        }

        switch (decisionType)
        {
            case DecisionType.Admit:
                visitorManager.AdmitCurrentVisitor();
                break;

            case DecisionType.Deny:
                visitorManager.DenyCurrentVisitor();
                break;
        }
    }
}