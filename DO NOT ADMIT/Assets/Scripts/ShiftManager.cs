using UnityEngine;

public class ShiftManager : MonoBehaviour
{
    [Header("Decision Stats")]
    [SerializeField] private int correctDecisions;
    [SerializeField] private int mistakes;

    [Header("Final Shift Stats")]
    [SerializeField] private int impostorsLetIn;
    [SerializeField] private int employeesLetIn;
    [SerializeField] private int employeesDenied;

    public int CorrectDecisions =>
        correctDecisions;

    public int Mistakes =>
        mistakes;

    public int ImpostorsLetIn =>
        impostorsLetIn;

    public int EmployeesLetIn =>
        employeesLetIn;

    public int EmployeesDenied =>
        employeesDenied;

    // ==================================================
    // DECISION
    // ==================================================

    public void RegisterDecision(
        VisitorData visitor,
        CorrectDecision correctDecision,
        CorrectDecision playerDecision)
    {
        if (visitor == null)
        {
            Debug.LogWarning(
                "VisitorData is null."
            );

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
                playerDecision
            );
        }
    }

    // ==================================================
    // DENIED VISITOR
    // ==================================================

    public void RegisterDeniedVisitor(
        Visitor visitor)
    {
        if (visitor == null)
            return;

        // Only a REAL employee denied counts
        // toward the fired ending.
        if (!visitor.IsImpostor)
        {
            employeesDenied++;

            Debug.LogWarning(
                "VALID EMPLOYEE DENIED | " +
                visitor.Data.visitorName +
                " | Total: " +
                employeesDenied
            );
        }
        else
        {
            Debug.Log(
                "Impostor correctly denied."
            );
        }
    }

    // ==================================================
    // ADMITTED VISITOR
    // ==================================================

    public void RegisterAdmittedVisitor(
        Visitor visitor)
    {
        if (visitor == null)
            return;

        if (visitor.IsImpostor)
        {
            impostorsLetIn++;

            Debug.LogWarning(
                "IMPOSTOR LET IN | " +
                visitor.Data.visitorName +
                " | Total: " +
                impostorsLetIn
            );
        }
        else
        {
            employeesLetIn++;

            Debug.Log(
                "EMPLOYEE LET IN | " +
                visitor.Data.visitorName +
                " | Total: " +
                employeesLetIn
            );
        }
    }

    // ==================================================
    // ENDING CONDITIONS
    // ==================================================

    public bool HasLetImpostorIn()
    {
        return impostorsLetIn > 0;
    }

    public bool HasDeniedValidEmployee()
    {
        return employeesDenied > 0;
    }

    public bool HasFailedShift()
    {
        return
            impostorsLetIn > 0 ||
            employeesDenied > 0;
    }

    public bool HasPerfectShift()
    {
        return
            impostorsLetIn == 0 &&
            employeesDenied == 0;
    }
}