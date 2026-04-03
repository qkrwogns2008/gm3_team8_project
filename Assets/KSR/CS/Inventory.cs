using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public Transform content; // 아이템이 생성 공간

    private List<InventoryItem> items = new List<InventoryItem>(); // 인벤토리 데이터

    // 아이템 추가 (프리팹 기준)
    public void AddItem(GameObject itemPrefab)
    {
        // 프리팹에서 ItemData 가져오기
        Item_Connect connect = itemPrefab.GetComponent<Item_Connect>();

        if (connect == null || connect.itemData == null)
        {
            Debug.LogWarning("ItemData가 연결되지 않은 프리팹입니다.");
            return;
        }

        ItemData data = connect.itemData;

        // 같은 아이템 찾기
        InventoryItem existingItem = items.Find(i => i.itemData == data);

        if (existingItem != null)
        {
            // 이미 있으면 개수 증가
            existingItem.count++;
        }
        else
        {
            // 없으면 새로 추가
            InventoryItem newItem = new InventoryItem
            {
                itemData = data,
                count = 1,
                prefab = itemPrefab
            };

            items.Add(newItem);
        }

        // UI 갱신
        RefreshUI();
    }

    // UI 전체 갱신
    void RefreshUI()
    {
        // 기존 UI 삭제
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // 리스트 기준으로 다시 생성
        foreach (InventoryItem item in items)
        {
            GameObject obj = Instantiate(item.prefab, content);

            // TMP 텍스트 찾기
            TextMeshProUGUI countText = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (countText != null)
            {
                // 개수 표시 (1이면 숨김)
                if (item.count > 1)
                {
                    countText.text = item.count.ToString();
                    countText.gameObject.SetActive(true);
                }
                else
                {
                    countText.gameObject.SetActive(false);
                }
            }
        }
    }
}

// 인벤토리 내부 데이터 구조
[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;   // 아이템 식별용
    public int count;           // 개수
    public GameObject prefab;   // UI 생성용 프리팹
}