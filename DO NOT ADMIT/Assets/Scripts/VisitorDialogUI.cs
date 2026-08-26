using System.Collections;
using TMPro;
using UnityEngine;

public class VisitorDialogueUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Timing")]
    [SerializeField] private float displayTime = 2f;

    private Coroutine dialogueRoutine;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string visitorName, string dialogue)
    {
        if (string.IsNullOrWhiteSpace(dialogue))
            return;

        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine = StartCoroutine(
            ShowDialogueRoutine(visitorName, dialogue)
        );
    }

    private IEnumerator ShowDialogueRoutine(
        string visitorName,
        string dialogue)
    {
        dialoguePanel.SetActive(true);

        // Combine name and dialogue into one string.
        string fullDialogue =
            visitorName.ToUpper() + ": " + dialogue;

        dialogueText.text = "";

        // Typewriter effect.
        foreach (char letter in fullDialogue)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        // Give player time to read after typing finishes.
        yield return new WaitForSeconds(displayTime);

        dialoguePanel.SetActive(false);
        dialogueRoutine = null;
    }

    public void HideDialogue()
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        dialoguePanel.SetActive(false);
    }
}