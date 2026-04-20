using UnityEngine;


[CreateAssetMenu(menuName = "Quest/QuestData", fileName = "QuestData_")]
public class CQuestDataSO : ScriptableObject
{
    [Header("퀘스트 정보")]
    public int QuestID;                 // 퀘스트 ID
    public string QuestName;            // 퀘스트 이름

    [Header("퀘스트 조건")]
    public EQuestType QuestType;        // 퀘스트 종류
    public int QuestGoal;               // 목표 수치

    [Header("퀘스트 완료")]
    public int RewardQuest;             // 보상 수치
    public EQuestReward QuestReward;    // 보상 종류
}
