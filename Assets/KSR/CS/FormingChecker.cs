using System.Collections.Generic;
using UnityEngine;

public class FormingChecker : MonoBehaviour
{
    public static FormingChecker Instance;

    [System.Serializable]
    public class CardInfo
    {
        public GameObject prefabA;       // 카드 영웅 프리팹
        public GameObject targetObject;  // 편성완료 이미지
    }

    [SerializeField] private List<CardInfo> cards;  // 카드 목록

    private void Awake()
    {
        Instance = this;
        Debug.Log("[FormingChecker] Awake");
    }

    private void Start()
    {
        Debug.Log("[FormingChecker] Start - JSON 기반 체크");
        CheckForming();
    }

    // JSON 기반 편성 체크
    public void CheckForming()
    {
        Debug.Log("[FormingChecker] CheckForming 호출");

        if (CDataManager.Instance == null)
        {
            Debug.LogError("[FormingChecker] CDataManager 없음");
            return;
        }

        var data = CDataManager.Instance.UserData;

        if (data == null || data.Hero_Array == null)
        {
            Debug.LogError("[FormingChecker] Hero_Array 없음");
            return;
        }

        // JSON에서 배치된 heroID 수집
        HashSet<EHeroID> placedHeroes = new HashSet<EHeroID>();

        for (int i = 0; i < data.Hero_Array.Length; i++)
        {
            int id = data.Hero_Array[i];

            if (id == 0) continue;

            placedHeroes.Add((EHeroID)id);
            Debug.Log("[FormingChecker] JSON 영웅 발견: " + id);
        }

        // 카드 UI 갱신
        foreach (var card in cards)
        {
            if (card.prefabA == null)
            {
                Debug.LogWarning("[FormingChecker] prefabA 비어있음");
                continue;
            }

            if (card.targetObject == null)
            {
                Debug.LogWarning("[FormingChecker] targetObject 비어있음");
                continue;
            }

            Hero_Connect1 connect = card.prefabA.GetComponent<Hero_Connect1>();
            if (connect == null || connect.heroDataSO == null)
            {
                Debug.LogWarning("[FormingChecker] prefab에 Hero_Connect1 없음");
                continue;
            }

            EHeroID heroID = connect.heroDataSO.HeroID;

            bool shouldActive = placedHeroes.Contains(heroID);

            Debug.Log("[FormingChecker] 카드 " + heroID + " → " + (shouldActive ? "ON" : "OFF"));

            // 상태 변화 있을 때만 적용
            if (card.targetObject.activeSelf != shouldActive)
            {
                card.targetObject.SetActive(shouldActive);
            }
        }

        Debug.Log("[FormingChecker] CheckForming 완료");
    }
}