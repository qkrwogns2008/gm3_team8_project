using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 슬롯 데이터
[Serializable]
public class FormingSlotData
{
    public EHeroID heroID;   // 영웅 ID
    public int slotIndex;    // 슬롯 위치
}

public class Forming_Data : MonoBehaviour
{
    [Header("배치 데이터")]
    public List<FormingSlotData> slots = new List<FormingSlotData>();

    [Header("슬롯 부모")]
    [SerializeField] private Transform parentB; // 슬롯 부모

    [Header("ID 프리팹 목록")]
    [SerializeField] private List<GameObject> idPrefabs; // ID 프리팹

    [Header("카드 리스트")]
    [SerializeField] private List<GameObject> cards; // 카드 리스트

    void Start()
    {
        Apply();                     // 슬롯 생성
        StartCoroutine(DelayedSync()); // 한 프레임 뒤 카드 상태 반영
    }

    // 데이터 기반으로 슬롯 생성
    void Apply()
    {
        if (parentB == null) return;

        // 슬롯 초기화
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            for (int j = slot.childCount - 1; j >= 0; j--)
            {
                Destroy(slot.GetChild(j).gameObject);
            }
        }

        // 데이터 기준 생성
        foreach (var data in slots)
        {
            if (data.slotIndex < 0 || data.slotIndex >= parentB.childCount)
                continue;

            Transform slot = parentB.GetChild(data.slotIndex);

            GameObject prefab = GetPrefab(data.heroID);
            if (prefab == null) continue;

            Instantiate(prefab, slot);
        }
    }

    // 한 프레임 뒤 카드 상태 반영
    IEnumerator DelayedSync()
    {
        yield return null;
        SyncCardState();
    }

    // 슬롯 기준으로 카드 상태 설정
    void SyncCardState()
    {
        // 모든 카드 OFF
        foreach (var card in cards)
        {
            if (card == null) continue;

            Hero_Forming forming = card.GetComponent<Hero_Forming>();
            if (forming == null) continue;

            SetCardState(forming, false);
        }

        // 배치된 카드만 ON
        foreach (var data in slots)
        {
            foreach (var card in cards)
            {
                if (card == null) continue;

                Hero_Connect1 connect = card.GetComponent<Hero_Connect1>();
                if (connect == null || connect.heroDataSO == null) continue;

                if (connect.heroDataSO.HeroID == data.heroID)
                {
                    Hero_Forming forming = card.GetComponent<Hero_Forming>();
                    if (forming == null) continue;

                    SetCardState(forming, true);
                    break;
                }
            }
        }
    }

    // 카드 상태 설정
    void SetCardState(Hero_Forming forming, bool state)
    {
        var placedField = typeof(Hero_Forming).GetField("isPlaced",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (placedField != null)
        {
            placedField.SetValue(forming, state);
        }

        var targetField = typeof(Hero_Forming).GetField("targetObject",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (targetField != null)
        {
            GameObject obj = targetField.GetValue(forming) as GameObject;
            if (obj != null)
                obj.SetActive(state);
        }
    }

    // HeroID에 맞는 프리팹 찾기
    GameObject GetPrefab(EHeroID id)
    {
        foreach (var prefab in idPrefabs)
        {
            if (prefab == null) continue;

            Hero_Connect1 connect = prefab.GetComponent<Hero_Connect1>();

            if (connect != null && connect.heroDataSO != null)
            {
                if (connect.heroDataSO.HeroID == id)
                    return prefab;
            }
        }

        return null;
    }
}