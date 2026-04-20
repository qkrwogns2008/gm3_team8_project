using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LV_UP_Button : MonoBehaviour
{
    [Header("매니저 참조")]
    public Up_Manager upManager; // 스테이지 참조용 매니저

    [Header("기본 상태")]
    public int currentLevel = 1; // 현재 레벨

    [Header("능력치")]
    public float currentStat = 0;   // 전체 누적값
    public float stageStat = 0;     // 현재 단계 표시용

    [Header("단계별 레벨당 능력치")]
    public List<float> statPerLevelByStage = new List<float>() { 10f, 20f, 30f };

    [Header("골드 설정")]
    public float baseCost = 10f;
    public float costIncreaseValue = 2f;

    public enum CostType { Add, Multiply }
    public CostType costType = CostType.Add;

    [Header("현재 비용")]
    public float currentCost;

    [Header("레벨 증가 배수")]
    public int levelStep = 1;

    [Header("단계별 최대 레벨")]
    public List<int> stageMaxLevels = new List<int>() { 200, 400, 600 };

    [Header("TMP UI")]
    public TMP_Text goldText;
    public TMP_Text levelText;
    public TMP_Text statText;
    public TMP_Text costText;

    [Header("단계 표시 UI")]
    public TMP_Text stageTextA;
    public TMP_Text stageTextB;

    [Header("MAX UI 설정")]
    public GameObject maxImage;

    [Header("버튼")]
    public Button levelUpButton;

    [Header("이미지 교체")]
    public GameObject originalImage;
    public GameObject changedImage;

    [Header("능력치 타입")]
    public StatType statType; // Attack / Defense / HP 구분

    void Start()
    {
        currentCost = baseCost;

        if (S_UP_Manger.Instance != null)
        {
            S_UP_Manger.Instance.Load();

            StatData data = S_UP_Manger.Instance.GetData(statType);

            if (data != null)
            {
                currentLevel = data.currentLevel;
                currentStat = data.currentStat;
                stageStat = data.stageStat;
                currentCost = data.currentCost;
            }
        }

        UpdateUI();
    }

    int GetCurrentStage()
    {
        if (upManager == null) return 0;
        return upManager.GetCurrentStage();
    }

    float GetStatPerLevel()
    {
        int stage = GetCurrentStage();

        if (stage < statPerLevelByStage.Count)
            return statPerLevelByStage[stage];

        return statPerLevelByStage[statPerLevelByStage.Count - 1];
    }

    float GetPreviewCost(out float afterCost, out int levelGained)
    {
        int maxLevel = GetMaxLevel();

        int targetLevel = currentLevel + levelStep;
        if (targetLevel > maxLevel)
            targetLevel = maxLevel;

        levelGained = targetLevel - currentLevel;

        float total = 0f;
        float tempCost = currentCost;

        for (int i = 0; i < levelGained; i++)
        {
            total += tempCost;

            if (costType == CostType.Add)
                tempCost += costIncreaseValue;
            else
                tempCost *= costIncreaseValue;
        }

        afterCost = tempCost;
        return total;
    }

    // =============================
    // 레벨업

    public void TryLevelUp()
    {
        if (CDataManager.Instance == null) return;

        float afterCost;
        int levelGained;

        float totalCost = GetPreviewCost(out afterCost, out levelGained);

        int currentGold = CDataManager.Instance.UserData.Gold;

        if (currentGold < totalCost)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 골드 차감 (데이터 매니저 사용)
        CDataManager.Instance.SpendGold((int)totalCost);

        currentLevel += levelGained;

        float statValue = GetStatPerLevel();
        float gainedStat = statValue * levelGained;

        stageStat += gainedStat;
        currentStat += gainedStat;

        currentCost = afterCost;

        UpdateUI();

        FindObjectOfType<Up_Manager>()?.OnLevelChanged();

        // =============================
        // 능력치 저장

        if (CDataManager.Instance != null)
        {
            switch (statType)
            {
                case StatType.Attack:
                    CDataManager.Instance.UserData.Atk_Level = (int)currentStat;
                    break;

                case StatType.Defense:
                    CDataManager.Instance.UserData.Def_Level = (int)currentStat;
                    break;

                case StatType.HP:
                    CDataManager.Instance.UserData.Life_Level = (int)currentStat;
                    break;
            }

            CDataManager.Instance.SaveUserData();
        }

        // =============================

        if (S_UP_Manger.Instance != null)
        {
            StatData data = S_UP_Manger.Instance.GetData(statType);

            if (data != null)
            {
                data.currentLevel = currentLevel;
                data.currentStat = currentStat;
                data.stageStat = stageStat;
                data.currentCost = currentCost;
            }

            S_UP_Manger.Instance.Save();
        }
    }

    public void UpdateUI()
    {
        if (CDataManager.Instance == null) return;

        float currentGold = CDataManager.Instance.UserData.Gold;

        if (goldText != null)
            goldText.text = FormatGold(currentGold);

        if (levelText != null)
            levelText.text = $"Lv.{currentLevel}";

        if (statText != null)
            statText.text = $"+{stageStat:F0}";

        float afterCost;
        int levelGained;
        float previewCost = GetPreviewCost(out afterCost, out levelGained);

        if (costText != null)
        {
            costText.text = FormatGold(previewCost);

            if (currentGold < previewCost)
                costText.color = Color.red;
            else
                costText.color = Color.white;
        }

        int stage = GetCurrentStage();
        int displayStage = stage + 1;

        if (stageTextA != null)
            stageTextA.text = $"영향력 {displayStage}단계";

        if (stageTextB != null)
            stageTextB.text = $"{displayStage}/10";

        int maxLevel = GetMaxLevel();
        bool isMax = currentLevel >= maxLevel;

        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(!isMax);

        if (maxImage != null)
            maxImage.SetActive(isMax);

        if (originalImage != null)
            originalImage.SetActive(!isMax);

        if (changedImage != null)
            changedImage.SetActive(isMax);
    }

    public void ResetLevel()
    {
        currentLevel = 1;
        stageStat = 0;

        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(true);

        if (maxImage != null)
            maxImage.SetActive(false);

        if (originalImage != null)
            originalImage.SetActive(true);

        if (changedImage != null)
            changedImage.SetActive(false);

        UpdateUI();
    }

    string FormatGold(float value)
    {
        if (value >= 1000f)
        {
            float v = value / 1000f;

            if (v < 10f)
                return v.ToString("F2") + "k";
            else if (v < 100f)
                return v.ToString("F1") + "k";
            else
                return v.ToString("F0") + "k";
        }
        else
        {
            return value.ToString("F0");
        }
    }

    int GetMaxLevel()
    {
        int stage = GetCurrentStage();

        if (stage < stageMaxLevels.Count)
            return stageMaxLevels[stage];

        return stageMaxLevels[stageMaxLevels.Count - 1];
    }
}