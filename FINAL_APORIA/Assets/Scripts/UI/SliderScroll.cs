using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScroll : MonoBehaviour
{
    [Header("Image To Change")]
    [SerializeField] private Image targetImage;

    [Header("Sprites To Cycle Through")]
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [Header("Optional Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentIndex = 0;

    private void Start()
    {
        // Assign button listeners if buttons are set
        if (nextButton != null)
            nextButton.onClick.AddListener(NextSprite);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousSprite);

        UpdateSprite();
    }

    // Moves to next sprite
    public void NextSprite()
    {
        if (sprites.Count == 0 || targetImage == null) return;

        currentIndex++;

        // Loop back to start
        if (currentIndex >= sprites.Count)
            currentIndex = 0;

        UpdateSprite();
    }

    // Moves to previous sprite
    public void PreviousSprite()
    {
        if (sprites.Count == 0 || targetImage == null) return;

        currentIndex--;

        // Loop back to end
        if (currentIndex < 0)
            currentIndex = sprites.Count - 1;

        UpdateSprite();
    }

    // Updates the displayed sprite
    private void UpdateSprite()
    {
        if (targetImage != null && sprites.Count > 0)
        {
            targetImage.sprite = sprites[currentIndex];
        }
    }
}