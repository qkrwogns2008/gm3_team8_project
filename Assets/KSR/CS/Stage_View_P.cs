using System.Collections.Generic;
using UnityEngine;

public class Stage_View_P : MonoBehaviour
{
    [Header("참조")]
    public Up_Manager upManager;
    public List<LV_UP_Button> buttons = new List<LV_UP_Button>();

    // =============================

    public void OnClick()
    {
        if (upManager == null) return;

        // 0단계 이하로는 못 내려감
        if (upManager.viewStage <= 0) return;

        upManager.viewStage--;

        ApplyPreview(upManager.viewStage);
    }

    // =============================
    // 프리뷰 적용

    void ApplyPreview(int stage)
    {
        foreach (var btn in buttons)
        {
            if (btn == null) continue;

            int maxLevel = GetMaxLevel(btn, stage);

            float previewStat = btn.statPerLevel * (maxLevel - 1);

            // UI 직접 덮어쓰기
            if (btn.levelText != null)
                btn.levelText.text = $"Lv.{maxLevel}";

            if (btn.statText != null)
                btn.statText.text = $"+{previewStat:F0}";

            int displayStage = stage + 1;

            if (btn.stageTextA != null)
                btn.stageTextA.text = $"영향력 {displayStage}단계";

            if (btn.stageTextB != null)
                btn.stageTextB.text = $"{displayStage}/10";
            //

            bool isMax = true; 

            if (btn.levelUpButton != null)
                btn.levelUpButton.gameObject.SetActive(false);

            if (btn.maxImage != null)
                btn.maxImage.SetActive(true);

            if (btn.originalImage != null)
                btn.originalImage.SetActive(false);

            if (btn.changedImage != null)
                btn.changedImage.SetActive(true);
        }
    }

    // =============================

    int GetMaxLevel(LV_UP_Button btn, int stage)
    {
        if (stage < btn.stageMaxLevels.Count)
            return btn.stageMaxLevels[stage];

        return btn.stageMaxLevels[btn.stageMaxLevels.Count - 1];
    }
}