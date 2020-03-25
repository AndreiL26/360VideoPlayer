using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System;

public class VideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider loopSlider;
    [SerializeField] private Text playbackSpeedText;
    [SerializeField] private Text timeText;
    [SerializeField] private Image playButtonImage;
    [SerializeField] private Sprite playButtonSprite;
    [SerializeField] private Sprite pauseButtonSprite;
    [SerializeField] private Image sounndButtonImage;
    [SerializeField] private Sprite soundOnButtonSprite;
    [SerializeField] private Sprite soundOffButtonSprite;
    [SerializeField] private HighlightMgr highlightMgr;
    private int currentVideoIndex;
    private List<string> videoPaths = new List<string>();
    private VideoData[] videoData = new VideoData[500];
    private float currentVolume = 1f;

    [Serializable]
    private class VideoData {
        public int Id;
        public string VideoLink;
        public VideoData(int Id, string VideoLink) {
            this.Id = Id;
            this.VideoLink = VideoLink;
        }
    }

    public bool IsPlaying {
        get { return videoPlayer.isPlaying; }
    }
 
    public bool IsPrepared {
        get { return videoPlayer.isPrepared; }
    }

    public double Time {
        get { return videoPlayer.time; }
    }

    public ulong Duration {
        get { return (ulong)(videoPlayer.frameCount/ videoPlayer.frameRate); }
    }

    private void OnEnable() {
        currentVolume = 1.0f;
    }
    
    private void Update() {
        if(videoPlayer.isPlaying) {
            UpdateVolume();
        }

        if (videoPlayer.isPlaying) {
            playButtonImage.sprite = pauseButtonSprite;
        } else {
            playButtonImage.sprite = playButtonSprite;
        }

        if (volumeSlider.value > 0.0f) {
            sounndButtonImage.sprite = soundOnButtonSprite;
        }
        else {
            sounndButtonImage.sprite = soundOffButtonSprite;
        }

        playbackSpeedText.text = videoPlayer.playbackSpeed.ToString("f2");

        string totalMinutes = Mathf.Floor ((float)videoPlayer.length / 60).ToString ("00");
        string totalSeconds = (videoPlayer.length % 60).ToString ("00");
        string currMinutes = Mathf.Floor ((int)videoPlayer.time / 60).ToString ("00");
        string currSeconds = ((int)videoPlayer.time % 60).ToString ("00");
        timeText.text = currMinutes + ":" + currSeconds +"/" + totalMinutes + ":" + totalSeconds;
    }
    private void Start() {
        StartCoroutine(GetText());
    }

    IEnumerator GetText() {
        using (UnityWebRequest www = UnityWebRequest.Get("https://getvideosphp.azurewebsites.net/")) {
            yield return www.SendWebRequest();
            
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string jsonString = JSONHelper.fixJson(www.downloadHandler.text);
                videoData = JSONHelper.FromJson<VideoData>(jsonString);
                int i = 0;
                while (i < videoData.Length && videoData[i].VideoLink != null) {
                    videoPaths.Add(videoData[i].VideoLink);
                    ++i;
                }
                LoadVideo(videoPaths[0]);
            }
        }
    }
    
    public void LoadVideo(string videoPath) {
        if (videoPlayer.url == videoPath) return;
        videoPlayer.url = videoPath;
        videoPlayer.Prepare();
    }

    public void SetAudio() {
        if (volumeSlider.value > 0.0f) {
            volumeSlider.value = 0.0f;
        } else {
            volumeSlider.value = 1.0f;
        }
    }

    public void PlayVideo() {
        if(IsPrepared) {
            if(!IsPlaying) {
                videoPlayer.Play();
            } else {
                videoPlayer.Pause ();
            }
        }
    }

    public void PauseVideo() {
        if(IsPlaying) {
            videoPlayer.Pause();
        }
    }

    public void RestartVideo() {
        if(IsPrepared) {
            PauseVideo();
            Seek(0);
        }
    }

    public void LoopVideo() {
        if (loopSlider.value == 0) {
            videoPlayer.isLooping = false;
        }
        else {
            videoPlayer.isLooping = true;
        }
    }

    public void Seek(float nTime) {
        if(videoPlayer.canSetTime && IsPrepared) {
            nTime = Mathf.Clamp(nTime, 0, 1);
            videoPlayer.time = nTime * Duration;
        }
    }

    public void IncremenetPlaybackSpeed() {
        if (videoPlayer.canSetPlaybackSpeed) {
            videoPlayer.playbackSpeed += 0.25f;
            videoPlayer.playbackSpeed = Mathf.Clamp(videoPlayer.playbackSpeed, 0, 10);
        }
    }

    public void DecrementPlaybackSpeed() {
        if (videoPlayer.canSetPlaybackSpeed) {
            videoPlayer.playbackSpeed -= 0.25f;
            videoPlayer.playbackSpeed = Mathf.Clamp(videoPlayer.playbackSpeed, 0, 10);
        }
    }

    public void UpdateVolume() {
        currentVolume = volumeSlider.value;
        videoPlayer.SetDirectAudioVolume(0, currentVolume);
    }

    public void LoadNextVideo() {
        highlightMgr.RemoveAllRegionsFromShader();
        if(currentVideoIndex < videoPaths.Count - 1) {
            currentVideoIndex++;
            LoadVideo(videoPaths[currentVideoIndex]);
            videoPlayer.Play();
        }
    }

    public void LoadPreviousVideo() {
        highlightMgr.RemoveAllRegionsFromShader();
        if(currentVideoIndex > 0) {
            currentVideoIndex--;
            LoadVideo(videoPaths[currentVideoIndex]);
            videoPlayer.Play();
        }
    }
}