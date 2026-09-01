using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TMP_Text))]
public class EndingScreenTypewriter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject objectiveTextObject;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.035f;

    [Header("Timing")]
    [SerializeField] private float returnDelay = 3f;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private TMP_Text endingText;
    private Coroutine typeRoutine;

    private void Awake()
    {
        endingText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (endingText == null)
            endingText = GetComponent<TMP_Text>();

        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        typeRoutine = StartCoroutine(
            StartEnding()
        );
    }

    private IEnumerator StartEnding()
    {
        // Wait one frame so EndingController
        // finishes assigning the ending text.
        yield return null;

        string fullText = endingText.text;

        // Do NOTHING unless this is actually
        // one of the game's ending screens.
        bool isEnding =
            fullText.Contains("SHIFT COMPLETE") ||
            fullText.Contains("SHIFT FAILED");

        if (!isEnding)
        {
            typeRoutine = null;
            yield break;
        }

        // Now we're definitely ending the game.
        if (objectiveTextObject != null)
            objectiveTextObject.SetActive(false);

        endingText.text = "";

        foreach (char letter in fullText)
        {
            endingText.text += letter;

            yield return new WaitForSecondsRealtime(
                typingSpeed
            );
        }

        yield return new WaitForSecondsRealtime(
            returnDelay
        );

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }

    private void OnDisable()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }
    }
}