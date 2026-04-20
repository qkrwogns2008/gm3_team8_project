using System.Collections;
using UnityEngine;

public class UI_sound : MonoBehaviour
{
    [SerializeField] private UIAudioSO _audioData; // 오디오 데이터

    private void OnEnable()
    {
        // 1. 데이터 체크
        if (_audioData == null)
        {
            Debug.LogError($"{gameObject.name}에 Audio Data가 비어있습니다!");
            return;
        }

        // 2. 사운드 매니저 준비 체크
        /*
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISFX(_audioData.uiOn);
        }
        else
        {
            StartCoroutine(CoPlaySFXDelayed()); // 딜레이 재생
        }
        */
    }

    // =============================
    // UI 효과음 재생


    private IEnumerator CoPlaySFXDelayed()
    {
        yield return null; // 한 프레임 대기

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISFX(_audioData.uiOn);
        }
    }
    public void PlayUISound_OnClick()
    {
        Debug.Log("UI S1");
        if (_audioData == null) return;
        Debug.Log("UI S2");
        if (SoundManager.Instance != null)
        {
            Debug.Log("UI S3");
            SoundManager.Instance.PlayUISFX(_audioData.uiOn);
            Debug.Log("UI S4");
        }
        Debug.Log("UI S5");
    }
}