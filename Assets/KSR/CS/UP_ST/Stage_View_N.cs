using System.Collections.Generic;
using UnityEngine;

public class Stage_View_N : MonoBehaviour
{
    [Header("참조")]
    public Up_Manager upManager; // 스테이지 관리 매니저
    public List<LV_UP_Button> buttons = new List<LV_UP_Button>(); // 버튼 목록

    // =============================
    // 다음 스테이지 보기 버튼 클릭

    public void OnClick()
    {
        if (upManager == null) return; // 매니저 없으면 종료

        int realStage = upManager.GetCurrentStage(); // 실제 현재 스테이지 가져오기

        if (upManager.viewStage >= realStage) return; // 실제 스테이지보다 크면 제한

        upManager.viewStage++; // 프리뷰 스테이지 증가

        // 실제 스테이지로 돌아온 경우
        if (upManager.viewStage == realStage)
        {
            upManager.OnLevelChanged(); // 실제 UI 상태로 복구
            return;
        }

        ApplyPreview(upManager.viewStage); // 프리뷰 적용
    }

    // =============================
    // 프리뷰 적용 (해당 스테이지 최대 상태 표시)

    void ApplyPreview(int stage)
    {
        foreach (var btn in buttons) // 모든 버튼 반복
        {
            if (btn == null) continue; // null 체크

            int maxLevel = GetMaxLevel(btn, stage); // 해당 스테이지 최대 레벨

            float statValue; // 단계별 능력치 값

            // 스테이지에 맞는 능력치 값 가져오기
            if (stage < btn.statPerLevelByStage.Count)
                statValue = btn.statPerLevelByStage[stage];
            else
                statValue = btn.statPerLevelByStage[btn.statPerLevelByStage.Count - 1];

            float previewStat = statValue * (maxLevel - 1); // 최대 레벨 기준 능력치 계산

            // 레벨 UI 표시
            if (btn.levelText != null)
                btn.levelText.text = $"Lv.{maxLevel}";

            // 능력치 UI 표시
            if (btn.statText != null)
                btn.statText.text = $"+{previewStat:F0}";

            int displayStage = stage + 1; // 표시용 단계

            // 단계 텍스트 UI
            if (btn.stageTextA != null)
                btn.stageTextA.text = $"영향력 {displayStage}단계";

            if (btn.stageTextB != null)
                btn.stageTextB.text = $"{displayStage}/10";

            // MAX 상태 UI 처리
            if (btn.levelUpButton != null)
                btn.levelUpButton.gameObject.SetActive(false); // 버튼 비활성

            if (btn.maxImage != null)
                btn.maxImage.SetActive(true); // MAX 표시

            if (btn.originalImage != null)
                btn.originalImage.SetActive(false); // 기본 이미지 숨김

            if (btn.changedImage != null)
                btn.changedImage.SetActive(true); // 변경 이미지 표시
        }
    }

    // =============================
    // 스테이지별 최대 레벨 반환

    int GetMaxLevel(LV_UP_Button btn, int stage)
    {
        if (stage < btn.stageMaxLevels.Count)
            return btn.stageMaxLevels[stage]; // 해당 단계 최대 레벨

        return btn.stageMaxLevels[btn.stageMaxLevels.Count - 1]; // 마지막 값 반환
    }
}