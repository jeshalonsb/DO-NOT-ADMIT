using UnityEngine;

public class DecisionButtonGate : MonoBehaviour
{
    [Header("Visitor ID")]
    [SerializeField] private GameObject idCard;

    private bool decisionUsed;

    public bool CanUseButton
    {
        get
        {
            if (idCard == null)
                return false;

            if (!idCard.activeInHierarchy)
                return false;

            if (decisionUsed)
                return false;

            return true;
        }
    }

    private void Update()
    {
        // When the ID disappears,
        // prepare for the next visitor.
        if (idCard == null ||
            !idCard.activeInHierarchy)
        {
            decisionUsed = false;
        }
    }

    public void UseDecision()
    {
        decisionUsed = true;
    }
}