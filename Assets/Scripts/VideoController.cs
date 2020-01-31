using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;

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
        if (IsPrepared) {
            slider.value = (float)NTime;
        }
    }

    public void LoadVideo(string name) {
        string temp = Application.dataPath + "/StreamingAssets/" + name; /*.mp4,.avi,.mov*/

    }

    public void PlayVideo() {

    }

    public void PauseVideo() {

    }

    public void RestardVideo() {

    }

    public void LoopVideo(bool toggle) {

    }

    public void Seek(float nTime) {

    }

    public void IncremenetPlaybackSpeed() {

    }

    public void DecrementPlaybackSpeed() {

    }


 
}
