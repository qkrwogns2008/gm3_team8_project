using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    // 카드 리스트 (순서 기준)
    public List<Card_Off> cards;

    // 카드 개수 표시 텍스트
    [SerializeField] private TMP_Text countText;

    void Start()
    {
        // 시작 시 정렬 적용
        RefreshOrder();

        // 시작 시 텍스트 갱신
        UpdateCountText();
    }

    void OnValidate()
    {
        // 에디터에서 값 변경 시에도 즉시 정렬 및 텍스트 갱신
        RefreshOrder();
        UpdateCountText();
    }

    // 카드 순서를 소유 여부 기준으로 재정렬
    public void RefreshOrder()
    {
        int index = 0;

        // 소유 카드 먼저 배치
        foreach (var card in cards)
        {
            if (card != null && card.IsOwned())
            {
                card.transform.SetSiblingIndex(index);
                index++;
            }
        }

        // 미소유 카드 뒤에 배치
        foreach (var card in cards)
        {
            if (card != null && !card.IsOwned())
            {
                card.transform.SetSiblingIndex(index);
                index++;
            }
        }
    }

    // 카드 획득 처리 후 정렬 갱신
    public void OnCardAcquired(Card_Off card)
    {
        card.Acquire();
        RefreshOrder();

        // 텍스트 갱신
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

    // 카드 개수 텍스트 갱신
    void UpdateCountText()
    {
        int ownedCount = 0;

        foreach (var card in cards)
        {
            if (card != null && card.IsOwned())
                ownedCount++;
        }

        int totalCount = cards.Count;

        if (countText != null)
            countText.text = ownedCount + "/" + totalCount;
    }
}