using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SoundSliderHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform volumeSlider;
    SliderBehaviour sb;
    public RectTransform timeTextRT;

    private Vector2 initialAnchorsMin;
    private Vector2 initialAnchorsMax;

    private void Start ()
    {
        sb = volumeSlider.GetComponent<SliderBehaviour> ();
        initialAnchorsMax = timeTextRT.anchorMax;
        initialAnchorsMin = timeTextRT.anchorMin;
        volumeSlider.localScale = new Vector3 (0.0f, 1.0f, 1.0f);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        sb.pointerOnButton = false;
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        sb.pointerOnButton = true;
        if (!sb.enabled)
        {
            sb.enabled = true;
            volumeSlider.DOKill ();
            timeTextRT.DOKill ();
            volumeSlider.DOScaleX (1.0f, 0.1f);
            timeTextRT.DOAnchorMax (initialAnchorsMax + new Vector2 (0.07f, 0.0f), 0.2f);
            timeTextRT.DOAnchorMin (initialAnchorsMin + new Vector2 (0.07f, 0.0f), 0.2f);
        }
    }
}
