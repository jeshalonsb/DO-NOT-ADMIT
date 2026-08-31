using UnityEngine;

public enum CorrectDecision
{
    Clear,
    Deny
}

[CreateAssetMenu(
    fileName = "New Visitor",
    menuName = "Do Not Admit/Visitor Data"
)]
public class VisitorData : ScriptableObject
{
    [Header("Identity")]
    public string visitorName;
    public string employeeID;
    public string department;
    public string clearanceLevel;

    [Header("Status")]
    public string employeeStatus = "ACTIVE";

    [Header("Visitor Prefab")]
    public GameObject visitorPrefab;

    [Header("ID Portrait")]
    public Sprite idPortrait;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string arrivalDialogue;

    [TextArea(2, 4)]
    public string clearDialogue;

    [TextArea(2, 4)]
    public string denyDialogue;
}