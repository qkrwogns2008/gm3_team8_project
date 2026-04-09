using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    [SerializeField] private BackgroundAudioSo _audioData;
    void Start()
    {
        // 전부 통과하면 재생!
        SoundManager.Instance.PlayBGM(_audioData.Background);
    }
}
