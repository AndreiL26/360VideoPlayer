using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using System.Collections.Generic;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;
    public Slider loopSlider;
    public Text playbackSpeedText;
    public Text timeText;
    public Image playButtonImage;
    public Sprite playButtonSprite;
    public Sprite pauseButtonSprite;
    public Image sounndButtonImage;
    public Sprite soundOnButtonSprite;
    public Sprite soundOffButtonSprite;
    [SerializeField] private HighlightMgr highlightMgr;
    private bool isPlayingVideo;
    private bool hasVolume;
    private int currentVideoIndex;
    private List<string> videoPaths = new List<string>();
    private float currentVolume = 1f;
    // video player properties
    private bool hasFinished;
    
    public bool IsPlaying {
        get { return videoPlayer.isPlaying; }
    }
 
    public bool IsLooping {
       get { return videoPlayer.isLooping; }
    }

    public bool IsPrepared {
        get { return videoPlayer.isPrepared; }
    }

    public bool HasFinished {
        get { return hasFinished; }
    }

    public double Time {
        get { return videoPlayer.time; }
    }

    public ulong Duration {
        get { return (ulong)(videoPlayer.frameCount/ videoPlayer.frameRate); }
    }

    public double NTime {
        get { return Time / Duration; }
    }

    private void OnEnable() {
        videoPlayer.errorReceived += errorReceived;
        videoPlayer.loopPointReached += loopPointReached;
        videoPlayer.prepareCompleted += prepareCompleted;
        videoPlayer.seekCompleted += seekCompleted;
        videoPlayer.started += started;
        hasVolume = true;
        currentVolume = 1.0f;
    }

    private void OnDisable() {
        videoPlayer.errorReceived -= errorReceived;
        videoPlayer.loopPointReached -= loopPointReached;
        videoPlayer.prepareCompleted -= prepareCompleted;
        videoPlayer.seekCompleted -= seekCompleted;
        videoPlayer.started -= started;
    }

    private void errorReceived(VideoPlayer videoPlayer, string message) {
        Debug.Log("Video Player error: " + message);
    }

    private void loopPointReached(VideoPlayer videoPlayer) {
        Debug.Log("Video Player loop point reached");
        hasFinished = true;
    }

    private void prepareCompleted(VideoPlayer videoPlayer) {
        Debug.Log("Video Player finished preparing");
        hasFinished = false;
    }

    private void seekCompleted(VideoPlayer videoPlayer) {
        Debug.Log("Video Player finished seeking");
        hasFinished = false;
    }
    
    private void started(VideoPlayer videoPlayer) {
        Debug.Log("Video Player started");
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

    private void Awake() {
        //Super Hard Coded just to show functionality
        //videoPaths.Add("https://team9.blob.core.windows.net/testcontainer/3D_Waterfront.mp4?se=2020-09-09&sp=rwdlac&sv=2018-03-28&ss=b&srt=sco&sig=A2iAIQZWf6xSXXO%2Bxw91D6tTcIPfBfCgOriddgBS6%2BQ%3D");

        videoPaths.Add(Path.Combine(Application.streamingAssetsPath, "MyVideo.mp4"));
        videoPaths.Add(Path.Combine(Application.streamingAssetsPath, "ThirdVideo.mp4"));

    }

    private void Start() {
        LoadVideo(videoPaths[0]);
    }

    public void LoadVideo(string videoPath) {
        //string temp = Application.dataPath + "/StreamingAssets/" + name; /*.mp4,.avi,.mov*/
        if (videoPlayer.url == videoPath) return;

        videoPlayer.url = videoPath;
        videoPlayer.Prepare();

        Debug.Log("Can set direct audio volume: " + videoPlayer.canSetDirectAudioVolume);
        Debug.Log("Can set playback speed: " + videoPlayer.canSetPlaybackSpeed);
        Debug.Log("Can skip on drop: " + videoPlayer.canSetSkipOnDrop);
        Debug.Log("Can set time: " + videoPlayer.canSetTime);
        Debug.Log("Can step: " + videoPlayer.canStep);
    }

    public void SetAudio()
    {
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