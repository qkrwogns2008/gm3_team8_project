using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public Transform content; // 아이템 UI 생성 위치

    [Header("아이템 DB (ID → prefab 매핑)")]
    public ItemDatabase itemDatabase; // ScriptableObject 연결

    private List<InventoryItem> items = new List<InventoryItem>(); // 런타임 인벤토리

    // =============================
    // 시작 시 저장 데이터 기반 복원

    void Start()
    {
        LoadFromData(); // 저장 데이터 → UI 복원
    }

    // =============================
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

        // =============================
        // CDataManager 저장 (영구 데이터)

        if (CDataManager.Instance != null)
        {
            CDataManager.Instance.AddItem(data.itemID, 1);
        }

        // =============================
        // UI 리스트 처리

        InventoryItem existingItem = items.Find(i => i.itemData == data);

        if (existingItem != null)
        {
            existingItem.count++;
        }
        else
        {
            InventoryItem newItem = new InventoryItem
            {
                itemData = data,
                count = 1,
                prefab = itemPrefab
            };

            items.Add(newItem);
        }

        RefreshUI(); // UI 갱신
    }

    // =============================
    // 저장 데이터 기반 인벤토리 복원

    public void LoadFromData()
    {
        if (CDataManager.Instance == null) return;
        if (itemDatabase == null)
        {
            Debug.LogWarning("ItemDatabase가 연결되지 않았습니다.");
            return;
        }

        items.Clear(); // 기존 데이터 초기화

        var savedItems = CDataManager.Instance.UserData.Inventory;

        foreach (var saved in savedItems)
        {
            // ID → prefab 찾기
            GameObject prefab = itemDatabase.GetPrefab(saved.ItemID);

            if (prefab == null) continue;

            Item_Connect connect = prefab.GetComponent<Item_Connect>();

            if (connect == null || connect.itemData == null) continue;

            InventoryItem newItem = new InventoryItem
            {
                itemData = connect.itemData, // ItemData 연결
                count = saved.Quantity,      // 저장된 수량
                prefab = prefab              // 프리팹
            };

            items.Add(newItem);
        }

        RefreshUI(); // UI 갱신
    }

    // =============================
    // UI 전체 갱신

    void RefreshUI()
    {
        // 기존 UI 삭제
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // 새로 생성
        foreach (InventoryItem item in items)
        {
            GameObject obj = Instantiate(item.prefab, content);

            TextMeshProUGUI countText = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (countText != null)
            {
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

    // =============================
    // 인벤토리 초기화 (UI + 런타임 데이터)

    public void ClearInventory()
    {
        items.Clear();

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}

// =============================
// 인벤토리 내부 데이터 구조

[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;   // 아이템 데이터
    public int count;           // 수량
    public GameObject prefab;   // UI 프리팹
}