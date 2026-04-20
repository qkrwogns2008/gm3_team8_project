using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    [SerializeField] private BackgroundAudioSo _audioData;

    private void OnEnable()
    {// 1. 데이터 자체가 없는지 먼저 체크
        if (_audioData == null)
        {
            Debug.LogError($"{gameObject.name}에 Audio Data가 비어있습니다!");
            return;
        }

        // 2. 사운드 매니저가 준비되었는지 체크
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(_audioData.Background);
        }
        else
        {
            // 아직 준비 안 됐다면 코루틴으로 한 프레임 뒤에 다시 시도!
            StartCoroutine(CoPlayBGMDelayed());
        }
        // 활성화될 때마다 BGM 재생!

    }
    private IEnumerator CoPlayBGMDelayed()
    {
        // SoundManager가 Awake를 끝낼 때까지 한 프레임 대기
        yield return null;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(_audioData.Background);
        }
    }
}
