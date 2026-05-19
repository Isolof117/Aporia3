using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public VideoClip newVideo;

    [Header("Objects To Disable")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    [Header("Audio Settings")]
    public float audioFadeDuration = 1.5f;

    [Header("Scene Settings")]
    public bool loadNextSceneOnFinish = true;

    private bool hasSwitchedVideo = false;
    private bool fadeStarted = false;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        // Start fully transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    private void Update()
    {
        //  Start fade BEFORE video ends
        if (hasSwitchedVideo && !fadeStarted && videoPlayer.isPlaying)
        {
            double timeRemaining = videoPlayer.length - videoPlayer.time;

            if (timeRemaining <= fadeDuration)
            {
                fadeStarted = true;
                StartCoroutine(FadeOut());
                StartCoroutine(FadeAudio());
            }
        }
    }

    public void OnButtonPressed()
    {
        if (videoPlayer != null && newVideo != null)
        {
            videoPlayer.isLooping = false;
            videoPlayer.clip = newVideo;

            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;

            hasSwitchedVideo = true;
            fadeStarted = false;
        }
        else
        {
            Debug.LogWarning("VideoPlayer or newVideo not assigned!");
        }

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared;
        vp.Play();
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = time / fadeDuration;

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }

            yield return null;
        }

        // Ensure full black
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }
    }

    private IEnumerator FadeAudio()
    {
        if (videoPlayer == null) yield break;

        float startVolume = videoPlayer.GetDirectAudioVolume(0);
        float time = 0f;

        while (time < audioFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / audioFadeDuration;

            float volume = Mathf.Lerp(startVolume, 0f, t);
            videoPlayer.SetDirectAudioVolume(0, volume);

            yield return null;
        }

        videoPlayer.SetDirectAudioVolume(0, 0f);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (hasSwitchedVideo && loadNextSceneOnFinish)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
}