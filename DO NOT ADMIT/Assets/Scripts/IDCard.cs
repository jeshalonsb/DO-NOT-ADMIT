using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDCard : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text departmentText;
    [SerializeField] private TMP_Text clearanceText;
    [SerializeField] private TMP_Text statusText;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;

    [Header("Pickup")]
    [SerializeField] private PickupID pickupID;

    private void Awake()
    {
        if (pickupID == null)
        {
            pickupID =
                GetComponent<PickupID>();
        }
    }

    public void DisplayVisitor(Visitor visitor)
    {
        if (visitor == null)
            return;

        // Turn card on first.
        gameObject.SetActive(true);

        // Make sure it is back on the desk.
        if (pickupID != null)
        {
            pickupID.ResetToDeskImmediate();
        }

        if (nameText != null)
        {
            nameText.text =
                visitor.DisplayName;
        }

        if (idText != null)
        {
            idText.text =
                "ID #" +
                visitor.DisplayEmployeeID;
        }

        if (departmentText != null)
        {
            departmentText.text =
                visitor.DisplayDepartment;
        }

        if (clearanceText != null)
        {
            clearanceText.text =
                "CLEARANCE: " +
                visitor.DisplayClearance;
        }

        if (statusText != null)
        {
            statusText.text =
                "STATUS: " +
                visitor.DisplayStatus;
        }

        if (portraitImage != null &&
            visitor.Data != null)
        {
            portraitImage.sprite =
                visitor.Data.idPortrait;

            portraitImage.enabled =
                visitor.Data.idPortrait != null;
        }

        Debug.Log(
            "Displaying ID for: " +
            visitor.DisplayName
        );
    }

    public void HideCard()
    {
        // Return card to desk first.
        if (pickupID != null)
        {
            pickupID.ResetToDeskImmediate();
        }

        // Then completely hide it.
        gameObject.SetActive(false);

        Debug.Log(
            "ID card hidden."
        );
    }
}