using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject objectToHide;

    [SerializeField] private GameObject objectToShow;

    private bool isPaused = false;

    private void Start()
    {
        // Starting visibility
        if (objectToHide != null)
            objectToHide.SetActive(true);

        if (objectToShow != null)
            objectToShow.SetActive(false);

        // Lock cursor at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure game runs normally
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Toggle pause with ESC or P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Swap object visibility
        if (objectToHide != null)
            objectToHide.SetActive(!isPaused);

        if (objectToShow != null)
            objectToShow.SetActive(isPaused);

        if (isPaused)
        {
            // Pause game
            Time.timeScale = 0f;

            // Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Resume game
            Time.timeScale = 1f;

            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // For Resume Button
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }
}