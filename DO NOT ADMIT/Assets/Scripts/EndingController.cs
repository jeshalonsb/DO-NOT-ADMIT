using System.Collections;
using TMPro;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    private enum EndingType
    {
        None,
        Good,
        Bad
    }

    [Header("Main References")]
    [SerializeField] private ShiftClock shiftClock;
    [SerializeField] private ShiftManager shiftManager;
    [SerializeField] private VisitorManager visitorManager;

    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerFlashlight playerFlashlight;

    // ==================================================
    // GOOD ENDING
    // ==================================================

    [Header("GOOD ENDING - Supervisor")]
    [SerializeField] private GameObject supervisor;
    [SerializeField] private Transform supervisorLookTarget;

    [SerializeField] private GameObject[] supervisorScareTriggers;

    [Range(0f, 1f)]
    [SerializeField] private float supervisorScareChance = 0.8f;

    [SerializeField] private float supervisorSpawnDistance = 2f;
    [SerializeField] private float supervisorYOffset = 0f;

    [Header("GOOD ENDING - Camera")]
    [SerializeField] private float goodCameraTurnDuration = 0.35f;
    [SerializeField] private float goodScarePause = 0.5f;

    [Header("GOOD ENDING - Dialogue")]
    [SerializeField] private VisitorDialogueUI dialogueUI;
    [SerializeField] private float dialogueLineTime = 3.5f;

    // ==================================================
    // BAD ENDING
    // ==================================================

    [Header("BAD ENDING - Trigger")]
    [SerializeField] private GameObject badEndingTrigger;

    [Header("BAD ENDING - Impostor")]
    [SerializeField] private GameObject impostor;
    [SerializeField] private Transform impostorLookTarget;
    [SerializeField] private Transform impostorSpawnPoint;

    [Header("BAD ENDING - Jumpscare")]
    [SerializeField] private float badCameraTurnDuration = 0.18f;
    [SerializeField] private float lungeDuration = 0.3f;
    [SerializeField] private float lungeStopDistance = 0.45f;

    [Header("BAD ENDING - Fall")]
    [SerializeField] private float fallDuration = 0.55f;
    [SerializeField] private float fallDistance = 1.1f;
    [SerializeField] private float fallRoll = 75f;

    [Header("BAD ENDING - Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;

    // ==================================================
    // ENDING UI
    // ==================================================

    [Header("Ending UI")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Game Flow")]
    [SerializeField]
    private GameFlowManager gameFlowManager;

    private EndingType endingType =
        EndingType.None;

    private bool shiftEnded;
    private bool supervisorScareUsed;
    private bool badJumpscareUsed;
    private bool gameEnded;

    // ==================================================
    // UNITY EVENTS
    // ==================================================

    private void OnEnable()
    {
        if (shiftClock != null)
        {
            shiftClock.OnHourChanged +=
                HandleHourChanged;
        }
    }

    private void OnDisable()
    {
        if (shiftClock != null)
        {
            shiftClock.OnHourChanged -=
                HandleHourChanged;
        }
    }

    private void Start()
    {
        if (supervisor != null)
            supervisor.SetActive(false);

        if (impostor != null)
            impostor.SetActive(false);

        SetAllGoodScareTriggers(false);

        if (badEndingTrigger != null)
            badEndingTrigger.SetActive(false);

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        if (endingText != null)
            endingText.gameObject.SetActive(false);
    }

    // ==================================================
    // 6 AM
    // ==================================================

    private void HandleHourChanged(
        int hour)
    {
        if (hour == 6)
            StartEndOfShift();
    }

    private void StartEndOfShift()
    {
        if (shiftEnded)
            return;

        shiftEnded = true;

        if (gameFlowManager != null)
            gameFlowManager.CompleteShift();

        // Completely close visitor processing
        if (visitorManager != null)
            visitorManager.CloseShift();

        if (objectiveText != null)
        {
            objectiveText.text =
                "SHIFT COMPLETE - RETURN TO YOUR CAR";
        }

        if (shiftManager == null)
        {
            Debug.LogWarning(
                "ShiftManager missing from EndingController."
            );

            return;
        }

        if (shiftManager.HasFailedShift())
        {
            PrepareBadEnding();
        }
        else
        {
            PrepareGoodEnding();
        }
    }

    // ==================================================
    // GOOD ENDING
    // ==================================================

    private void PrepareGoodEnding()
    {
        endingType =
            EndingType.Good;

        Debug.Log(
            "GOOD ENDING - RETURN TO CAR"
        );

        if (badEndingTrigger != null)
            badEndingTrigger.SetActive(false);

        bool shouldScare =
            Random.value <=
            supervisorScareChance;

        if (!shouldScare)
        {
            Debug.Log(
                "Supervisor will NOT appear this playthrough."
            );

            return;
        }

        if (
            supervisorScareTriggers == null ||
            supervisorScareTriggers.Length == 0)
        {
            Debug.LogWarning(
                "No supervisor scare triggers assigned."
            );

            return;
        }

        int chosenIndex =
            Random.Range(
                0,
                supervisorScareTriggers.Length
            );

        for (
            int i = 0;
            i < supervisorScareTriggers.Length;
            i++)
        {
            if (
                supervisorScareTriggers[i] != null)
            {
                supervisorScareTriggers[i]
                    .SetActive(
                        i == chosenIndex
                    );
            }
        }

        Debug.Log(
            "Supervisor scare point selected: " +
            chosenIndex
        );
    }

    public void TriggerSupervisorScare()
    {
        if (
            endingType !=
            EndingType.Good)
            return;

        if (supervisorScareUsed)
            return;

        if (gameEnded)
            return;

        supervisorScareUsed = true;

        SetAllGoodScareTriggers(false);

        StartCoroutine(
            SupervisorScareRoutine()
        );
    }

    private IEnumerator
        SupervisorScareRoutine()
    {
        LockPlayerControls();

        SpawnSupervisorBehindPlayer();

        yield return
            new WaitForSeconds(0.1f);

        if (supervisorLookTarget != null)
        {
            yield return StartCoroutine(
                TurnCameraToward(
                    supervisorLookTarget,
                    goodCameraTurnDuration
                )
            );
        }

        yield return
            new WaitForSeconds(
                goodScarePause
            );

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                "SUPERVISOR",
                "Hey."
            );
        }

        yield return
            new WaitForSeconds(
                dialogueLineTime
            );

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                "SUPERVISOR",
                "Good work tonight."
            );
        }

        yield return
            new WaitForSeconds(
                dialogueLineTime
            );

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                "SUPERVISOR",
                "Get home safe."
            );
        }

        yield return
            new WaitForSeconds(
                dialogueLineTime
            );

        if (!gameEnded)
            UnlockPlayerControls();
    }

    private void SpawnSupervisorBehindPlayer()
    {
        if (
            supervisor == null ||
            player == null ||
            playerCamera == null)
            return;

        Vector3 forward =
            Vector3.ProjectOnPlane(
                playerCamera.forward,
                Vector3.up
            ).normalized;

        if (
            forward.sqrMagnitude <
            0.01f)
        {
            forward =
                player.forward;
        }

        Vector3 spawnPosition =
            player.position -
            forward *
            supervisorSpawnDistance;

        spawnPosition.y +=
            supervisorYOffset;

        supervisor.transform.position =
            spawnPosition;

        Vector3 direction =
            player.position -
            supervisor.transform.position;

        direction.y = 0f;

        if (
            direction.sqrMagnitude >
            0.01f)
        {
            supervisor.transform.rotation =
                Quaternion.LookRotation(
                    direction
                );
        }

        supervisor.SetActive(true);
    }

    // ==================================================
    // BAD ENDING
    // ==================================================

    private void PrepareBadEnding()
    {
        endingType =
            EndingType.Bad;

        Debug.Log(
            "BAD ENDING ARMED - RETURN TO CAR"
        );

        SetAllGoodScareTriggers(false);

        if (badEndingTrigger != null)
            badEndingTrigger.SetActive(true);
    }

    public void TriggerBadJumpscare()
    {
        if (
            endingType !=
            EndingType.Bad)
            return;

        if (badJumpscareUsed)
            return;

        if (gameEnded)
            return;

        badJumpscareUsed = true;

        if (badEndingTrigger != null)
            badEndingTrigger.SetActive(false);

        StartCoroutine(
            BadJumpscareRoutine()
        );
    }

    private IEnumerator
        BadJumpscareRoutine()
    {
        LockPlayerControls();

        if (impostor != null)
        {
            if (
                impostorSpawnPoint != null)
            {
                impostor.transform.position =
                    impostorSpawnPoint.position;

                impostor.transform.rotation =
                    impostorSpawnPoint.rotation;
            }

            impostor.SetActive(true);
        }

        if (
            impostorLookTarget != null)
        {
            yield return StartCoroutine(
                TurnCameraToward(
                    impostorLookTarget,
                    badCameraTurnDuration
                )
            );
        }

        if (
            audioSource != null &&
            jumpscareSound != null)
        {
            audioSource.PlayOneShot(
                jumpscareSound
            );
        }

        yield return StartCoroutine(
            LungeImpostor()
        );

        yield return StartCoroutine(
            KnockPlayerDown()
        );

        yield return
            new WaitForSeconds(0.35f);

        yield return StartCoroutine(
            FadeToBlack()
        );

        gameEnded = true;

        if (endingText != null)
        {
            endingText.text =
                "SHIFT FAILED";

            endingText.gameObject
                .SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        Debug.Log(
            "BAD ENDING COMPLETE"
        );
    }

    private IEnumerator
        LungeImpostor()
    {
        if (
            impostor == null ||
            playerCamera == null)
            yield break;

        Vector3 startPosition =
            impostor.transform.position;

        Vector3 direction =
            playerCamera.position -
            impostor.transform.position;

        Vector3 targetPosition =
            playerCamera.position -
            direction.normalized *
            lungeStopDistance;

        float timer = 0f;

        while (
            timer < lungeDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    lungeDuration
                );

            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            impostor.transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );

            yield return null;
        }

        impostor.transform.position =
            targetPosition;
    }

    private IEnumerator
        KnockPlayerDown()
    {
        if (playerCamera == null)
            yield break;

        Vector3 startPosition =
            playerCamera.position;

        Quaternion startRotation =
            playerCamera.rotation;

        Vector3 targetPosition =
            startPosition +
            Vector3.down *
            fallDistance;

        Quaternion targetRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                0f,
                fallRoll
            );

        float timer = 0f;

        while (
            timer < fallDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    fallDuration
                );

            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            playerCamera.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );

            playerCamera.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    progress
                );

            yield return null;
        }
    }

    // ==================================================
    // CAR ENDING
    // ==================================================

    public void PlayerReachedCar()
    {
        if (!shiftEnded)
            return;

        if (gameEnded)
            return;

        if (
            endingType !=
            EndingType.Good)
            return;

        StartCoroutine(
            CompleteGoodEnding()
        );
    }

    private IEnumerator
        CompleteGoodEnding()
    {
        gameEnded = true;

        SetAllGoodScareTriggers(false);

        LockPlayerControls();

        yield return StartCoroutine(
            FadeToBlack()
        );

        if (endingText != null)
        {
            endingText.text =
                "SHIFT COMPLETE";

            endingText.gameObject
                .SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        Debug.Log(
            "GOOD ENDING COMPLETE"
        );
    }

    // ==================================================
    // CAMERA
    // ==================================================

    private IEnumerator
        TurnCameraToward(
            Transform target,
            float duration)
    {
        if (
            playerCamera == null ||
            target == null)
            yield break;

        Quaternion startRotation =
            playerCamera.rotation;

        Vector3 direction =
            target.position -
            playerCamera.position;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        float timer = 0f;

        while (
            timer < duration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    duration
                );

            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            playerCamera.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    progress
                );

            yield return null;
        }

        playerCamera.rotation =
            targetRotation;
    }

    // ==================================================
    // CONTROLS
    // ==================================================

    private void LockPlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (playerFlashlight != null)
            playerFlashlight.enabled = false;
    }

    private void UnlockPlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        if (playerFlashlight != null)
            playerFlashlight.enabled = true;
    }

    // ==================================================
    // FADE
    // ==================================================

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
            yield break;

        float startAlpha =
            fadePanel.alpha;

        float timer = 0f;

        while (
            timer < fadeDuration)
        {
            timer += Time.deltaTime;

            fadePanel.alpha =
                Mathf.Lerp(
                    startAlpha,
                    1f,
                    timer /
                    fadeDuration
                );

            yield return null;
        }

        fadePanel.alpha = 1f;
    }

    // ==================================================
    // HELPERS
    // ==================================================

    private void SetAllGoodScareTriggers(
        bool state)
    {
        if (
            supervisorScareTriggers ==
            null)
            return;

        foreach (
            GameObject trigger
            in supervisorScareTriggers)
        {
            if (trigger != null)
                trigger.SetActive(state);
        }
    }

    // ==================================================
    // TESTING
    // ==================================================

    public void TestGoodEnding()
    {
        shiftEnded = true;

        if (visitorManager != null)
            visitorManager.CloseShift();

        if (objectiveText != null)
        {
            objectiveText.text =
                "SHIFT COMPLETE - RETURN TO YOUR CAR";
        }

        PrepareGoodEnding();
    }

    public void TestBadEnding()
    {
        shiftEnded = true;

        if (visitorManager != null)
            visitorManager.CloseShift();

        if (objectiveText != null)
        {
            objectiveText.text =
                "SHIFT COMPLETE - RETURN TO YOUR CAR";
        }

        PrepareBadEnding();
    }
}