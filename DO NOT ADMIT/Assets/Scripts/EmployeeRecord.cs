using UnityEngine;

[CreateAssetMenu(fileName = "New Employee Record", menuName = "Do Not Admit/Employee Record")]
public class EmployeeRecord : ScriptableObject
{ 
    [Header("Official Employee Information")]
    public string employeeName;
    public string employeeID;
    public string department;
    public string clearanceLevel;
    public string employeeStatus = "ACTIVE";
}
