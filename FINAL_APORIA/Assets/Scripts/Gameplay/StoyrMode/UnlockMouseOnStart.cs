using UnityEngine;

public class UnlockMouseOnStart : MonoBehaviour
{
    void Start()
    {
        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;

        // Make the cursor visible
        Cursor.visible = true;
    }
}