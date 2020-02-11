using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShowBar : MonoBehaviour
{
    RectTransform bar;
    CanvasGroup cvsGr;
    bool isUp = false;

    void Start()
    {
        bar = this.GetComponent<RectTransform> ();
        cvsGr = this.GetComponent<CanvasGroup> ();
        cvsGr.interactable = false;
        //bar.anchorMax = new Vector2 (1.0f, 0.0f);
        cvsGr.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log ("Pressed A");
            if (!isUp)
            {
                Debug.Log ("I should go up");
                cvsGr.DOFade (1.0f, 0.25f).SetEase (Ease.InSine).OnComplete(()=> { cvsGr.interactable = true; });
                
                //bar.DOAnchorMax(new Vector2 (1.0f, 0.1f), 0.1f).SetEase (Ease.InSine);
            } else
            {
                Debug.Log ("I should go down");
                cvsGr.DOFade (0.0f, 0.25f).SetEase (Ease.OutSine).OnComplete (() => { cvsGr.interactable = false; });
                //bar.DOAnchorMax (new Vector2(1.0f, 0.0f), 0.1f).SetEase (Ease.OutSine);
            }

            isUp = !isUp;
        }
    }
}
