using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderManager : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _uiSfxSlider;
    [SerializeField] private Slider _unitSfxSlider;

    void OnEnable()
    {
        if(CDataManager.Instance == null)
        {
            return;
        }
        if (_bgmSlider != null)
        {
            _bgmSlider.value = CDataManager.Instance.UserData.BGMVolume;
        }
        if (_uiSfxSlider != null)
        {
            _uiSfxSlider.value = CDataManager.Instance.UserData.UIVolume;
        }
        if (_unitSfxSlider != null)
        {
            _unitSfxSlider.value = CDataManager.Instance.UserData.SFXVolume;
        }
    }

    public void SetBGMVolume(float volume)
    {
        CDataManager.Instance.UserData.BGMVolume = volume;
        PlayerPrefs.SetFloat("BGM_Volume", volume);

    }
    public void SFXSoundVolume(float volume)
    {
        CDataManager.Instance.UserData.SFXVolume = volume;
        PlayerPrefs.SetFloat("SoundEffect_Volume", volume);
    }
    public void UISFXSoundVolume(float volume)
    {
        CDataManager.Instance.UserData.UIVolume = volume;
        PlayerPrefs.SetFloat("SoundEffect_Volume", volume);
    }
}
