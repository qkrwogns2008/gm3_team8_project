using UnityEngine;

public class S_UP_Manger : MonoBehaviour
{
    public static S_UP_Manger Instance; // 싱글톤 인스턴스

    public S_UP_Data data = new S_UP_Data(); // 전체 저장 데이터

    void Awake()
    {
        if (Instance == null) // 인스턴스 없으면
            Instance = this; // 자기 자신 등록
        else
            Destroy(gameObject); // 중복 제거
    }

    // =============================
    // 타입별 데이터 가져오기

    public StatData GetData(StatType type)
    {
        switch (type) // 타입에 따라 반환
        {
            case StatType.Attack:
                return data.Attack;

            case StatType.Defense:
                return data.Defense;

            case StatType.HP:
                return data.HP;
        }

        return null; // 예외 처리
    }

    // =============================
    // 저장 (스테이지 포함)

    public void Save()
    {
        string json = JsonUtility.ToJson(data); // JSON 변환
        PlayerPrefs.SetString("S_UP_Data", json); // 저장
        PlayerPrefs.Save(); // 적용
    }

    // =============================
    // 로드

    public void Load()
    {
        if (!PlayerPrefs.HasKey("S_UP_Data")) return; // 저장 없으면 종료

        string json = PlayerPrefs.GetString("S_UP_Data"); // 데이터 불러오기
        data = JsonUtility.FromJson<S_UP_Data>(json); // 역직렬화
    }

    // =============================
    // 초기화 (테스트용)

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("S_UP_Data"); // 저장 삭제
        PlayerPrefs.Save(); // 적용

        data = new S_UP_Data(); // 데이터 초기화

        // 모든 업그레이드 버튼 초기화
        LV_UP_Button[] buttons = FindObjectsOfType<LV_UP_Button>();

        foreach (var btn in buttons)
        {
            if (btn == null) continue;

            btn.currentLevel = 1; // 레벨 초기화
            btn.currentStat = 0; // 누적 능력치 초기화
            btn.stageStat = 0; // 단계 능력치 초기화
            btn.currentCost = btn.baseCost; // 비용 초기화

            btn.UpdateUI(); // UI 갱신
        }

        // 스테이지 초기화 (단일 기준)
        Up_Manager manager = FindObjectOfType<Up_Manager>();
        if (manager != null)
        {
            manager.ResetStage(); // 스테이지 리셋
        }
    }
}

// =============================
// 타입 구분

public enum StatType
{
    Attack,  // 공격
    Defense, // 방어
    HP       // 체력
}