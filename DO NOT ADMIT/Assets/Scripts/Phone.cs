using System.Collections;
using UnityEngine;

public class Phone : Interactable
{
    [Header("Phone Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ringSound;

    [Header("Dialogue")]
    [SerializeField] private VisitorDialogueUI dialogueUI;

    [Header("Call Settings")]
    [SerializeField] private float timeBetweenLines = 3.5f;

    [Header("Visitor Flow")]
    [SerializeField] private VisitorManager visitorManager;

    private bool phoneRinging;
    private bool callAnswered;
    private bool powered = true;

    public override void ShowPrompt()
    {
        if (!phoneRinging || !powered)
        {
            HidePrompt();
            return;
        }

        base.ShowPrompt();
    }
    public void StartCall()
    {
        if (!powered)
            return;

        if (phoneRinging || callAnswered)
            return;

        phoneRinging = true;

        if (visitorManager != null)
            visitorManager.PauseVisitorSpawning();

        if (audioSource != null && ringSound != null)
        {
            audioSource.clip = ringSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        Debug.Log("PHONE IS RINGING");
    }

    public override void Interact()
    {
        if (!powered)
        {
            Debug.Log("Phone has no power.");
            return;
        }

        if (!phoneRinging)
            return;

        AnswerPhone();
    }

    private void AnswerPhone()
    {
        phoneRinging = false;
        callAnswered = true;

        HidePrompt();

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }

        Debug.Log("PHONE ANSWERED");

        StartCoroutine(CallSequence());
    }

    private IEnumerator CallSequence()
    {
        dialogueUI.ShowDialogue(
            "UNKNOWN",
            "Still there?"
        );

        yield return new WaitForSeconds(timeBetweenLines);

        dialogueUI.ShowDialogue(
            "YOU",
            "Who is this?"
        );

        yield return new WaitForSeconds(timeBetweenLines);

        dialogueUI.ShowDialogue(
            "UNKNOWN",
            "Doesn't matter."
        );

        yield return new WaitForSeconds(timeBetweenLines);

        dialogueUI.ShowDialogue(
            "UNKNOWN",
            "Just don't trust the uniform."
        );

        yield return new WaitForSeconds(timeBetweenLines);

        dialogueUI.ShowDialogue(
            "YOU",
            "What are you talking about?"
        );

        yield return new WaitForSeconds(timeBetweenLines);

        dialogueUI.ShowDialogue(
            "UNKNOWN",
            "..."
        );

        yield return new WaitForSeconds(2f);
        Debug.Log("PHONE CALL ENDED");

        if (visitorManager != null)
        {
            visitorManager.ResumeVisitorSpawning();
        }
    }
    public void SetPowered(bool state)
    {
        powered = state;

        if (!powered)
        {
            // Stop ringing if power dies.
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }

            phoneRinging = false;

            Debug.Log("Phone lost power.");
        }
        else
        {
            Debug.Log("Phone power restored.");
        }
    }
}