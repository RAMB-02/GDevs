using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingSceneController : MonoBehaviour
{
    [Header("필수 연결")]
    public VideoPlayer videoPlayer; // 인스펙터에서 연결할 VideoPlayer

    [Header("설정")]
    public string nextSceneName; // 인스펙터에서 이동할 씬 이름을 직접 입력

    private void Start()
    {
        if (videoPlayer != null)
        {
            // 영상 재생이 끝나면 OnVideoEnd 함수를 호출하도록 등록
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("VideoPlayer가 연결되지 않았습니다! 인스펙터 창에서 연결해주세요.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // nextSceneName 변수에 값이 입력되었는지 확인
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // 인스펙터에서 지정한 씬으로 이동
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // 씬 이름이 비어있으면 경고 메시지를 출력
            Debug.LogError("이동할 씬 이름이 지정되지 않았습니다! 인스펙터 창에서 Next Scene Name을 설정해주세요.");
        }
    }
}