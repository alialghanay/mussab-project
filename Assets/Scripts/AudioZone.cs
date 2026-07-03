using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class AudioZone : MonoBehaviour
{
    public AudioSource ambienceSource;
    public float fadeInSeconds = 1.5f;
    public float fadeOutSeconds = 1.5f;
    public float targetVolume = 1f;

    Coroutine activeFade;

    void Start()
    {
        if (ambienceSource != null)
        {
            ambienceSource.volume = 0f;
            ambienceSource.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || ambienceSource == null) return;
        StopActiveFade();
        activeFade = StartCoroutine(FadeTo(targetVolume, fadeInSeconds));
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || ambienceSource == null) return;
        StopActiveFade();
        activeFade = StartCoroutine(FadeTo(0f, fadeOutSeconds));
    }

    void StopActiveFade()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
            activeFade = null;
        }
    }

    IEnumerator FadeTo(float target, float duration)
    {
        if (duration <= 0f)
        {
            ambienceSource.volume = target;
            yield break;
        }

        float start = ambienceSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        ambienceSource.volume = target;
        activeFade = null;
    }
}
