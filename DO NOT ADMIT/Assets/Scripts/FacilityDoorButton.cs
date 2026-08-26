using UnityEngine;

public class FacilityDoorButton : Interactable
{
    [SerializeField] private VisitorManager visitorManager;

    public override void Interact()
    {
        base.Interact();

        if (visitorManager != null)
            visitorManager.UnlockFacilityDoor();
    }
}