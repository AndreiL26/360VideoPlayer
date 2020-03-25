using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AddZoneController : MonoBehaviour
{
    [SerializeField] private HighlightMgr highlightMgr;
    [SerializeField] private CanvasGroup addZonePanel;
    [SerializeField] private Slider centerXSlider;
    [SerializeField] private Slider centerYSlider;
    [SerializeField] private Slider sizeXSlider;
    [SerializeField] private Slider sizeYSlider;
    [SerializeField] private InputField descriptionInputField;
    [SerializeField] private InputField startTimeInputField;
    [SerializeField] private InputField endTimeInputField;
    private bool isUp = false;

    private void Start() {
        addZonePanel.interactable = false;
        addZonePanel.alpha = 0;
    }

    public void ShowAddZonePanel() {
        addZonePanel.DOKill();
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
        highlightMgr.ModifyPreviewValues(centerXSlider.value, centerYSlider.value, sizeXSlider.value, sizeYSlider.value);
    }

    public void AddPreviewRegionToData() {
        ShowAddZonePanel();
        float startTime;
        float endTime;
        if (float.TryParse(startTimeInputField.text, out float startResult)) {
            startTime = startResult;
        }
        else {
            startTime = 0;
        }
        if (float.TryParse(endTimeInputField.text, out float endResult)) {
            endTime = endResult;
        }
        else {
            endTime = 0;
        }
        RegionData newRegion = new RegionData(centerXSlider.value, centerYSlider.value, sizeXSlider.value, sizeYSlider.value,
                                        descriptionInputField.text, startTime, endTime);
        highlightMgr.AddPreviewRegionsToRegionsData(newRegion);
    }
}
