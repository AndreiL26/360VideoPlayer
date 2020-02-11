using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider volumeSlider;
    public Text currentMinutes;
    public Text currentSeconds;
    public Text totalMinutes;
    public Text totalSeconds;
    public float currentVolume = 1f;


    public Image playButtonImage;
    public Sprite playButtonSprite;
    public Sprite pauseButtonSprite;
    private bool isPlayingVideo;

    bool hasVolume;
    public Image sounndButtonImage;
    public Sprite soundOnButtonSprite;
    public Sprite soundOffButtonSprite;

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
        videoPlayer.frameReady += frameReady;
        videoPlayer.loopPointReached += loopPointReached;
        videoPlayer.prepareCompleted += prepareCompleted;
        videoPlayer.seekCompleted += seekCompleted;
        videoPlayer.started += started;

        hasVolume = true;
        currentVolume = 1.0f;
    }

    private void OnDisable() {
        videoPlayer.errorReceived -= errorReceived;
        videoPlayer.frameReady -= frameReady;
        videoPlayer.loopPointReached -= loopPointReached;
        videoPlayer.prepareCompleted -= prepareCompleted;
        videoPlayer.seekCompleted -= seekCompleted;
        videoPlayer.started -= started;
    }

    private void errorReceived(VideoPlayer videoPlayer, string message) {
        Debug.Log("Video Player error: " + message);
    }

    private void frameReady(VideoPlayer videoPlayer, long frame) {
        // Invoked every time a frame is ready -> CPU Costly
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
            SetCurrentTimeUI();
            UpdateVolume();
        }

        if (videoPlayer.isPlaying)
        {
            playButtonImage.sprite = pauseButtonSprite;
        } else
        {
            playButtonImage.sprite = playButtonSprite;
        }

        if (volumeSlider.value > 0.0f)
        {
            sounndButtonImage.sprite = soundOnButtonSprite;
        }
        else
        {
            sounndButtonImage.sprite = soundOffButtonSprite;
        }
        /*
        if (IsPrepared) {
            slider.value = (float)NTime;
        }
        */
    }

    private void Awake() {
        //videoPlayer.EnableAudioTrack(0, false);
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "MyVideo.mp4");
        videoPlayer.Prepare();
    }

    private void SetCurrentTimeUI() {
        string minutes = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
        string seconds = ((int)videoPlayer.time % 60).ToString("00");

        currentMinutes.text = minutes;
        currentSeconds.text = seconds;
    }

    private void SetTotalTimeUI() {
        videoPlayer.Prepare();
        string minutes = Mathf.Floor((float)videoPlayer.length / 60).ToString("00");
        string seconds = (videoPlayer.length % 60).ToString("00");
        Debug.Log(minutes);
        Debug.Log(seconds);
        totalMinutes.text = minutes;
        totalSeconds.text = seconds;
    }

    public void LoadVideo(string name) {
        string temp = Application.dataPath + "/StreamingAssets/" + name; /*.mp4,.avi,.mov*/
        if (videoPlayer.url == temp) return;

        videoPlayer.url = temp;
        videoPlayer.Prepare();

        Debug.Log("Can set direct audio volume: " + videoPlayer.canSetDirectAudioVolume);
        Debug.Log("Can set playback speed: " + videoPlayer.canSetPlaybackSpeed);
        Debug.Log("Can skip on drop: " + videoPlayer.canSetSkipOnDrop);
        Debug.Log("Can set time: " + videoPlayer.canSetTime);
        Debug.Log("Can step: " + videoPlayer.canStep);
    }

    public void SetAudio()
    {
        if (volumeSlider.value > 0.0f)
        {
            volumeSlider.value = 0.0f;
        } else
        {
            volumeSlider.value = 1.0f;
        }
    }

    public void PlayVideo() {
        if(IsPrepared)
        {
            if(!IsPlaying)
            {
                videoPlayer.Play();
            } else
            {
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

    public void LoopVideo(bool toggle) {
        if(IsPrepared) {
            videoPlayer.isLooping = toggle;
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
            videoPlayer.playbackSpeed += 1;
            videoPlayer.playbackSpeed = Mathf.Clamp(videoPlayer.playbackSpeed, 0, 10);
        }
    }

    public void DecrementPlaybackSpeed() {
        if (videoPlayer.canSetPlaybackSpeed) {
            videoPlayer.playbackSpeed -= 1;
            videoPlayer.playbackSpeed = Mathf.Clamp(videoPlayer.playbackSpeed, 0, 10);
        }
    }

    public void UpdateVolume() {
        currentVolume = volumeSlider.value;
        videoPlayer.SetDirectAudioVolume(0, currentVolume);
    }
}
