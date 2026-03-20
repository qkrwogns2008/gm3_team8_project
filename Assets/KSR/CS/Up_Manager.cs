using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Up_Manager : MonoBehaviour
{
    [Header("업그레이드 버튼들")]
    public List<LV_UP_Button> upgradeButtons = new List<LV_UP_Button>();

    [Header("플레이어")]
    public Dummy_Player DummyPlayer;

    [Header("단계 기준 (총 레벨)")]
    public List<int> stageThresholds = new List<int>() { 600, 1800, 3600 };

    [Header("총합 표시 UI")]
    public TMP_Text totalLevelText;

    private int currentStage = 0;

    // =============================

    void Start()
    {
        OnLevelChanged();
    }

    // =============================
    // 총 레벨 계산

    int GetTotalLevel()
    {
        int total = 0;

        foreach (var btn in upgradeButtons)
        {
            if (btn == null) continue;
            total += btn.currentLevel;
        }

        return total;
    }

    // =============================
    // 단계 상승 체크

    void CheckStageUp()
    {
        if (currentStage >= stageThresholds.Count) return;

        int totalLevel = GetTotalLevel();

        if (totalLevel >= stageThresholds[currentStage])
        {
            StageUp();
        }
    }

    // =============================
    // 단계 상승 처리

    void StageUp()
    {
        currentStage++;

        Debug.Log("단계 상승! 현재 단계: " + currentStage);

        foreach (var btn in upgradeButtons)
        {
            if (btn == null) continue;

            btn.currentStage = currentStage;
            btn.ResetLevel();
        }
    }

    // =============================
    // 총합 텍스트 갱신

    void UpdateTotalLevelUI()
    {
        if (totalLevelText == null) return;

        int totalLevel = GetTotalLevel();

        if (currentStage >= stageThresholds.Count)
        {
            totalLevelText.text = "MAX";
            return;
        }

        int target = stageThresholds[currentStage];

        totalLevelText.text = totalLevel + " / " + target;
    }

    // =============================
    // 외부에서 호출 (버튼 눌렀을 때)

    public void OnLevelChanged()
    {
        // 단계 먼저 체크
        CheckStageUp();

        // 그 다음 텍스트 갱신
        UpdateTotalLevelUI();

        // 각 버튼 UI 갱신
        foreach (var btn in upgradeButtons)
        {
            if (btn == null) continue;
            btn.UpdateUI();
        }
    }
}