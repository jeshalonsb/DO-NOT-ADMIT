using UnityEngine;

public class DecisionButton : Interactable
{
    public enum DecisionType
    {
        Clear,
        Deny
    }

    [Header("Decision")]
    [SerializeField]
    private DecisionType decisionType;

    [Header("References")]
    [SerializeField]
    private VisitorManager visitorManager;

    [SerializeField]
    private DecisionButtonGate decisionGate;

    [Header("Audio")]
    [SerializeField]
    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonSound;

    public override void Interact()
    {
        if (visitorManager == null)
            return;

        // Visitor hasn't reached booth yet,
        // or a decision was already made.
        if (decisionGate == null ||
            !decisionGate.CanUseButton)
        {
            return;
        }

        // Immediately lock BOTH buttons.
        decisionGate.UseDecision();

        PlayButtonSound();

        switch (decisionType)
        {
            case DecisionType.Clear:

                visitorManager
                    .ClearCurrentVisitor();

                break;

            case DecisionType.Deny:

                visitorManager
                    .DenyCurrentVisitor();

                break;
        }
    }

    public override void ShowPrompt()
    {
        // Don't even show the interaction
        // prompt until a visitor is ready.
        if (decisionGate == null ||
            !decisionGate.CanUseButton)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }

    private void PlayButtonSound()
    {
        if (buttonAudioSource == null ||
            buttonSound == null)
        {
            return;
        }

        buttonAudioSource.PlayOneShot(
            buttonSound
        );
    }
}