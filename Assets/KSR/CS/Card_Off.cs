using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Card_Off : MonoBehaviour
{
    // 외부에서 가져올 카드 개수 텍스트
    [SerializeField] private TextMeshProUGUI ownedText;

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

        // 에디터에서 값 변경 시 매니저에 반영
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

    void Update()
    {
        ApplyVisual();
    }

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

    // 외부 텍스트에서 개수 가져오기
    int GetOwnedCount()
    {
        if (ownedText == null) return 0;

        int count = 0;
        int.TryParse(ownedText.text, out count);
        return count;
    }

    // 소유 여부 확인
    public bool IsOwned()
    {
        return GetOwnedCount() > 0;
    }

    // CardManager 호환용 (기능 없음)
    public void Acquire()
    {
    }

    // 시각 상태 적용
    void ApplyVisual()
    {
        int ownedCount = GetOwnedCount();
        bool owned = ownedCount > 0;

        // 이미지 색상 처리
        if (images != null)
        {
            Color imgColor = owned ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            foreach (var img in images)
                img.color = imgColor;
        }

        // 숨김 텍스트 처리
        if (hideTexts != null)
        {
            foreach (var txt in hideTexts)
                if (txt != null)
                    txt.gameObject.SetActive(owned);
        }

        // 텍스트 색상 처리
        if (dimTexts != null)
        {
            Color textColor = owned ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
            foreach (var txt in dimTexts)
                if (txt != null)
                    txt.color = textColor;
        }
    }
}