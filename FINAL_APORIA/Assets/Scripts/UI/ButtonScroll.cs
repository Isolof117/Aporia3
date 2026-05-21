using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScroll : MonoBehaviour
{
    [Header("Objects To Cycle Through")]
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    [Header("Optional Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentIndex = 0;

    private void Start()
    {
        // Assign button listeners if buttons are set
        if (nextButton != null)
            nextButton.onClick.AddListener(NextObject);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousObject);

        UpdateObjects();
    }

    // Activates next object in the list
    public void NextObject()
    {
        if (objects.Count == 0) return;

        currentIndex++;

        // Loop back to start
        if (currentIndex >= objects.Count)
            currentIndex = 0;

        UpdateObjects();
    }

    // Activates previous object in the list
    public void PreviousObject()
    {
        if (objects.Count == 0) return;

        currentIndex--;

        // Loop back to end
        if (currentIndex < 0)
            currentIndex = objects.Count - 1;

        UpdateObjects();
    }

    // Turns all objects off except the current one
    private void UpdateObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(i == currentIndex);
        }
    }
}