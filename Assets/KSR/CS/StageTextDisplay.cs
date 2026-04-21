using UnityEngine;
using TMPro;

public class StageTextDisplay : MonoBehaviour
{
    [Header("출력 텍스트")]
    public TMP_Text stageText; // 인스펙터 연결

    void Update()
    {
        if (CDataManager.Instance == null) return;
        if (stageText == null) return;

        int current = CDataManager.Instance.UserData.CurrentStageLevel;

        stageText.text = $"제 {current} 스테이지";
    }
}