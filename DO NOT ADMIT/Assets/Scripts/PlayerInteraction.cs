using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    private Interactable currentInteractable;
    private BreakerBox currentBreaker;

    private void Update()
    {
        CheckForInteractable();

        if (Keyboard.current == null)
            return;

        // Normal interactables use a single E press
        if (Keyboard.current.eKey.wasPressedThisFrame &&
            currentInteractable != null &&
            currentBreaker == null)
        {
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionLayer))
        {
            Interactable newInteractable =
                hit.collider.GetComponent<Interactable>();

            BreakerBox newBreaker =
                hit.collider.GetComponent<BreakerBox>();

            if (newInteractable != null)
            {
                // Switched to a different interactable
                if (newInteractable != currentInteractable)
                {
                    if (currentInteractable != null)
                        currentInteractable.HidePrompt();

                    // Stop breaker hold if we looked away from old breaker
                    if (currentBreaker != null)
                    {
                        currentBreaker.SetPlayerLooking(false);
                        currentBreaker = null;
                    }

                    currentInteractable = newInteractable;
                    currentInteractable.ShowPrompt();

                    // Check if this interactable is the breaker
                    if (newBreaker != null)
                    {
                        currentBreaker = newBreaker;
                        currentBreaker.SetPlayerLooking(true);
                    }
                }

                return;
            }
        }

        ClearCurrentInteractable();
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.HidePrompt();
            currentInteractable = null;
        }

        if (currentBreaker != null)
        {
            currentBreaker.SetPlayerLooking(false);
            currentBreaker = null;
        }
    }
}