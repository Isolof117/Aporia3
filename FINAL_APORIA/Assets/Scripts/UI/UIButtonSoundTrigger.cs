using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ButtonAudio.Instance != null)
            ButtonAudio.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ButtonAudio.Instance != null)
            ButtonAudio.Instance.PlayClick();
    }
}