using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SoundSliderHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform volumeSlider;
    [SerializeField] private RectTransform timeTextRT;
    private Vector2 initialAnchorsMin;
    private Vector2 initialAnchorsMax;
    private SoundSliderBehaviour soundSliderBehaviour;

    private void Start () {
        soundSliderBehaviour = volumeSlider.GetComponent<SoundSliderBehaviour> ();
        initialAnchorsMax = timeTextRT.anchorMax;
        initialAnchorsMin = timeTextRT.anchorMin;
        volumeSlider.localScale = new Vector3 (0.0f, 1.0f, 1.0f);
    }

    public void OnPointerExit (PointerEventData eventData) {
        soundSliderBehaviour.pointerOnButton  = false;
    }

    public void OnPointerEnter (PointerEventData eventData) {
        soundSliderBehaviour.pointerOnButton = true;
        if (!soundSliderBehaviour.enabled) {
            soundSliderBehaviour.enabled = true;
            volumeSlider.DOKill ();
            timeTextRT.DOKill ();
            volumeSlider.DOScaleX (1.0f, 0.1f);
            timeTextRT.DOAnchorMax (initialAnchorsMax + new Vector2 (0.07f, 0.0f), 0.2f);
            timeTextRT.DOAnchorMin (initialAnchorsMin + new Vector2 (0.07f, 0.0f), 0.2f);
        }
    }
}
