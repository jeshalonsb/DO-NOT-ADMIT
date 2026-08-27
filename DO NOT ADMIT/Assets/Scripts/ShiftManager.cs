using UnityEngine;

public class ShiftManager : MonoBehaviour
{
    [Header("Shift Stats")]
    [SerializeField] private int correctDecisions;
    [SerializeField] private int mistakes;

    [Header("Failure Settings")]
    [SerializeField] private int maximumMistakes = 2;

    public int CorrectDecisions => correctDecisions;
    public int Mistakes => mistakes;

    public void RegisterDecision(
        VisitorData visitor,
        CorrectDecision correctDecision,
        CorrectDecision playerDecision)
    {
        if (visitor == null)
        {
            Debug.LogWarning("VisitorData is null.");
            return;
        }

        if (correctDecision == playerDecision)
        {
            correctDecisions++;

            Debug.Log(
                "CORRECT DECISION | " +
                visitor.visitorName +
                " | Correct: " +
                correctDecision
            );
        }
        else
        {
            mistakes++;

            Debug.LogWarning(
                "WRONG DECISION | " +
                visitor.visitorName +
                " | Correct: " +
                correctDecision +
                " | Player chose: " +
                playerDecision +
                " | Mistakes: " +
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