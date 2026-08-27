using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string interactionMessage = "Interact";

    [Header("Floating Prompt")]
    [SerializeField] private GameObject interactionPrompt;

    private void Start()
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
        Debug.Log("Interacted with: " + gameObject.name);
    }
}