using UnityEngine;
using UnityEngine.Video;

public class LightControlOnVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    // public으로 변경하여 인스펙터에서 직접 라이트를 드래그 앤 드롭으로 할당합니다.
    public Light targetLight;

    void Start()
    {
        // 씬에서 VideoPlayer 컴포넌트를 찾습니다.
        videoPlayer = FindObjectOfType<VideoPlayer>();

        // targetLight가 인스펙터에서 할당되었는지 확인합니다.
        if (videoPlayer != null && targetLight != null)
        {
            videoPlayer.started += OnVideoStarted;
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            // 오류 메시지를 더 구체적으로 표시하여 디버깅을 돕습니다.
            if (videoPlayer == null)
            {
                Debug.LogError("씬에서 VideoPlayer를 찾을 수 없습니다.");
            }
            if (targetLight == null)
            {
                // 오류 메시지를 인스펙터에서 할당해야 함을 명확히 알립니다.
                Debug.LogError("Target Light가 할당되지 않았습니다. 인스펙터에서 Light 오브젝트를 연결해주세요.");
            }
        }
    }

    void OnVideoStarted(VideoPlayer vp)
    {
        if (targetLight != null)
        {
            targetLight.enabled = false;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (targetLight != null)
        {
            targetLight.enabled = true;
        }
    }
}