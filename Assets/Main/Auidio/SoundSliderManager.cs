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
}
