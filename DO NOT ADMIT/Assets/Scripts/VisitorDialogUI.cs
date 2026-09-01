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

    [Header("Typing Audio")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float typingVolume = 0.25f;

    [Header("Timing")]
    [SerializeField] private float displayTime = 2f;

    private Coroutine dialogueRoutine;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (typingAudioSource != null)
        {
            typingAudioSource.playOnAwake = false;
            typingAudioSource.loop = false;
        }
    }

    public void ShowDialogue(string visitorName, string dialogue)
    {
        if (string.IsNullOrWhiteSpace(dialogue))
            return;

        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        StopTypingSound();

        dialogueRoutine = StartCoroutine(
            ShowDialogueRoutine(visitorName, dialogue)
        );
    }

    private IEnumerator ShowDialogueRoutine(
        string visitorName,
        string dialogue)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        string fullDialogue =
            visitorName.ToUpper() + ": " + dialogue;

        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char letter in fullDialogue)
        {
            if (dialogueText != null)
                dialogueText.text += letter;

            if (!char.IsWhiteSpace(letter))
            {
                PlayTypingSound();
            }

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        StopTypingSound();

        yield return new WaitForSeconds(
            displayTime
        );

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        dialogueRoutine = null;
    }

    private void PlayTypingSound()
    {
        if (typingAudioSource == null)
            return;

        if (typingSound == null)
            return;

        typingAudioSource.PlayOneShot(
            typingSound,
            typingVolume
        );
    }

    private void StopTypingSound()
    {
        if (typingAudioSource == null)
            return;

        typingAudioSource.Stop();
    }

    public void HideDialogue()
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        StopTypingSound();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}