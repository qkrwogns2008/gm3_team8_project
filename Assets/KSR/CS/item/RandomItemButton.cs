using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomItemButton : MonoBehaviour
{
    [Header("참조")]
    public Inventory inventory; // 인벤토리 참조

    [Header("아이템 풀 (프리팹 넣기)")]
    public List<GameObject> itemPrefabs = new List<GameObject>(); // 랜덤 대상

    [Header("버튼")]
    public Button button; // 버튼 컴포넌트

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnClick); // 클릭 이벤트 등록
        }
    }

    // =============================
    // 버튼 클릭 시 랜덤 아이템 획득

    void OnClick()
    {
        if (inventory == null) return; // 인벤토리 체크
        if (itemPrefabs.Count == 0) return; // 아이템 없으면 종료

        int randIndex = Random.Range(0, itemPrefabs.Count); // 랜덤 인덱스
        GameObject selectedItem = itemPrefabs[randIndex]; // 선택된 아이템

        inventory.AddItem(selectedItem); // 아이템 지급 (UI + 저장 동시 처리)
    }
}