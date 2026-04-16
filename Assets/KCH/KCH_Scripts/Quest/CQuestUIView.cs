using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CQuestUIView : MonoBehaviour
{
    [Header("퀘스트 UI 설정")]
    public Image QuestIcon;             // 퀘스트 아이콘
    public TMP_Text QusetText;          // 퀘스트 제목

    [Header("퀘스트 게이지 설정")]
    public Image FillImage;             // 퀘스트 게이지 
    public TMP_Text ProgressText;       // 진행도 텍스트

    [Header("퀘스트 보상 관련")]
    public Image RewardIcon;            // 보상 아이콘
    public TMP_Text RewardText;         // 누적 보상

    [Header("퀘스트 버튼")]
    public Button QuestButton;          // 진행중, 받기 버튼
    public TMP_Text QuestButtonText;    // 버튼 글자

    // UI 갱신 함수
    public void SetUI(CQuestDataSO data, UserQuestData progress)
    {
        QuestIcon.sprite = data.QuestIcon;
        QusetText.text = data.QuestName;

        // 보상 누적 수량
        RewardIcon.sprite = data.RewardIcon;

        QuestButton.onClick.RemoveAllListeners();
        QuestButton.onClick.AddListener(() => CQuestManager.Instance.RewardQuest(progress.QuestID));

        // 버튼 상태 제어
        // 진행도 게이지
        if (progress.ReewardCount > 0)
        {
            ProgressText.text = $"{data.QuestGoal} / {data.QuestGoal}";
            FillImage.fillAmount = 1f;

            QuestButtonText.text = "받기";
            QuestButton.interactable = true;
        }
        else
        {
            ProgressText.text = $"{progress.CurrentGague} / {data.QuestGoal}";
            FillImage.fillAmount = (float)progress.CurrentGague / data.QuestGoal;

            QuestButtonText.text = "진행중";
            QuestButton.interactable = false;
        }

        int totalReward = data.RewardQuest * (progress.ReewardCount > 0 ? progress.ReewardCount : 1);
        RewardText.text = totalReward.ToString();
    }  
}
