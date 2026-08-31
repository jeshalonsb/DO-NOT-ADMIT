using System.Collections;
using UnityEngine;

public class MenuLightFlicker : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light targetLight;

    [Header("Flicker Timing")]
    [SerializeField] private float minTimeBetweenFlickers = 2f;
    [SerializeField] private float maxTimeBetweenFlickers = 6f;

    [Header("Flicker Burst")]
    [SerializeField] private int minFlickersPerBurst = 1;
    [SerializeField] private int maxFlickersPerBurst = 4;

    [SerializeField] private float minFlickerDuration = 0.03f;
    [SerializeField] private float maxFlickerDuration = 0.12f;

    [Header("Optional Intensity Variation")]
    [SerializeField] private bool varyIntensity = true;
    [SerializeField] private float minimumIntensityMultiplier = 0.4f;

    private float originalIntensity;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetLight != null)
            originalIntensity = targetLight.intensity;
    }

    private void Start()
    {
        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            float waitTime =
                Random.Range(
                    minTimeBetweenFlickers,
                    maxTimeBetweenFlickers
                );

            yield return new WaitForSeconds(waitTime);

            int flickerCount =
                Random.Range(
                    minFlickersPerBurst,
                    maxFlickersPerBurst + 1
                );

            for (int i = 0; i < flickerCount; i++)
            {
                if (targetLight == null)
                    yield break;

                targetLight.enabled = false;

                yield return new WaitForSeconds(
                    Random.Range(
                        minFlickerDuration,
                        maxFlickerDuration
                    )
                );

                targetLight.enabled = true;

                if (varyIntensity)
                {
                    targetLight.intensity =
                        originalIntensity *
                        Random.Range(
                            minimumIntensityMultiplier,
                            1f
                        );
                }

                yield return new WaitForSeconds(
                    Random.Range(
                        minFlickerDuration,
                        maxFlickerDuration
                    )
                );

                targetLight.intensity = originalIntensity;
            }
        }
    }
}