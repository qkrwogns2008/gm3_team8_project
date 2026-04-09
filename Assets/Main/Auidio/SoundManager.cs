using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("배경음악")]
    [SerializeField] private AudioSource _bgmSource;
    [Header("효과음 채널")]
    [SerializeField] private AudioSource _uiSfxSource;
    [SerializeField] private AudioSource _unitSfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 배경음 재생
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (_bgmSource == null) return;
        if (_bgmSource.clip == clip) return; // 같은 곡이면 무시

        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }
    // 배경음 멈투
    public void StopBGM(AudioClip clip, bool loop = false)
    {
        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }

    // UI 사운드
    public void PlayUISFX(AudioClip clip)
    {
        if (clip == null) return;
        _uiSfxSource.PlayOneShot(clip);
    }

    // 유닛 사운드
    public void PlayUnitSFX(AudioClip clip)
    {
        if (clip == null) return;
        _unitSfxSource.PlayOneShot(clip);
    }
}