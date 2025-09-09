using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSceneChanger : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 인스펙터 창에서 연결할 비디오 플레이어
    public string nextSceneName; // 이동할 다음 씬의 이름

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // 비디오 재생이 끝났을 때 호출될 이벤트에 메소드를 등록합니다.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // 비디오 재생이 끝났을 때 호출되는 메소드
    void OnVideoFinished(VideoPlayer vp)
    {
        // 이벤트 리스너를 제거하여 중복 호출을 방지합니다.
        vp.loopPointReached -= OnVideoFinished;
        // 지정된 다음 씬으로 전환합니다.
        SceneManager.LoadScene(nextSceneName);
    }
}