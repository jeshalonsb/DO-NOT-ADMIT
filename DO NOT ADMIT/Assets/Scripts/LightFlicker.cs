using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light lightSource;

    [Header("Flicker Timing")]
    [SerializeField] private float minimumTimeBetweenFlickers = 5f;
    [SerializeField] private float maximumTimeBetweenFlickers = 15f;

    [Header("Flicker Settings")]
    [SerializeField] private int minimumFlickers = 1;
    [SerializeField] private int maximumFlickers = 4;

    [SerializeField] private float minimumFlickerSpeed = 0.03f;
    [SerializeField] private float maximumFlickerSpeed = 0.12f;

    private bool flickerEnabled = false;

    private void Start()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light>();

        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(
                minimumTimeBetweenFlickers,
                maximumTimeBetweenFlickers);

            yield return new WaitForSeconds(waitTime);

            if (flickerEnabled &&
                lightSource != null &&
                lightSource.enabled)
            {
                yield return StartCoroutine(Flicker());
            }
        }
    }

    private IEnumerator Flicker()
    {
        int flickerCount = Random.Range(
            minimumFlickers,
            maximumFlickers + 1);

        for (int i = 0; i < flickerCount; i++)
        {
            lightSource.enabled = false;

            yield return new WaitForSeconds(
                Random.Range(
                    minimumFlickerSpeed,
                    maximumFlickerSpeed));

            lightSource.enabled = true;

            yield return new WaitForSeconds(
                Random.Range(
                    minimumFlickerSpeed,
                    maximumFlickerSpeed));
        }
    }

    public void SetFlickerEnabled(bool state)
    {
        flickerEnabled = state;
    }
}