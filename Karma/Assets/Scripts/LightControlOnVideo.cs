using UnityEngine;
using UnityEngine.Video;

public class LightControlOnVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer; // private으로 변경하여 스크립트에서 직접 찾게 만듭니다.
    private Light targetLight;
    public string lightTagName = "Directional Light";
    
    void Start()
    {
        // 씬에서 VideoPlayer 컴포넌트를 찾습니다.
        videoPlayer = FindObjectOfType<VideoPlayer>();

        // 'MainLight' 태그를 가진 오브젝트를 씬에서 찾아 조명을 연결합니다.
        GameObject lightObject = GameObject.FindWithTag(lightTagName);
        if (lightObject != null)
        {
            targetLight = lightObject.GetComponent<Light>();
        }

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
                Debug.LogError("씬에서 '" + lightTagName + "' 태그를 가진 Light 오브젝트를 찾을 수 없습니다.");
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