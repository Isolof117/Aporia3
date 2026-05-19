using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public static ButtonAudio Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("UI Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHover()
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}