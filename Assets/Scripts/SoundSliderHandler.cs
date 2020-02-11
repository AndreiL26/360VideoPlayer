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

    private void Start ()
    {
        sb = volumeSlider.GetComponent<SliderBehaviour> ();
        volumeSlider.localScale = new Vector3 (0.0f, 1.0f, 1.0f);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        sb.pointerOnButton = false;
        //if (sb.hasPointer == false)
        //{
        //    sb.CanExit = true;
        //    volumeSlider.DOKill ();
        //    volumeSlider.DOScaleX (0.0f, 0.1f);
        //}
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        sb.pointerOnButton = true;
        if (!sb.enabled)
        {
            sb.enabled = true;
            volumeSlider.DOKill ();
            volumeSlider.DOScaleX (1.0f, 0.1f);
        }
    }
}
