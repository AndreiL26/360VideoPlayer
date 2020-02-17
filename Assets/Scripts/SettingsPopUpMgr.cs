﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsPopUpMgr : MonoBehaviour
{
    public CanvasGroup settingsPanel;
    public Transform img;
    bool isUp = false;

    void Start()
    {
        settingsPanel.interactable = false;
        settingsPanel.alpha = 0;
    }

    public void PressedSettingsButton()
    {
        settingsPanel.DOKill ();
        img.DOKill ();
        if (!isUp)
        {
            img.DORotate (new Vector3(0.0f, 0.0f, 45.0f), 0.25f).SetEase (Ease.InSine);
            settingsPanel.DOFade (1.0f, 0.25f).SetEase (Ease.InSine).OnComplete (() => { settingsPanel.interactable = true; });
        }
        else
        {
            img.DORotate (new Vector3 (0.0f, 0.0f, 0.0f), 0.25f).SetEase (Ease.OutSine);
            settingsPanel.DOFade (0.0f, 0.25f).SetEase (Ease.OutSine).OnComplete (() => { settingsPanel.interactable = false; });
        }

        isUp = !isUp;
    }
}
