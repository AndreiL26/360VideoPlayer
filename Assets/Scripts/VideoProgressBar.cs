using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer videoPlayer;
    private Image progress;

    void Awake() {
        progress = GetComponent<Image>();   
    }

    void Update() { 
        if (videoPlayer.frameCount > 0) {
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        TrySkip(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) {
        TrySkip(eventData);
    }

    private void TrySkip(PointerEventData eventData) {
        // Handles skipping the video to the percentage clicked in the progress bar
        Vector2 localPoint;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, null, out localPoint)) { 
            // change null to the camera used if you are using a world position
            float percent = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            SkipToPercent(percent);
        }
    }

    private void SkipToPercent(float percent) {
        var frame = videoPlayer.frameCount * percent;
        videoPlayer.frame = (long)frame;
    }
}
