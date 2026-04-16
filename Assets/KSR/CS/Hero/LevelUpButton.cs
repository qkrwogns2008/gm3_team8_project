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

    void Update()
    {
        UpdateUI();
    }

    // 버튼 OnClick에 연결
    public void OnClick()
    {
        if (heroDataSO == null) return;

        var userHeroData = CDataManager.Instance.GetHeroData(heroDataSO.HeroID);
        var userData = CDataManager.Instance.UserData;

        // 맥스 레벨이면 아무것도 하지 않음
        if (userHeroData.Level >= 50)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);
            return;
        }

        int requiredExp = userHeroData.Level * 10;

        // 경험치 부족
        if (userData.expPoint < requiredExp)
        {
            if (disableTarget != null)
                disableTarget.SetActive(false);
            return;
        }

        // 1레벨만 상승
        if (userData.expPoint >= requiredExp)
        {
            userData.expPoint -= requiredExp;
            // userHeroData.Level++;
            CDataManager.Instance.AddHeroLevel(heroDataSO.HeroID,1);
        }
    }

    // UI 갱신
    void UpdateUI()
    {
        if (heroDataSO == null || expCombinedText == null) return;

        var userHeroData = CDataManager.Instance.GetHeroData(heroDataSO.HeroID);
        var userData = CDataManager.Instance.UserData;

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
            return v.ToString("F0");
        else if (v >= 10)
            return v.ToString("F1");
        else
            return v.ToString("F2");
    }
}