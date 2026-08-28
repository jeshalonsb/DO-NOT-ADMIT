using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField]
    private string interactionMessage =
        "Interact";

    [Header("Floating Prompt")]
    [SerializeField]
    private GameObject interactionPrompt;

    [Header("Blackout")]
    [Tooltip(
        "Enable this only for things the player should use during the blackout, such as the flashlight, breaker, and booth door."
    )]
    [SerializeField]
    private bool allowDuringBlackout;

    protected bool AllowDuringBlackout =>
        allowDuringBlackout;

    protected GameObject InteractionPrompt =>
        interactionPrompt;

    protected virtual void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public string GetInteractionMessage()
    {
        return interactionMessage;
    }

    public virtual void ShowPrompt()
    {
        /*
         * If blackout is happening, hide normal
         * interaction prompts.
         */
        if (GameFlowManager.Instance != null &&
            GameFlowManager.Instance
                .SuppressNormalInteractionPrompts &&
            !allowDuringBlackout)
        {
            HidePrompt();
            return;
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    public virtual void HidePrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public virtual void Interact()
    {
        Debug.Log(
            "Interacted with: " +
            gameObject.name
        );
    }
}