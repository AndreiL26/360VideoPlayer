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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isUp) {
                cvsGr.DOFade(1.0f, 0.25f).SetEase(Ease.InSine).OnComplete(() => { cvsGr.interactable = true; });

            }
            else {
                cvsGr.DOFade(0.0f, 0.25f).SetEase(Ease.OutSine).OnComplete(() => { cvsGr.interactable = false; });
            }
            isUp = !isUp;
        }
    }
}
