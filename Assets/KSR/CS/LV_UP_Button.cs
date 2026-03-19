using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LV_UP_Button : MonoBehaviour
{
    [Header("기본 상태")]
    public int currentLevel = 1;
    public int currentStage = 0;
    public float currentGold = 1000;

    [Header("능력치")]
    public float currentStat = 0;
    public float statPerLevel = 10f;

    [Header("골드 설정")]
    public float baseCost = 10f;
    public float costIncreaseValue = 2f;

    public enum CostType { Add, Multiply }
    public CostType costType = CostType.Add;

    [Header("레벨 증가 배수")]
    public int levelStep = 1; // 1, 10, 100

    [Header("단계별 최대 레벨")]
    public List<int> stageMaxLevels = new List<int>() { 200, 400, 600 };

    [Header("TMP UI")]
    public TMP_Text goldText;
    public TMP_Text levelText;
    public TMP_Text statText;
    public TMP_Text costText;

    // =============================

    void Start()
    {
        UpdateUI();
    }

    public void TryLevelUp()
    {
        int maxLevel = GetMaxLevel();

        int targetLevel = currentLevel + levelStep;
        if (targetLevel > maxLevel)
            targetLevel = maxLevel;

        float totalCost = CalculateTotalCost(currentLevel, targetLevel);

        if (currentGold < totalCost)
        {
            Debug.Log("골드 부족");
            return;
        }

        int levelGained = targetLevel - currentLevel;
        float statGained = statPerLevel * levelGained;

        currentGold -= totalCost;
        currentLevel = targetLevel;
        currentStat += statGained;

        if (currentLevel >= maxLevel)
        {
            LevelUpStage();
        }

        UpdateUI();
    }

    // =============================

    void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"{currentGold:F0}";

        if (levelText != null)
            levelText.text = $"Lv.{currentLevel}";

        if (statText != null)
            statText.text = $"+{currentStat:F0}";

        float nextCost = CalculateTotalCost(
            currentLevel,
            Mathf.Min(currentLevel + levelStep, GetMaxLevel())
        );

        if (costText != null)
            costText.text = $"{nextCost:F0}";
    }

    // =============================

    float CalculateTotalCost(int startLevel, int endLevel)
    {
        float total = 0;
        float cost = GetCostAtLevel(startLevel);

        for (int i = startLevel; i < endLevel; i++)
        {
            total += cost;

            if (costType == CostType.Add)
                cost += costIncreaseValue;
            else
                cost *= costIncreaseValue;
        }

        return total;
    }

    float GetCostAtLevel(int level)
    {
        float cost = baseCost;

        for (int i = 1; i < level; i++)
        {
            if (costType == CostType.Add)
                cost += costIncreaseValue;
            else
                cost *= costIncreaseValue;
        }

        return cost;
    }

    int GetMaxLevel()
    {
        if (currentStage < stageMaxLevels.Count)
            return stageMaxLevels[currentStage];

        return stageMaxLevels[stageMaxLevels.Count - 1];
    }

    void LevelUpStage()
    {
        currentStage++;
        currentLevel = 1;

        Debug.Log($"단계 상승! 현재 단계: {currentStage}");
    }
}