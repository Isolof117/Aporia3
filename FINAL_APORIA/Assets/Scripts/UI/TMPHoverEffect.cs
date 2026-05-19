using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TMPHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Settings")]
    public TMP_Text text;

    [Header("Hover Settings")]
    public float hoverFontSizeIncrease = 6f;
    public float animationSpeed = 8f;

    private float baseFontSize;
    private string originalText;
    private Coroutine animCoroutine;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        baseFontSize = text.fontSize;
        originalText = text.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);

        text.text = originalText.ToUpper();

        animCoroutine = StartCoroutine(AnimateFontSize(baseFontSize + hoverFontSizeIncrease));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);

        text.text = originalText;

        animCoroutine = StartCoroutine(AnimateFontSize(baseFontSize));
    }

    private IEnumerator AnimateFontSize(float targetSize)
    {
        while (Mathf.Abs(text.fontSize - targetSize) > 0.01f)
        {
            text.fontSize = Mathf.Lerp(text.fontSize, targetSize, Time.deltaTime * animationSpeed);
            yield return null;
        }

        text.fontSize = targetSize;
    }
}