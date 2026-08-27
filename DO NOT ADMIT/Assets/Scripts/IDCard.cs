using TMPro;
using UnityEngine;

public class IDCard : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text departmentText;
    [SerializeField] private TMP_Text clearanceText;
    [SerializeField] private TMP_Text statusText;

    public void DisplayVisitor(Visitor visitor)
    {
        if (visitor == null)
            return;

        nameText.text =
            visitor.DisplayName;

        idText.text =
            "ID #" + visitor.DisplayEmployeeID;

        departmentText.text =
            visitor.DisplayDepartment;

        clearanceText.text =
            "CLEARANCE: " +
            visitor.DisplayClearance;

        statusText.text =
            "STATUS: " +
            visitor.DisplayStatus;

        gameObject.SetActive(true);

        Debug.Log(
            "Displaying ID for: " +
            visitor.DisplayName
        );
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }
}