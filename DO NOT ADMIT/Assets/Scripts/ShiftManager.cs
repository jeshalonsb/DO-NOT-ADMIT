using UnityEngine;

public class ShiftManager : MonoBehaviour
{
    [Header("Shift Stats")]
    [SerializeField] private int correctDecisions = 0;
    [SerializeField] private int mistakes = 0;

    [Header("Failure Settings")]
    [SerializeField] private int maximumMistakes = 2;

    public int CorrectDecisions => correctDecisions;
    public int Mistakes => mistakes;

    public void RegisterDecision(
        VisitorData visitor,
        CorrectDecision playerDecision)
    {
        if (visitor == null)
        {
            Debug.LogWarning("Cannot register decision: VisitorData is null.");
            return;
        }

        if (visitor.correctDecision == playerDecision)
        {
            correctDecisions++;

            Debug.Log(
                "CORRECT DECISION: " +
                visitor.visitorName +
                " should have been " +
                visitor.correctDecision
            );
        }
        else
        {
            mistakes++;

            Debug.LogWarning(
                "WRONG DECISION: " +
                visitor.visitorName +
                " should have been " +
                visitor.correctDecision +
                ". Mistakes: " +
                mistakes +
                "/" +
                maximumMistakes
            );
        }
    }

    public bool HasFailedShift()
    {
        return mistakes >= maximumMistakes;
    }
}