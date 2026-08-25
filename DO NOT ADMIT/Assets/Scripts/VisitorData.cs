using UnityEngine;

[CreateAssetMenu(fileName = "New Visitor", menuName = "Do Not Admit/Visitor Data")]
public class VisitorData : ScriptableObject
{
    [Header("Identity")]
    public string visitorName;
    public string employeeID;
    public string department;
    public string clearanceLevel;

    [Header("Status")]
    public string employeeStatus = "ACTIVE";
}