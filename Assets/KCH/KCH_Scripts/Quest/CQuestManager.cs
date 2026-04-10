using System;
using System.Collections.Generic;
using UnityEngine;


public class CQuestManager : MonoBehaviour
{
    public static CQuestManager Instance;

    [Header("퀘스트 SO 리스트")]
    public List<CQuestDataSO> QuestDataList = new List<CQuestDataSO>();
    private Dictionary<int, CQuestDataSO> _questDict = new Dictionary<int, CQuestDataSO>();

    // 임시 데이터
    public List<UserQuestData> UserQuestList => CDataManager.Instance.UserData.QuestList;

    // 옵저버 알림
    public Action OnDataUpdate;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        // 딕셔너리 매핑
        BuildMaps();

        // 임시 데이터 생성
        InitProgress();
    }

    // 딕셔너리 매핑
    private void BuildMaps()
    {
        _questDict.Clear();
        int count = QuestDataList.Count;

        for (int i = 0; i < count; i++)
        {
            CQuestDataSO data = QuestDataList[i];
            if (!_questDict.ContainsKey(data.QuestID))
            {
                _questDict.Add(data.QuestID, data);
            }
        }
    }

    private void InitProgress()
    {
        if (UserQuestList.Count > 0)
        {
            return;
        }

        int count = QuestDataList.Count;
        for (int i = 0; i < count; i++)
        {
            UserQuestData progress = new UserQuestData();
            progress.QuestID = QuestDataList[i].QuestID;
            progress.CurrentGague = 0;
            progress.ReewardCount = 0;

            // 리스트에 추가
            UserQuestList.Add(progress);
        }

        CDataManager.Instance.SaveUserData();
    }

    /// 퀘스트 진행도 업데이트 함수
    public void QuestProgress(EQuestType questType, int amount)
    {
        var userData = CDataManager.Instance.UserData;

        // 유저 리스트 확인
        int progressCount = UserQuestList.Count;

        for (int i = 0; i < progressCount; i++)
        {
            UserQuestData progress = UserQuestList[i];

            // progress.QuestID가 딕셔너리에 있는지 확인
            if (_questDict.ContainsKey(progress.QuestID))
            {
                CQuestDataSO dataSO = _questDict[progress.QuestID];

                // 타입 일치시
                if (dataSO.QuestType == questType)
                {
                    // 게이지 증가
                    progress.CurrentGague += amount;

                    // 목표치를 넘으면 보상 누적
                    while (progress.CurrentGague >= dataSO.QuestGoal)
                    {
                        progress.CurrentGague -= dataSO.QuestGoal;

                        // 보상 개수 증가
                        progress.ReewardCount++;
                    }
                }
            }
        }

        if (OnDataUpdate != null)
        {
            OnDataUpdate.Invoke();
        }

        CDataManager.Instance.SaveUserData();
    }

    // 유저 보상 지급
    public void RewardQuest(int questID)
    {
        for (int i = 0; i < UserQuestList.Count; i++)
        {
            var progress = UserQuestList[i];
            if (progress.QuestID == questID && progress.ReewardCount > 0)
            {
                 
                CQuestDataSO dataSO = _questDict[questID];

                // 실제 유저 재화 시스템과 연결
                int rewardTotal = dataSO.RewardQuest * progress.ReewardCount;
             
                // 보상 지급
                CDataManager.Instance.AddPickUpTicket(rewardTotal);                 
                Debug.Log($"{dataSO.QuestName} 보상 : {rewardTotal}개 획득");
             
                // 보상 횟수 초기화
                progress.ReewardCount = 0;
             
                // UI 갱신
                OnDataUpdate?.Invoke();
             
                CDataManager.Instance.SaveUserData();
                return;
            }
        }
    }

    public void RewardAllQuest()
    {
        bool isAllReward = false;

        for (int i = 0; i < UserQuestList.Count; i++)
        {
            var progress = UserQuestList[i];

            if (progress.ReewardCount > 0)
            {

                CQuestDataSO dataSO = _questDict[progress.QuestID];
                int rewardTotal = dataSO.RewardQuest * progress.ReewardCount;

                // 보상 지급
                CDataManager.Instance.AddPickUpTicket(rewardTotal);
                Debug.Log($"{dataSO.QuestName} 보상 : {rewardTotal}개 획득");

                // 보상 횟수 초기화
                progress.ReewardCount = 0;
                isAllReward = true; 
            }
        }

        if (isAllReward)
        {
            // UI 갱신
            OnDataUpdate?.Invoke();
            CDataManager.Instance.SaveUserData();
            Debug.Log("퀘스트 모든 보상 획득");
        }
    }
}
