using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    private Interactable currentInteractable;

    private void Update()
    {
        CheckForInteractable();

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            Interactable newInteractable =
                hit.collider.GetComponent<Interactable>();

            if (newInteractable != null)
            {
                // We're looking at a different object
                if (newInteractable != currentInteractable)
                {
                    if (currentInteractable != null)
                        currentInteractable.HidePrompt();

                    currentInteractable = newInteractable;
                    currentInteractable.ShowPrompt();
                }

                return;
            }
        }

        // Nothing interactable is being looked at
        if (currentInteractable != null)
        {
            currentInteractable.HidePrompt();
            currentInteractable = null;
        }
    }
}