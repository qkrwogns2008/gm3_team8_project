using System.Collections.Generic;
using UnityEngine;

public class Stage_View_N : MonoBehaviour
{
    [Header("참조")]
    public Up_Manager upManager;
    public List<LV_UP_Button> buttons = new List<LV_UP_Button>();

    // =============================

    public void OnClick()
    {
        if (upManager == null) return;

        int realStage = upManager.GetCurrentStage();

        if (upManager.viewStage >= realStage) return;

        upManager.viewStage++;

        if (upManager.viewStage == realStage)
        {
            upManager.OnLevelChanged();
            return;
        }

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

            float statValue;

            if (stage < btn.statPerLevelByStage.Count)
                statValue = btn.statPerLevelByStage[stage];
            else
                statValue = btn.statPerLevelByStage[btn.statPerLevelByStage.Count - 1];

            float previewStat = statValue * (maxLevel - 1);

            if (btn.levelText != null)
                btn.levelText.text = $"Lv.{maxLevel}";

            if (btn.statText != null)
                btn.statText.text = $"+{previewStat:F0}";

            int displayStage = stage + 1;

            if (btn.stageTextA != null)
                btn.stageTextA.text = $"영향력 {displayStage}단계";

            if (btn.stageTextB != null)
                btn.stageTextB.text = $"{displayStage}/10";

            // MAX 상태 UI
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