using TMPro;
using UnityEngine;

public class EmployeeDatabase : MonoBehaviour
{
    [Header("Employee Records")]
    [SerializeField] private EmployeeRecord[] employeeRecords;

    [Header("UI")]
    [SerializeField] private TMP_InputField searchInput;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text departmentText;
    [SerializeField] private TMP_Text clearanceText;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private GameObject resultPanel;

    private void Start()
    {
        ClearResults();
    }

    public void SearchEmployee()
    {
        string searchValue = searchInput.text.Trim();

        EmployeeRecord foundRecord = null;

        foreach (EmployeeRecord record in employeeRecords)
        {
            bool idMatches =
                record.employeeID.Equals(
                    searchValue,
                    System.StringComparison.OrdinalIgnoreCase
                );

            bool nameMatches =
                record.employeeName.Equals(
                    searchValue,
                    System.StringComparison.OrdinalIgnoreCase
                );

            if (idMatches || nameMatches)
            {
                foundRecord = record;
                break;
            }
        }

        if (foundRecord != null)
        {
            DisplayRecord(foundRecord);
        }
        else
        {
            DisplayNotFound();
        }
    }

    private void DisplayRecord(EmployeeRecord record)
    {
        resultPanel.SetActive(true);

        nameText.text = "NAME: " + record.employeeName;
        idText.text = "ID: #" + record.employeeID;
        departmentText.text = "DEPARTMENT: " + record.department;
        clearanceText.text = "CLEARANCE: " + record.clearanceLevel;
        statusText.text = "STATUS: " + record.employeeStatus;
    }

    private void DisplayNotFound()
    {
        resultPanel.SetActive(true);

        nameText.text = "NO RECORD FOUND";
        idText.text = "";
        departmentText.text = "";
        clearanceText.text = "";
        statusText.text = "";
    }

    public void ClearResults()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (searchInput != null)
        {
            searchInput.text = "";
            searchInput.ActivateInputField();
        }

    }
}