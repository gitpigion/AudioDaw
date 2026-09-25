using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void Play()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "/Users/benjiegan/Documents/benji/Python dataset/Test2.mp4";

        videoPlayer.Play();
    }

    public void RestartVideo()
    {
        videoPlayer.Pause();
        videoPlayer.time = 0;
        videoPlayer.Play();
    }
}