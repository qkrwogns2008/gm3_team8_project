using UnityEngine;

public class Info_Swap : MonoBehaviour
{
    [SerializeField] private GameObject objectA; // 끌 오브젝트
    [SerializeField] private GameObject objectB; // 켤 오브젝트

    [SerializeField] private UIAudioSO _audioData; // 오디오 데이터

    public void Swap()
    {
        if (objectA != null)
            objectA.SetActive(false);

        if (objectB != null)
            objectB.SetActive(true);

        // UI 효과음 재생
        if (_audioData == null) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISFX(_audioData.uiOn); // UI 효과음 재생
        }
    }
}