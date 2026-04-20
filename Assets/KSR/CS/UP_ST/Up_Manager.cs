using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Up_Manager : MonoBehaviour
{
    [Header("업그레이드 버튼들")]
    public List<LV_UP_Button> upgradeButtons = new List<LV_UP_Button>(); // 버튼 목록

    [Header("플레이어")]
    public Dummy_Player DummyPlayer; // 플레이어 참조

    [Header("단계 기준 (총 레벨)")]
    public List<int> stageThresholds = new List<int>() { 600, 1800, 3600 }; // 단계 기준

    [Header("총합 표시 UI")]
    public TMP_Text totalLevelText; // 총합 UI

    private int currentStage = 0; // 현재 실제 단계 (단일 기준)

    [Header("프리뷰용 현재 보는 단계")]
    public int viewStage; // 프리뷰 단계

    void Start()
    {
        // 저장된 스테이지 로드
        if (S_UP_Manger.Instance != null)
        {
            S_UP_Manger.Instance.Load(); // 데이터 로드
            currentStage = S_UP_Manger.Instance.data.currentStage; // 스테이지 복원
        }

        viewStage = currentStage; // 시작 시 동기화
        OnLevelChanged(); // 초기 계산 및 UI 갱신
    }

    int GetTotalLevel()
    {
        int total = 0; // 합계 초기화

        foreach (var btn in upgradeButtons) // 버튼 순회
        {
            if (btn == null) continue; // null 체크
            total += btn.currentLevel; // 레벨 합산
        }

        return total; // 반환
    }

    void CheckStageUp()
    {
        if (currentStage >= stageThresholds.Count) return; // 최대 단계 체크

        int totalLevel = GetTotalLevel(); // 총 레벨 계산

        if (totalLevel >= stageThresholds[currentStage]) // 조건 만족 시
        {
            StageUp(); // 단계 상승
        }
    }

    void StageUp()
    {
        currentStage++; // 단계 증가

        Debug.Log("단계 상승! 현재 단계: " + currentStage); // 로그

        viewStage = currentStage; // 프리뷰 동기화

        foreach (var btn in upgradeButtons) // 모든 버튼에 반영
        {
            if (btn == null) continue;

            btn.ResetLevel(); // 레벨 초기화
        }

        SaveStage(); // 스테이지 저장
    }

    void UpdateTotalLevelUI()
    {
        if (totalLevelText == null) return; // UI 체크

        int totalLevel = GetTotalLevel(); // 총 레벨

        if (currentStage >= stageThresholds.Count) // 최대 단계
        {
            totalLevelText.text = "MAX"; // 표시
            return;
        }

        int target = stageThresholds[currentStage]; // 목표값

        totalLevelText.text = totalLevel + " / " + target; // UI 갱신
    }

    public void OnLevelChanged()
    {
        CheckStageUp(); // 단계 체크
        UpdateTotalLevelUI(); // UI 갱신

        foreach (var btn in upgradeButtons) // 버튼 UI 갱신
        {
            if (btn == null) continue;
            btn.UpdateUI();
        }
    }

    public int GetCurrentStage()
    {
        return currentStage; // 현재 단계 반환
    }

    public void ResetStage()
    {
        currentStage = 0; // 단계 초기화
        viewStage = 0; // 프리뷰 초기화

        foreach (var btn in upgradeButtons) // 버튼 초기화
        {
            if (btn == null) continue;

            btn.currentLevel = 1; // 레벨 초기화
            btn.stageStat = 0; // 표시값 초기화
            btn.currentStat = 0; // 누적값 초기화
            btn.currentCost = btn.baseCost; // 비용 초기화

            btn.UpdateUI(); // UI 갱신
        }

        SaveStage(); // 스테이지 저장
        UpdateTotalLevelUI(); // UI 갱신
    }

    void SaveStage()
    {
        if (S_UP_Manger.Instance == null) return; // 매니저 체크

        S_UP_Manger.Instance.data.currentStage = currentStage; // 스테이지 저장
        S_UP_Manger.Instance.Save(); // 저장
    }
}