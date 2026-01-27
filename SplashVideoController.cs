using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainMenu";
    public bool allowSkip = true;
    public KeyCode skipKey = KeyCode.Space;

    private bool _loading;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (!allowSkip) return;

        if (Input.GetKeyDown(skipKey))
            LoadNext();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNext();
    }

    private void LoadNext()
    {
        if (_loading) return;
        _loading = true;

        videoPlayer.Stop();
        SceneManager.LoadScene(nextSceneName);
    }
}
