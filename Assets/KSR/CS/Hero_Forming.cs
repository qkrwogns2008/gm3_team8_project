using UnityEngine;

public class Hero_Forming : MonoBehaviour
{
    [SerializeField] private Transform parentB;        // 전체 슬롯 부모 (0~15 슬롯)
    [SerializeField] private GameObject prefabA;       // 생성할 영웅 프리팹
    [SerializeField] private GameObject warningPrefab; // 최대 초과 시 경고 UI

    private GameObject spawnedObject; // 현재 이 카드로 생성된 영웅 오브젝트
    private bool isPlaced = false;    // 편성 상태 여부

    public bool IsPlaced()
    {
        return isPlaced; // 외부에서 편성 여부 확인
    }

    // 시작 시 필드 상태 기준으로 이 카드가 이미 배치되어 있는지 확인
    void Start()
    {
        if (CDataManager.Instance == null) return;

        // 프리팹에서 heroID 가져오기
        Hero_Connect1 prefabConnect = prefabA.GetComponent<Hero_Connect1>();
        if (prefabConnect == null || prefabConnect.heroDataSO == null) return;

        EHeroID heroID = prefabConnect.heroDataSO.HeroID;

        // 필드 전체 슬롯 검사
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            // 슬롯이 비어있으면 패스
            if (slot.childCount == 0) continue;

            Transform child = slot.GetChild(0);

            // 슬롯에 있는 오브젝트의 heroID 확인
            Hero_Connect1 connect = child.GetComponent<Hero_Connect1>();
            if (connect == null || connect.heroDataSO == null) continue;

            // 동일한 heroID 발견 시 이미 배치된 상태로 판단
            if (connect.heroDataSO.HeroID == heroID)
            {
                spawnedObject = child.gameObject;
                isPlaced = true;
                return;
            }
        }

        // 없으면 미배치 상태
        isPlaced = false;
    }

    public void Spawn()
    {
        if (parentB == null || prefabA == null) return;

        // 프리팹에서 heroID 가져오기
        Hero_Connect1 prefabConnect = prefabA.GetComponent<Hero_Connect1>();
        if (prefabConnect == null || prefabConnect.heroDataSO == null) return;

        EHeroID heroID = prefabConnect.heroDataSO.HeroID;

        // 이미 배치된 경우 → 필드에서 제거
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            if (slot.childCount == 0) continue;

            Transform child = slot.GetChild(0);

            Hero_Connect1 connect = child.GetComponent<Hero_Connect1>();
            if (connect == null || connect.heroDataSO == null) continue;

            // heroID 기준으로 정확하게 찾아서 제거
            if (connect.heroDataSO.HeroID == heroID)
            {
                // 필드에서 즉시 제외 처리
                child.gameObject.SetActive(false);

                spawnedObject = null;
                isPlaced = false;

                // 현재 필드 상태를 기준으로 JSON 전체 갱신
                UpdateJSON();

                // 오브젝트 제거
                Destroy(child.gameObject);

                // UI 갱신
                if (FormingChecker.Instance != null)
                    FormingChecker.Instance.CheckForming();

                return;
            }
        }

        // 현재 배치된 유닛 수 계산
        int currentCount = 0;
        for (int i = 0; i < parentB.childCount; i++)
        {
            if (parentB.GetChild(i).childCount > 0)
                currentCount++;
        }

        // 최대 5개 제한
        if (currentCount >= 5)
        {
            if (warningPrefab != null)
                Instantiate(warningPrefab, parentB);
            return;
        }

        // 빈 슬롯 찾기
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            if (slot.childCount == 0)
            {
                // 영웅 생성
                spawnedObject = Instantiate(prefabA, slot);

                isPlaced = true;

                // 현재 필드 상태를 기준으로 JSON 전체 갱신
                UpdateJSON();

                // UI 갱신
                if (FormingChecker.Instance != null)
                    FormingChecker.Instance.CheckForming();

                break;
            }
        }
    }

    // 현재 필드 상태를 기준으로 JSON 데이터 초기화 후 재기록
    private void UpdateJSON()
    {
        if (CDataManager.Instance == null) return;

        // 전체 초기화
        for (int i = 0; i < 16; i++)
        {
            int x = i % 4;
            int y = i / 4;
            CDataManager.Instance.AddUserHeroArray(x, y, (EHeroID)0);
        }

        // 슬롯 기준으로 재기록
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            if (slot.childCount == 0) continue;

            Transform child = slot.GetChild(0);

            // 비활성화된 오브젝트는 제외
            if (!child.gameObject.activeSelf) continue;

            Hero_Connect1 connect = child.GetComponent<Hero_Connect1>();
            if (connect == null || connect.heroDataSO == null) continue;

            int x = i % 4;
            int y = i / 4;


            CDataManager.Instance.UserData.IsHeroArrayChanged = true; // 변경 플래그 설정
            CDataManager.Instance.AddUserHeroArray(x, y, connect.heroDataSO.HeroID);
        }
    }
}