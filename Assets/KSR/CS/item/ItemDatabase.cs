using System.Collections.Generic;
using UnityEngine;

// =============================
// 아이템 ID ↔ 프리팹 매핑 데이터베이스

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [Header("아이템 목록")]
    public List<ItemEntry> items = new List<ItemEntry>(); // 전체 매핑 리스트

    // =============================
    // ID로 프리팹 찾기

    public GameObject GetPrefab(int itemID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemID == itemID)
                return items[i].prefab; // 일치하는 프리팹 반환
        }

        return null; // 없으면 null
    }
}

// =============================
// 개별 매핑 데이터

[System.Serializable]
public class ItemEntry
{
    public int itemID;           // 아이템 ID
    public GameObject prefab;    // 해당 프리팹
}