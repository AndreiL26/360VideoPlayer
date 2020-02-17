﻿using System.Collections;
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

    public RectTransform timeTextRT;

    private Vector2 initialAnchorsMin;
    private Vector2 initialAnchorsMax;
    
    private void Start ()
    {
        hasPointer = false;
        initialAnchorsMax = timeTextRT.anchorMax;
        initialAnchorsMin = timeTextRT.anchorMin;
    }

    void Update ()
    {
        if (enabled && !hasPointer && !pointerOnButton)
        {
            enabled = false;
            this.DOKill ();
            timeTextRT.DOKill ();
            this.transform.DOScaleX (0.0f, 0.1f);
            timeTextRT.DOAnchorMax (initialAnchorsMax, 0.2f);
            timeTextRT.DOAnchorMin (initialAnchorsMin, 0.2f);
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
