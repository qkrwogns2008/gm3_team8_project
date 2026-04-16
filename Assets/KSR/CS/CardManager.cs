using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardManager : MonoBehaviour
{
    // 카드 리스트 (순서 기준)
    public List<Card_Off> cards;

    // 카드 개수 표시 텍스트
    [SerializeField] private TMP_Text countText;

    void Start()
    {
        // 시작 시 정렬 및 텍스트 갱신
        RefreshOrder();
        UpdateCountText();
    }

    void Update()
    {
        // 외부 상태 변화 반영을 위해 매 프레임 갱신
        RefreshOrder();
        UpdateCountText();
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += DelayedRefresh;
#endif
    }

#if UNITY_EDITOR
    void DelayedRefresh()
    {
        if (this == null) return;

        RefreshOrder();
        UpdateCountText();
    }
#endif

    // 카드 순서를 편성 → 소유 → 나머지 기준으로 재정렬
    public void RefreshOrder()
    {
        int index = 0;

        // 1. 편성된 카드
        foreach (var card in cards)
        {
            if (card != null && IsCardPlaced(card))
            {
                card.transform.SetSiblingIndex(index);
                index++;
            }
        }

        // 2. 소유 카드 (편성되지 않은)
        foreach (var card in cards)
        {
            if (card != null && !IsCardPlaced(card) && card.IsOwned())
            {
                card.transform.SetSiblingIndex(index);
                index++;
            }
        }

        // 3. 미소유 카드
        foreach (var card in cards)
        {
            if (card != null && !IsCardPlaced(card) && !card.IsOwned())
            {
                card.transform.SetSiblingIndex(index);
                index++;
            }
        }
    }

    // JSON 기준으로 카드가 편성되어 있는지 확인
    bool IsCardPlaced(Card_Off card)
    {
        if (CDataManager.Instance == null) return false;

        Hero_Connect1 connect = card.GetComponent<Hero_Connect1>();
        if (connect == null || connect.heroDataSO == null) return false;

        int[] heroArray = CDataManager.Instance.UserData.Hero_Array;

        for (int i = 0; i < heroArray.Length; i++)
        {
            if (heroArray[i] == (int)connect.heroDataSO.HeroID)
                return true;
        }

        return false;
    }

    // 카드 획득 처리 후 정렬 갱신
    public void OnCardAcquired(Card_Off card)
    {
        card.Acquire(); // 호환용
        RefreshOrder();
        UpdateCountText();
    }

    // 소유 카드만 표시
    public void ShowOwnedOnly()
    {
        foreach (var card in cards)
        {
            if (card != null)
                card.gameObject.SetActive(card.IsOwned());
        }
    }

    // 전체 카드 표시
    public void ShowAll()
    {
        foreach (var card in cards)
        {
            if (card != null)
                card.gameObject.SetActive(true);
        }
    }

    // 카드 종류 개수 텍스트 갱신
    void UpdateCountText()
    {
        int ownedTypeCount = 0;

        // "종류" 기준: 1장 이상이면 카운트
        foreach (var card in cards)
        {
            if (card != null && card.IsOwned())
                ownedTypeCount++;
        }

        int totalCount = cards.Count;

        if (countText != null)
            countText.text = ownedTypeCount + "/" + totalCount;
    }
}