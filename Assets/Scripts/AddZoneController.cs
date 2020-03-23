using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AddZoneController : MonoBehaviour
{
    public HighlightMgr highlightMgr;
    public CanvasGroup addZonePanel;

    public Slider centerXSlider;
    public Slider centerYSlider;
    public Slider sizeXSlider;
    public Slider sizeYSlider;
    public InputField descriptionInputField;
    public InputField startTimeInputField;
    public InputField endTimeInputField;

    bool isUp = false;

    void Start() {
        addZonePanel.interactable = false;
        addZonePanel.alpha = 0;
    }

    public void ShowAddZonePanel() {
        addZonePanel.DOKill ();
        if (!isUp) {
            addZonePanel.DOFade (1.0f, 0.25f).SetEase (Ease.InSine).OnComplete (() => { addZonePanel.interactable = true; });
            Vector4 sliderValues = highlightMgr.SetupPreviewRegion();
            centerXSlider.value = sliderValues.x;
            centerYSlider.value = sliderValues.y;
            sizeXSlider.value = sliderValues.z;
            sizeYSlider.value = sliderValues.w;
            descriptionInputField.text = " ";
            startTimeInputField.text = "0";
            endTimeInputField.text = "0";
        }
        else {
            addZonePanel.DOFade (0.0f, 0.25f).SetEase (Ease.OutSine).OnComplete (() => { addZonePanel.interactable = false; });
            highlightMgr.StopPreviewRegion();
        }

        isUp = !isUp;
    }

    public void ModifiedRegionsValues() {
        highlightMgr.ModifyPreviewValues(centerXSlider.value, centerYSlider.value, sizeXSlider.value, sizeYSlider.value, descriptionInputField.text, startTimeInputField.text, endTimeInputField.text);
    }

    public void AddPreviewRegionToData() {
        ShowAddZonePanel();
        highlightMgr.AddPreviewRegionsToRegionsData();
    }
}
