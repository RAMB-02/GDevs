using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingSceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // EndingScene에 붙은 VideoPlayer

    private void Start()
    {
        if (videoPlayer != null)
        {
            // 영상이 끝나면 자동으로 StartMenuScene 이동
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("VideoPlayer가 연결되지 않았습니다!");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("EndingScene2");
    }
}
