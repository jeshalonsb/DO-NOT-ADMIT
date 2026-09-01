using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    private Interactable currentInteractable;

    private BreakerBox currentBreaker;
    private TrainingManual currentManual;

    private PickupID heldID;

    private void Update()
    {
        CheckForInteractable();

        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        // --------------------------------------
        // BREAKER
        // --------------------------------------

        if (currentBreaker != null)
            return;

        // --------------------------------------
        // HOLDING ID
        // --------------------------------------

        if (heldID != null)
        {
            /*
             * If we're looking at another interactable
             * while holding the ID, interact with it
             * instead of putting the ID down.
             */
            if (currentInteractable != null &&
                currentInteractable != heldID)
            {
                currentInteractable.Interact();
                return;
            }

            /*
             * Otherwise E puts the ID back down.
             */
            heldID.PutDown();
            heldID = null;

            return;
        }

        // --------------------------------------
        // NORMAL INTERACTION
        // --------------------------------------

        if (currentInteractable != null)
        {
            PickupID pickupID =
                currentInteractable
                    .GetComponent<PickupID>();

            currentInteractable.Interact();

            if (pickupID != null &&
                pickupID.IsHeld)
            {
                heldID = pickupID;
            }
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(
            transform.position,
            transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionLayer))
        {
            Interactable newInteractable =
                hit.collider
                    .GetComponentInParent<Interactable>();

            BreakerBox newBreaker =
                hit.collider
                    .GetComponentInParent<BreakerBox>();

            TrainingManual newManual =
                hit.collider
                    .GetComponentInParent<TrainingManual>();

            if (newInteractable != null)
            {
                // --------------------------------------
                // CHANGED INTERACTABLE
                // --------------------------------------

                if (newInteractable !=
                    currentInteractable)
                {
                    ClearCurrentInteractable();

                    currentInteractable =
                        newInteractable;

                    if (newManual == null)
                    {
                        currentInteractable
                            .ShowPrompt();
                    }
                }

                // --------------------------------------
                // BREAKER
                // --------------------------------------

                if (newBreaker != null)
                {
                    if (currentBreaker !=
                        newBreaker)
                    {
                        if (currentBreaker != null)
                        {
                            currentBreaker
                                .SetPlayerLooking(false);
                        }

                        currentBreaker =
                            newBreaker;

                        currentBreaker
                            .SetPlayerLooking(true);
                    }
                }
                else if (currentBreaker != null)
                {
                    currentBreaker
                        .SetPlayerLooking(false);

                    currentBreaker = null;
                }

                // --------------------------------------
                // MANUAL
                // --------------------------------------

                if (newManual != null)
                {
                    if (currentManual !=
                        newManual)
                    {
                        if (currentManual != null)
                        {
                            currentManual
                                .SetPlayerLooking(false);
                        }

                        currentManual =
                            newManual;
                    }

                    currentManual
                        .SetPlayerLooking(true);
                }
                else if (currentManual != null)
                {
                    currentManual
                        .SetPlayerLooking(false);

                    currentManual = null;
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
            currentBreaker
                .SetPlayerLooking(false);

            currentBreaker = null;
        }

        if (currentManual != null)
        {
            currentManual
                .SetPlayerLooking(false);

            currentManual = null;
        }
    }
}