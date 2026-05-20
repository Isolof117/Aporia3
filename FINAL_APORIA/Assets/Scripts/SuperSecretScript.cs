using UnityEngine;
using UnityEngine.UI;

public class SuperSecretScript : MonoBehaviour
{
    [Header("Konami Code Settings")]
    private KeyCode[] konamiCode = new KeyCode[]
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    private int currentIndex = 0;

    [Header("Secret UI")]
    [SerializeField] private GameObject secretImageObject;

    [Header("Secret Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip secretSound;

    [Header("Optional")]
    [SerializeField] private bool disableAfterActivation = true;

    private bool activated = false;

    private void Start()
    {
        if (secretImageObject != null)
        {
            secretImageObject.SetActive(false);
        }

        // Ensure looping is enabled
        if (audioSource != null)
        {
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (activated && disableAfterActivation)
            return;

        if (Input.anyKeyDown)
        {
            CheckKonamiInput();
        }
    }

    private void CheckKonamiInput()
    {
        if (Input.GetKeyDown(konamiCode[currentIndex]))
        {
            currentIndex++;

            if (currentIndex >= konamiCode.Length)
            {
                ActivateSecret();
                currentIndex = 0;
            }
        }
        else
        {
            currentIndex = 0;

            if (Input.GetKeyDown(konamiCode[0]))
            {
                currentIndex = 1;
            }
        }
    }

    private void ActivateSecret()
    {
        activated = true;

        Debug.Log("Konami Code Entered!");

        if (secretImageObject != null)
        {
            secretImageObject.SetActive(true);
        }

        if (audioSource != null && secretSound != null)
        {
            audioSource.clip = secretSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}