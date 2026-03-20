using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LV_UP_Button : MonoBehaviour
{
    [Header("플레이어 데이터")]
    public Dummy_Player DummyPlayer; // 플레이어 골드 참조

    [Header("기본 상태")]
    public int currentLevel = 1;     // 현재 레벨
    public int currentStage = 0;     // 현재 단계 (영향력 단계)

    [Header("능력치")]
    public float currentStat = 0;    // 누적 능력치 (리셋 안됨)
    public float statPerLevel = 10f; // 레벨당 증가량

    [Header("골드 설정")]
    public float baseCost = 10f;         // 기본 비용
    public float costIncreaseValue = 2f; // 증가 값

    public enum CostType { Add, Multiply } // 비용 증가 방식
    public CostType costType = CostType.Add;

    [Header("레벨 증가 배수")]
    public int levelStep = 1; // 한 번에 오르는 레벨

    [Header("단계별 최대 레벨")]
    public List<int> stageMaxLevels = new List<int>() { 200, 400, 600 }; // 단계별 MAX

    [Header("TMP UI")]
    public TMP_Text goldText;   // 골드 표시
    public TMP_Text levelText;  // 레벨 표시
    public TMP_Text statText;   // 능력치 표시
    public TMP_Text costText;   // 비용 표시

    [Header("MAX UI 설정")]
    public GameObject maxImage; // MAX 표시 이미지

    [Header("버튼")]
    public Button levelUpButton; // 레벨업 버튼

    [Header("이미지 교체")]
    public GameObject originalImage; // 기본 이미지
    public GameObject changedImage;  // MAX 시 교체 이미지

    // =============================

    void Start()
    {
        UpdateUI(); // 시작 시 UI 초기화
    }

    // =============================
    // 레벨업 시도

    public void TryLevelUp()
    {
        if (DummyPlayer == null) return; // 플레이어 없으면 종료

        float currentGold = DummyPlayer.gold;

        int maxLevel = GetMaxLevel(); // 현재 단계 기준 MAX

        // 목표 레벨 계산
        int targetLevel = currentLevel + levelStep;
        if (targetLevel > maxLevel)
            targetLevel = maxLevel;

        // 총 비용 계산
        float totalCost = CalculateTotalCost(currentLevel, targetLevel);

        // 골드 부족 시 종료
        if (currentGold < totalCost)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 증가량 계산
        int levelGained = targetLevel - currentLevel;
        float statGained = statPerLevel * levelGained;

        // 골드 차감
        DummyPlayer.gold -= totalCost;

        // 레벨 및 능력치 증가
        currentLevel = targetLevel;
        currentStat += statGained;

        // UI 갱신
        UpdateUI();

        // Manager에게 변경 알림 (총합 갱신 + 단계 체크)
        FindObjectOfType<Up_Manager>()?.OnLevelChanged();
    }

    // =============================
    // UI 갱신

    public void UpdateUI()
    {
        if (DummyPlayer == null) return;

        float currentGold = DummyPlayer.gold;

        // 골드 표시
        if (goldText != null)
            goldText.text = FormatGold(currentGold);

        // 레벨 표시
        if (levelText != null)
            levelText.text = $"Lv.{currentLevel}";

        // 능력치 표시
        if (statText != null)
            statText.text = $"+{currentStat:F0}";

        int maxLevel = GetMaxLevel();

        // 다음 비용 계산
        float nextCost = CalculateTotalCost(
            currentLevel,
            Mathf.Min(currentLevel + levelStep, maxLevel)
        );

        // 비용 표시 + 색상 변경
        if (costText != null)
        {
            costText.text = FormatGold(nextCost);

            if (currentGold < nextCost)
                costText.color = Color.red;   // 부족
            else
                costText.color = Color.white; // 가능
        }

        // =============================
        // MAX 상태 처리

        bool isMax = currentLevel >= maxLevel;

        // 버튼 ON/OFF (완전히 사라짐)
        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(!isMax);

        // MAX 이미지 표시
        if (maxImage != null)
            maxImage.SetActive(isMax);

        // 이미지 교체
        if (originalImage != null)
            originalImage.SetActive(!isMax);

        if (changedImage != null)
            changedImage.SetActive(isMax);
    }

    // =============================
    // 단계 상승 시 호출

    public void ResetLevel()
    {
        currentLevel = 1; // 레벨만 초기화 (능력치는 유지)

        // 버튼 다시 활성화
        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(true);

        // MAX 이미지 끄기
        if (maxImage != null)
            maxImage.SetActive(false);

        // 이미지 원상복구
        if (originalImage != null)
            originalImage.SetActive(true);

        if (changedImage != null)
            changedImage.SetActive(false);

        // UI 갱신
        UpdateUI();

        // Manager 다시 갱신 (총합 초기화 반영)
        FindObjectOfType<Up_Manager>()?.OnLevelChanged();
    }

    // =============================
    // 골드 포맷

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
    // 총 비용 계산

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

    // =============================
    // 특정 레벨 비용 계산

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

    // =============================
    // 현재 단계 기준 MAX 레벨 반환

    int GetMaxLevel()
    {
        if (currentStage < stageMaxLevels.Count)
            return stageMaxLevels[currentStage];

        return stageMaxLevels[stageMaxLevels.Count - 1];
    }
}