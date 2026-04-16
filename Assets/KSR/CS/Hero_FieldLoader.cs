using System.Collections;
using UnityEngine;

public class Hero_FieldLoader : MonoBehaviour
{
    [SerializeField] private Transform fieldParent;
    [SerializeField] private GameObject[] heroPrefabs;

    private bool hasLoaded = false; // 중복 실행 방지

    void Update()
    {
        // UI가 활성화된 순간 1번만 실행
        if (!hasLoaded && gameObject.activeInHierarchy)
        {
            hasLoaded = true;
            StartCoroutine(DelayedLoad());
        }
    }

    IEnumerator DelayedLoad()
    {
        // UI 완전히 안정화될 때까지 1프레임 대기
        yield return null;
        LoadAndSpawn();
    }

    public void LoadAndSpawn()
    {
        if (fieldParent == null || CDataManager.Instance == null) return;

        CDataManager.Instance.LoadUserData();
        var data = CDataManager.Instance.UserData;

        for (int i = 0; i < fieldParent.childCount; i++)
        {
            Transform slot = fieldParent.GetChild(i);

            // 기존 오브젝트 제거
            for (int j = slot.childCount - 1; j >= 0; j--)
            {
                Destroy(slot.GetChild(j).gameObject);
            }

            int heroID = data.Hero_Array[i];
            if (heroID == 0) continue;

            GameObject prefab = GetPrefabByID((EHeroID)heroID);
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(slot, false);

            Hero_Connect1 connect = obj.GetComponent<Hero_Connect1>();
            if (connect != null)
            {
                connect.heroDataSO = GetHeroSO((EHeroID)heroID);
            }
        }
    }

    GameObject GetPrefabByID(EHeroID id)
    {
        foreach (var prefab in heroPrefabs)
        {
            Hero_Connect1 connect = prefab.GetComponent<Hero_Connect1>();
            if (connect != null && connect.heroDataSO != null)
            {
                if (connect.heroDataSO.HeroID == id)
                    return prefab;
            }
        }
        return null;
    }

    HeroDataSO GetHeroSO(EHeroID id)
    {
        foreach (var prefab in heroPrefabs)
        {
            Hero_Connect1 connect = prefab.GetComponent<Hero_Connect1>();
            if (connect != null && connect.heroDataSO != null)
            {
                if (connect.heroDataSO.HeroID == id)
                    return connect.heroDataSO;
            }
        }
        return null;
    }
}