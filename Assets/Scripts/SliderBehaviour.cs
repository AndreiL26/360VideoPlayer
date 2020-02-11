using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SliderBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hasPointer;
    public bool pointerOnButton;
    public bool enabled;

    private void Start ()
    {
        hasPointer = false;
    }

    void Update ()
    {
        if (enabled && !hasPointer && !pointerOnButton)
        {
            enabled = false;
            this.DOKill ();
            this.transform.DOScaleX (0.0f, 0.1f);
        }
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        hasPointer = false;
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        hasPointer = true;
    }
}
