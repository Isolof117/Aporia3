using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelWhenEnemiesGone : MonoBehaviour
{
    [Header("Enemy Settings")]
    public string enemyTag = "Enemy";

    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private bool endingTriggered = false;

    void Start()
    {
        // IMPORTANT: ensure fade image is active so it can render
        if (fadeImage != null)
            fadeImage.gameObject.SetActive(true);

        // start fully transparent
        SetAlpha(0f);
    }

    void Update()
    {
        if (endingTriggered) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            endingTriggered = true;
            StartCoroutine(FadeAndLoadScene());
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        float timer = 0f;

        // fade to black
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Clamp01(timer / fadeDuration);
            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(1f);

        // small delay so final frame is visible
        yield return null;

        SceneManager.LoadScene(sceneToLoad);
    }

    void SetAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}