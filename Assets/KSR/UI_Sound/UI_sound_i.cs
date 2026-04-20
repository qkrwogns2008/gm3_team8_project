using UnityEngine;
using UnityEngine.EventSystems;

public class UI_sound_i : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UIAudioSO _audioData;

    // =============================
    // 이미지 클릭 시 실행

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_audioData == null) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISFX(_audioData.uiOn); // UI 효과음 재생
        }
    }
}