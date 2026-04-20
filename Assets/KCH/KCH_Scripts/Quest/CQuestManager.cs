using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal.Profiling;
using Coffee.UIExtensions;
using System.Collections;

public class CQuestManager : MonoBehaviour
{
    public static CQuestManager Instance;

    #region 인스펙터
    [Header("퀘스트 SO 리스트")]
    public List<CQuestDataSO> QuestDataList = new List<CQuestDataSO>();
    public List<UserQuestData> UserQuestList => CDataManager.Instance.UserData.QuestList;
    #endregion

    #region 내부 변수
    // CDataManager 연결
    private Dictionary<int, CQuestDataSO> _questDict = new Dictionary<int, CQuestDataSO>();
    private Coroutine _monitorCoroutine;    // 모니터 코루틴 참조

    private int _lastStageLevel;            // 마지막 스테이지 데이터
    private int _lastAtkLevel;              // 마지막 공격력 레벨 데이터
    private int _lastDefLevel;              // 마지막 방어력 레벨 데이터
    private int _lastLifeLevel;             // 마지막 생명력 레벨 데이터
    private int _lastTotalHeroLevel;        // 마지막 영웅 레벨 데이터
    #endregion

    // 옵저버 알림
    public Action OnDataUpdate;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        }

        else
        {
            Destroy(gameObject);
        }


        BuildQuestMap();
    }

    private void Start()
    {
        // 데이터 초기화
        InitProgress();

        // 데이터 수치 백업
        SyncLastData();

        if (_monitorCoroutine != null) 
        {
            StopCoroutine(_monitorCoroutine);
        }

        // 모니터링 코루틴 시작
        _monitorCoroutine = StartCoroutine(CO_MonitorUserData());
    }

    private void SyncLastData()
    {
        var userData = CDataManager.Instance.UserData;

        _lastStageLevel = userData.MainStageLevel;
        _lastAtkLevel = userData.Atk_Level;
        _lastDefLevel = userData.Def_Level;
        _lastLifeLevel = userData.Life_Level;
        _lastTotalHeroLevel = GetTotalHeroLevel();
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

    private void OnEnable()
    {
        // 이벤트 구독 시작
        CQuestEvent.QuestProgressAction += HandleQuestProgress;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        CQuestEvent.QuestProgressAction -= HandleQuestProgress;
        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
        }
    }

    private IEnumerator CO_MonitorUserData()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f);

        while (true)
        {
            var userData = CDataManager.Instance.UserData;

            if (userData == null) 
            { 
                yield return delay; 
                continue;
            }

            bool isChanged = false;

            // 메인 스테이지 레벨 (Type 2)
            if (userData.MainStageLevel > _lastStageLevel)
            {
                int diff = userData.MainStageLevel - _lastStageLevel;
                HandleQuestProgress(EQuestType.StateUp, diff);
                _lastStageLevel = userData.MainStageLevel;
                isChanged = true;
            }

            // 공격력 영향력 레벨 (Type 4)
            if (userData.Atk_Level > _lastAtkLevel)
            {
                // 증가량 계산
                int diff = userData.Atk_Level - _lastAtkLevel;
                HandleQuestProgress(EQuestType.AtkLevel, diff);
                // 백업 갱신
                _lastAtkLevel = userData.Atk_Level;
                isChanged = true;
            }

            // 방어력 영향력 레벨 (Type 5)
            if (userData.Def_Level > _lastDefLevel)
            {
                int diff = userData.Def_Level - _lastDefLevel;
                HandleQuestProgress(EQuestType.DefLevel, diff);
                _lastDefLevel = userData.Def_Level;
                isChanged = true;
            }

            // 생명력 영향력 레벨 (Type 6)
            if (userData.Life_Level > _lastLifeLevel)
            {
                int diff = userData.Life_Level - _lastLifeLevel;
                HandleQuestProgress(EQuestType.LifeLevel, diff);
                _lastLifeLevel = userData.Life_Level;
                isChanged = true;
            }

            int currentTotalHeroLevel = GetTotalHeroLevel();
            // 영웅 레벨 증가 (Type 7)
            if (currentTotalHeroLevel > _lastTotalHeroLevel)
            {
                int diff = currentTotalHeroLevel - _lastTotalHeroLevel;
                HandleQuestProgress(EQuestType.HeroLevel, diff);
                _lastTotalHeroLevel = currentTotalHeroLevel;
                isChanged = true;
            }

            if (isChanged)
            {
                // 갱신 알림
                OnDataUpdate?.Invoke();
            }

            yield return delay;
        }
    }

    private int GetTotalHeroLevel()
    {
        int total = 0;

        var heroList = CDataManager.Instance.UserData.HeroList;
        for (int i = 0; i < heroList.Count; i++)
        {
            total += heroList[i].Level;
        }
        return total;
    }

    // 딕셔너리 캐싱
    private void BuildQuestMap()
    {
        // 초기화
        _questDict.Clear();

        for (int i = 0; i < QuestDataList.Count; i++)
        {
            if (QuestDataList[i] != null && !_questDict.ContainsKey(QuestDataList[i].QuestID))
            {
                _questDict.Add(QuestDataList[i].QuestID, QuestDataList[i]);
            }
        }
    }

    private void HandleQuestProgress(EQuestType type, int value)
    {
        bool isChanged = false;
        int count = UserQuestList.Count;

        for (int i = 0; i < count; i++)
        {
            UserQuestData progress = UserQuestList[i]; 
            if (_questDict.TryGetValue(progress.QuestID, out CQuestDataSO questSO))
            {
                if (questSO.QuestType == type)
                {
                    progress.CurrentGague += value;
                    isChanged = true;

                    Debug.Log($"{questSO.QuestName} 진행도 증가: {value} (현재: { progress.CurrentGague}/{ questSO.QuestGoal})");
                    while (progress.CurrentGague >= questSO.QuestGoal)
                    {
                        progress.CurrentGague -= questSO.QuestGoal;
                        progress.ReewardCount++;

                        Debug.Log($"{questSO.QuestName} 목표 달성 / 수령 가능횟수: { progress.ReewardCount}");
                    }
                }
            }
        }

        if (isChanged)
        {
            OnDataUpdate?.Invoke();
            CDataManager.Instance.SaveUserData();
            Debug.Log("퀘스트 업데이트 완료");
        }
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
                GiveReward(dataSO.QuestReward, rewardTotal);
                Debug.Log($"{dataSO.QuestName} 보상 : {rewardTotal}개 획득");

                // 팝업 리스트 생성
                List<SQuestReward> rewards = new List<SQuestReward>();
                rewards.Add(new SQuestReward(dataSO.QuestReward, rewardTotal));
                CPopupManager.Instance.ShowRewardPopup(rewards);

                // 보상 횟수 초기화
                progress.ReewardCount = 0;

                // UI 갱신
                OnDataUpdate?.Invoke();

                CDataManager.Instance.SaveUserData();
                return;
            }
        }
    }

    // 보상 모두 받기
    public List<SQuestReward> RewardAllQuest()
    {
        // 보상 저장소 딕셔너리
        Dictionary<EQuestReward, int> rewardDict = new Dictionary<EQuestReward, int>();
        bool isAllReward = false;

        // 퀘스트 순회
        int count = UserQuestList.Count;
        for (int i = 0; i < UserQuestList.Count; i++)
        {
            var progress = UserQuestList[i];
            if (progress.ReewardCount > 0)
            {
                CQuestDataSO dataSO = _questDict[progress.QuestID];
                int rewardTotal = dataSO.RewardQuest * progress.ReewardCount;

                // 타입별 합산
                if (rewardDict.ContainsKey(dataSO.QuestReward))
                {
                    rewardDict[dataSO.QuestReward] += rewardTotal;
                }

                else
                {
                    rewardDict.Add(dataSO.QuestReward, rewardTotal);
                }

                // 보상 지급
                GiveReward(dataSO.QuestReward, rewardTotal);
                Debug.Log($"{dataSO.QuestName} 보상 : {rewardTotal}개 획득");

                // 보상 횟수 초기화
                progress.ReewardCount = 0;
                isAllReward = true; 
            }
        }

        // 리스트 변환
        List<SQuestReward> rewardList = new List<SQuestReward>();
        foreach (var item in rewardDict)
        {
            rewardList.Add(new SQuestReward(item.Key, item.Value));
        }

        // 모두 받기
        if (isAllReward)
        {
            // UI 갱신
            OnDataUpdate?.Invoke();
            CDataManager.Instance.SaveUserData();
            Debug.Log("퀘스트 모든 보상 획득");
        }

        // 리스트 반환
        return rewardList;
    }

    private void GiveReward(EQuestReward type, int amount)
    {
        switch (type)
        {
            case EQuestReward.Ruby: 
                CDataManager.Instance.AddRubby(amount);
                break;
            case EQuestReward.Ticket:
                CDataManager.Instance.AddPickUpTicket(amount);
                break;
            case EQuestReward.Exp:
                CDataManager.Instance.AddExp(amount);
                break;
            case EQuestReward.Hero:
                CDataManager.Instance.AddHeroData((EHeroID)amount);
                break;
        }
    }
}
