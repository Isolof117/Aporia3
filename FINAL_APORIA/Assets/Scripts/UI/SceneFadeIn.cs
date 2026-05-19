using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (fadeImage != null)
        {
            // Start fully black
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;

            // Make sure it blocks clicks during fade
            fadeImage.raycastTarget = true;

            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        // Ensure fully transparent
        Color finalColor = fadeImage.color;
        finalColor.a = 0f;
        fadeImage.color = finalColor;

        // Stop blocking clicks
        fadeImage.raycastTarget = false;

        // Optional: fully disable the object
        fadeImage.gameObject.SetActive(false);
    }
}