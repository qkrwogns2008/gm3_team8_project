using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Card_Off : MonoBehaviour
{
    // 카드 소유 여부
    [SerializeField] private bool isOwned = false;

    // 이미지 전체 어둡게 처리할 루트
    [SerializeField] private GameObject imageRoot;

    // 미소유 시 숨길 텍스트 루트
    [SerializeField] private GameObject hideTextRoot;

    // 미소유 시 어둡게 할 텍스트 루트
    [SerializeField] private GameObject dimTextRoot;

    // CardManager 참조
    [SerializeField] private CardManager cardManager;

    // 내부 캐싱
    private Image[] images;
    private TMP_Text[] hideTexts;
    private TMP_Text[] dimTexts;


    void Awake()
    {
        Init();
        ApplyVisual();
    }

    void OnValidate()
    {
        Init();
        ApplyVisual();

        // 에디터에서 isOwned 변경 시 매니저에 반영 (지연 실행)
        if (cardManager != null)
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += DelayedRefresh;
#endif
        }
    }

#if UNITY_EDITOR
    void DelayedRefresh()
    {
        if (this == null) return;
        if (cardManager != null)
        {
            cardManager.RefreshOrder();
        }
    }
#endif

    // 각 루트에서 컴포넌트 수집
    void Init()
    {
        if (imageRoot != null)
            images = imageRoot.GetComponentsInChildren<Image>(true);

        if (hideTextRoot != null)
            hideTexts = hideTextRoot.GetComponentsInChildren<TMP_Text>(true);

        if (dimTextRoot != null)
            dimTexts = dimTextRoot.GetComponentsInChildren<TMP_Text>(true);
    }

    // 소유 여부 설정
    public void SetOwned(bool value)
    {
        isOwned = value;
        ApplyVisual();

        // 상태 변경 시 매니저에 반영
        if (cardManager != null)
        {
            cardManager.RefreshOrder();
        }
    }

    // 카드 획득 처리
    public void Acquire()
    {
        isOwned = true;
        ApplyVisual();

        // 상태 변경 시 매니저에 반영
        if (cardManager != null)
        {
            cardManager.RefreshOrder();
        }
    }

    // 외부에서 소유 여부 확인
    public bool IsOwned()
    {
        return isOwned;
    }

    // 시각 상태 적용
    void ApplyVisual()
    {
        // 이미지 색상 처리
        if (images != null)
        {
            Color imgColor = isOwned ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            foreach (var img in images)
                img.color = imgColor;
        }

        // 숨김 텍스트 처리
        if (hideTexts != null)
        {
            foreach (var txt in hideTexts)
                if (txt != null)
                    txt.gameObject.SetActive(isOwned);
        }

        // 텍스트 색상 처리
        if (dimTexts != null)
        {
            Color textColor = isOwned ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
            foreach (var txt in dimTexts)
                if (txt != null)
                    txt.color = textColor;
        }
    }
}