using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CQuestUIView : MonoBehaviour
{
    [Header("퀘스트 UI 설정")]
    public Image QuestIcon;             // 퀘스트 아이콘
    public TMP_Text QusetText;          // 퀘스트 제목
    public Image FillImage;             // 퀘스트 게이지 
    public TMP_Text ProgressText;       // 진행도 텍스트

    [Header("퀘스트 보상 관련")]
    public Image RewardIcon;            // 보상 아이콘
    public TMP_Text RewardAmountText;   // 누적 보상

    [Header("퀘스트 버튼")]
    public Button QuestButton;          // 진행중, 받기 버튼
    public TMP_Text QuestButtonText;    // 버튼 글자

    // UI 갱신 함수
    public void SetUI(CQuestDataSO data, CQuestProgress progress)
    {
        QuestIcon.sprite = data.QuestIcon;
        QusetText.text = data.QuestName;

        // 진행도 게이지
        ProgressText.text = $"{progress.CurrentGague} / {data.QuestGoal}";
        FillImage.fillAmount = (float)progress.CurrentGague / data.QuestGoal;

        // 보상 누적 수량
        RewardIcon.sprite = data.RewardIcon;

        int totalReward = data.RewardQuest * progress.RewardCOunt;
        RewardAmountText.text = totalReward.ToString();


        QuestButton.onClick.RemoveAllListeners();
        QuestButton.onClick.AddListener(() => {CQuestManager.Instance.RewardQuest(progress.QuestID);});

        // 버튼 상태 제어
        if (progress.RewardCOunt > 0)
        {
            QuestButtonText.text = "받기";
            QuestButton.interactable = true;
        }
        else
        {
            QuestButtonText.text = "진행중";
            QuestButton.interactable = false;
        }
    }  
}
