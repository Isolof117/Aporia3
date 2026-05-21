using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TriggerImageFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleTime = 2f;

    private Coroutine currentRoutine;

    private void Start()
    {
        fadeImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(FadeRoutine());
        }
    }

    private IEnumerator FadeRoutine()
    {
        fadeImage.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(0f, 1f));

        yield return new WaitForSeconds(visibleTime);

        yield return StartCoroutine(Fade(1f, 0f));

        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float start, float end)
    {
        float timer = 0f;

        Color c = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            c.a = Mathf.Lerp(start, end, timer / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        c.a = end;
        fadeImage.color = c;
    }
}