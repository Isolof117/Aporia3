using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SplashToMenu : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage; // Fullscreen black image
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scene")]
    [SerializeField] private string sceneName = "MainMenu";

    private void Start()
    {
        // Ensure fade image starts transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // Subscribe to video end event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        float time = 0f;

        // Fade to black
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = time / fadeDuration;

            Color c = fadeImage.color;
            c.a = Mathf.Clamp01(alpha);
            fadeImage.color = c;

            yield return null;
        }

        // Ensure fully black
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;

        // Load scene
        SceneManager.LoadScene(sceneName);
    }
}