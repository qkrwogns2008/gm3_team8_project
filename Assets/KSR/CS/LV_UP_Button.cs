using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LV_UP_Button : MonoBehaviour
{
    [Header("플레이어 데이터")]
    public Dummy_Player DummyPlayer;

    [Header("기본 상태")]
    public int currentLevel = 1;
    public int currentStage = 0;

    [Header("능력치")]
    public float currentStat = 0;
    public float statPerLevel = 10f;

    [Header("골드 설정")]
    public float baseCost = 10f;
    public float costIncreaseValue = 2f;

    public enum CostType { Add, Multiply }
    public CostType costType = CostType.Add;

    [Header("현재 비용 (핵심)")]
    public float currentCost; // 다음 1레벨 비용 (누적 유지)

    [Header("레벨 증가 배수")]
    public int levelStep = 1;

    [Header("단계별 최대 레벨")]
    public List<int> stageMaxLevels = new List<int>() { 200, 400, 600 };

    [Header("TMP UI")]
    public TMP_Text goldText;
    public TMP_Text levelText;
    public TMP_Text statText;
    public TMP_Text costText;

    [Header("MAX UI 설정")]
    public GameObject maxImage;

    [Header("버튼")]
    public Button levelUpButton;

    [Header("이미지 교체")]
    public GameObject originalImage;
    public GameObject changedImage;

    // =============================

    void Start()
    {
        currentCost = baseCost; // 시작 비용
        UpdateUI();
    }

    // =============================
    // 현재 클릭 시 필요한 총 비용 계산 (UI & 실제 결제용)

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

        afterCost = tempCost; // 레벨업 후 비용

        return total;
    }

    // =============================
    // 레벨업

    public void TryLevelUp()
    {
        if (DummyPlayer == null) return;

        float afterCost;
        int levelGained;

        float totalCost = GetPreviewCost(out afterCost, out levelGained);

        if (DummyPlayer.gold < totalCost)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 골드 차감
        DummyPlayer.gold -= totalCost;

        // 레벨 증가
        currentLevel += levelGained;

        // 능력치 증가
        currentStat += statPerLevel * levelGained;

        // 비용 갱신 (누적 유지)
        currentCost = afterCost;

        UpdateUI();

        FindObjectOfType<Up_Manager>()?.OnLevelChanged();
    }

    // =============================
    // UI 갱신

    public void UpdateUI()
    {
        if (DummyPlayer == null) return;

        float currentGold = DummyPlayer.gold;

        if (goldText != null)
            goldText.text = FormatGold(currentGold);

        if (levelText != null)
            levelText.text = $"Lv.{currentLevel}";

        if (statText != null)
            statText.text = $"+{currentStat:F0}";

        // 현재 클릭 시 총 비용 가져오기
        float afterCost;
        int levelGained;
        float previewCost = GetPreviewCost(out afterCost, out levelGained);

        if (costText != null)
        {
            costText.text = FormatGold(previewCost); // 🔥 총 비용 표시

            // 총 비용 기준으로 빨간색
            if (currentGold < previewCost)
                costText.color = Color.red;
            else
                costText.color = Color.white;
        }

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

    // =============================
    // 스테이지 상승 시

    public void ResetLevel()
    {
        currentLevel = 1;

        // 🔥 currentCost 유지 (핵심)

        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(true);

        if (maxImage != null)
            maxImage.SetActive(false);

        if (originalImage != null)
            originalImage.SetActive(true);

        if (changedImage != null)
            changedImage.SetActive(false);

        UpdateUI();

        FindObjectOfType<Up_Manager>()?.OnLevelChanged();
    }

    // =============================

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

    // =============================

    int GetMaxLevel()
    {
        if (currentStage < stageMaxLevels.Count)
            return stageMaxLevels[currentStage];

        return stageMaxLevels[stageMaxLevels.Count - 1];
    }
}