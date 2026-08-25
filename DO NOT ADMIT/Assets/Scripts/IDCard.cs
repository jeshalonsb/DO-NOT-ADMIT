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

    public void DisplayVisitor(VisitorData data)
    {
        if (data == null)
        {
            Debug.LogWarning("VisitorData is null.");
            return;
        }

        Debug.Log("Displaying ID for: " + data.visitorName);

        nameText.text = data.visitorName;
        idText.text = "ID #" + data.employeeID;
        departmentText.text = data.department;
        clearanceText.text = "CLEARANCE: " + data.clearanceLevel;
        statusText.text = "STATUS: " + data.employeeStatus;

        gameObject.SetActive(true);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }
}