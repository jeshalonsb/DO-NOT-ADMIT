using System.Collections;
using UnityEngine;

public class BoothAudioZone : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource outsideAmbience;
    [SerializeField] private AudioSource boothAmbience;

    [Header("Outside Volumes")]
    [SerializeField] private float outsideVolume = 0.45f;
    [SerializeField] private float boothVolumeOutside = 0f;

    [Header("Inside Booth Volumes")]
    [SerializeField] private float outsideVolumeInside = 0.08f;
    [SerializeField] private float boothVolumeInside = 0.25f;

    [Header("Crossfade")]
    [SerializeField] private float fadeDuration = 0.75f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (outsideAmbience != null)
        {
            outsideAmbience.volume =
                outsideVolume;
        }

        if (boothAmbience != null)
        {
            boothAmbience.volume =
                boothVolumeOutside;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartCrossfade(
            outsideVolumeInside,
            boothVolumeInside
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartCrossfade(
            outsideVolume,
            boothVolumeOutside
        );
    }

    private void StartCrossfade(
        float targetOutside,
        float targetBooth)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }

        fadeCoroutine =
            StartCoroutine(
                CrossfadeRoutine(
                    targetOutside,
                    targetBooth
                )
            );
    }

    private IEnumerator CrossfadeRoutine(
        float targetOutside,
        float targetBooth)
    {
        if (outsideAmbience == null ||
            boothAmbience == null)
        {
            yield break;
        }

        float startOutside =
            outsideAmbience.volume;

        float startBooth =
            boothAmbience.volume;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );

            outsideAmbience.volume =
                Mathf.Lerp(
                    startOutside,
                    targetOutside,
                    t
                );

            boothAmbience.volume =
                Mathf.Lerp(
                    startBooth,
                    targetBooth,
                    t
                );

            yield return null;
        }

        outsideAmbience.volume =
            targetOutside;

        boothAmbience.volume =
            targetBooth;

        fadeCoroutine = null;
    }
}