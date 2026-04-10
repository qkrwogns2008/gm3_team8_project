using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroList : MonoBehaviour
{
    public Transform content; // 영웅 생성 공간

    private List<HeroListData> heros = new List<HeroListData>(); // 영웅 리스트 데이터

    // 영웅 추가 (프리팹 기준)
    public void AddHero(GameObject heroPrefab)
    {
        // 프리팹에서 HeroData 가져오기
        Hero_Connect1 connect = heroPrefab.GetComponent<Hero_Connect1>();

        if (connect == null || connect.heroDataSO == null)
        {
            Debug.LogWarning("HeroDataSO가 연결되지 않은 프리팹입니다.");
            return;
        }

        HeroDataSO data = connect.heroDataSO;

        // 같은 아이템 찾기
        HeroListData existingHero = heros.Find(i => i.heroDataSO == data);

        if (existingHero != null)
        {
            // 이미 있으면 개수 증가
            existingHero.count++;
        }
        else
        {
            // 없으면 새로 추가
            HeroListData newHero = new HeroListData
            {
                heroDataSO = data,
                count = 1,
                prefab = heroPrefab
            };

            heros.Add(newHero);
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
        foreach (HeroListData hero in heros)
        {
            GameObject obj = Instantiate(hero.prefab, content);

            // TMP 텍스트 찾기
            TextMeshProUGUI countText = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (countText != null)
            {
                // 개수 표시 (1이면 숨김)
                if (hero.count > 1)
                {
                    countText.text = hero.count.ToString();
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
public class HeroListData
{
    public HeroDataSO heroDataSO;   // 히어로 식별용
    public int count;           // 개수
    public GameObject prefab;   // UI 생성용 프리팹
}