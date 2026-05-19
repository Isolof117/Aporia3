using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage; // Black UI Image
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (fadeImage != null)
        {
            // Keep it completely OFF at start
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void SwitchScene()
    {
        StartCoroutine(FadeAndSwitch());
    }

    private IEnumerator FadeAndSwitch()
    {
        if (fadeImage != null)
        {
            // Turn ON when fade starts
            fadeImage.gameObject.SetActive(true);

            // Start transparent
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            // Block clicks during fade
            fadeImage.raycastTarget = true;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }

            yield return null;
        }

        // Ensure fully black
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        SceneManager.LoadScene(sceneName);
    }
}