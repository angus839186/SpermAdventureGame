using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

public class EndingVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject startPanel;
    public GameObject RestartPanel;

    public GameObject VideoPanel;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("EndingVideo: VideoPlayer is not assigned.", this);
            return;
        }

        string videoPath = Path.Combine(Application.streamingAssetsPath, "ending.mp4");

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.playOnAwake = false;

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;

        startPanel.SetActive(true);
        RestartPanel.SetActive(false);
    }

    public void OnClickPlayVideo()
    {
        startPanel.SetActive(false);
        videoPlayer.Play();
        Debug.Log("Play Video");
    }

    public void OnClickSkipVideo()
    {
        startPanel.SetActive(false);
        RestartPanel.SetActive(true);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        RestartPanel.SetActive(true);
        VideoPanel.SetActive(false);
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError($"EndingVideo: Failed to play {vp.url}. {message}", this);
    }

    void OnDestroy()
    {
        if (videoPlayer == null)
            return;

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.errorReceived -= OnVideoError;
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene("00_Title");
    }
}
