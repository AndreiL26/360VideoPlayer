using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoProgressBar : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Slider slider;

    void Awake()
    {
        videoPlayer = GameObject.Find("360 Video Player").GetComponent<VideoPlayer>();
        slider = GetComponent<Slider>();   
    }

    void Update()
    {
        if (videoPlayer.frameCount > 0) {
            slider.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
    }

}
