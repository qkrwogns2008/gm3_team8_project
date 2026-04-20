using UnityEngine;
using TMPro;

public class LevelUpButton : MonoBehaviour
{
    [Header("영웅 데이터")]
    [SerializeField] private HeroDataSO heroDataSO;

    [Header("비활성화할 오브젝트")]
    [SerializeField] private GameObject disableTarget;

    [Header("경험치 표시 텍스트 (보유 / 요구 또는 MAX)")]
    [SerializeField] private TextMeshProUGUI expCombinedText;

    [SerializeField] private UIAudioSO _audioData; // 오디오 데이터

    void Update()
    {
        UpdateUI();
    }

    // 버튼 OnClick에 연결
    public void OnClick()
    {
        // 데이터 없으면 실행 안함
        if (heroDataSO == null) return;

        var userHeroData = CDataManager.Instance.GetHeroData(heroDataSO.HeroID);
        var userData = CDataManager.Instance.UserData;

        // 영웅 미보유 → 동작 중단
        if (userHeroData.Quantity <= 0)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);
            return;
        }

        // 맥스 레벨 → 동작 중단
        if (userHeroData.Level >= 50)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);
            return;
        }

        int requiredExp = userHeroData.Level * 10;

        // 경험치 부족 → 동작 중단
        if (userData.expPoint < requiredExp)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);
            return;
        }

        // 1레벨 상승 처리
        if (userData.expPoint >= requiredExp)
        {
            userData.expPoint -= requiredExp;

            // 레벨 증가 (외부 시스템 사용)
            CDataManager.Instance.AddHeroLevel(heroDataSO.HeroID, 1);
        }
        // UI 효과음 재생
        Debug.Log("UI S1");
        if (_audioData == null) return;
        Debug.Log("UI S2");
        if (SoundManager.Instance != null)
        {
            Debug.Log("UI S3");
            SoundManager.Instance.PlayUISFX(_audioData.uiOn);
            Debug.Log("UI S4");
        }
        Debug.Log("UI S5");
    }

    // UI 갱신
    void UpdateUI()
    {
        // 데이터 없으면 실행 안함
        if (heroDataSO == null || expCombinedText == null) return;

        var userHeroData = CDataManager.Instance.GetHeroData(heroDataSO.HeroID);
        var userData = CDataManager.Instance.UserData;

        // 영웅 미보유 → 비활성화 표시
        if (userHeroData.Quantity <= 0)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);

            expCombinedText.text = "미보유";
            return;
        }

        // 맥스 레벨 표시
        if (userHeroData.Level >= 50)
        {
            expCombinedText.text = "MAX";

            if (disableTarget != null)
                disableTarget.SetActive(false);

            return;
        }

        int requiredExp = userHeroData.Level * 10;
        int currentExp = userData.expPoint;

        string cur = FormatNumber(currentExp);
        string req = FormatNumber(requiredExp);

        expCombinedText.text = cur + " / " + req;
    }

    // 숫자 포맷 (K, M / 최대 4자리)
    string FormatNumber(int value)
    {
        if (value >= 1000000)
        {
            float v = value / 1000000f;
            return Format4(v) + "M";
        }
        else if (value >= 1000)
        {
            float v = value / 1000f;
            return Format4(v) + "K";
        }
        else
        {
            return value.ToString();
        }
    }

    // 최대 4자리 표시
    string Format4(float v)
    {
        if (v >= 100)
            return v.ToString("F0");   // 123
        else if (v >= 10)
            return v.ToString("F1");   // 12.3
        else
            return v.ToString("F2");   // 1.23
    }
}