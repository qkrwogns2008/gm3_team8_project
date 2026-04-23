using System;
using System.IO;
using UnityEngine;

public class CDataManager : MonoBehaviour
{
    public static CDataManager Instance { get; private set; }

    public CUserData UserData { get; private set; }

    private string _savePath;
    [SerializeField] bool isDebugMode = false; // 디버그 모드 여부
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            #if UNITY_EDITOR
            // 유니티 에디터(PC 작업 중)일 때는 프로젝트 폴더 바로 옆에 저장 (찾기 쉬움)
            _savePath = Path.Combine(Application.dataPath, "..", "SaveData.json");
            #else
            // 실제 빌드(모바일, PC 빌드본)일 때는 안전한 영구 저장소에 저장
            _savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
            #endif
            LoadUserData();
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        CDataManager.Instance.AddHeroDummy(EHeroID.Baskin);
        CDataManager.Instance.AddHeroDummy(EHeroID.Nami);
        CDataManager.Instance.AddHeroDummy(EHeroID.Loto);
        CDataManager.Instance.AddHeroDummy(EHeroID.Jak);
        CDataManager.Instance.AddHeroDummy(EHeroID.Sarah);
        CDataManager.Instance.AddHeroDummy(EHeroID.Elga);
        CDataManager.Instance.AddHeroDummy(EHeroID.Karon);
        CDataManager.Instance.AddHeroDummy(EHeroID.Rook);
        CDataManager.Instance.AddHeroDummy(EHeroID.Snipper);
        CDataManager.Instance.AddHeroDummy(EHeroID.Shane);
        CDataManager.Instance.AddHeroDummy(EHeroID.Evan);
        CDataManager.Instance.AddHeroDummy(EHeroID.Alice);
        CDataManager.Instance.AddHeroDummy(EHeroID.Teo);
        CDataManager.Instance.AddHeroDummy(EHeroID.Yeonhee);
        CDataManager.Instance.AddHeroDummy(EHeroID.Radgrid);
        CDataManager.Instance.AddHeroDummy(EHeroID.Ecila);
    }
    // [저장] 데이터 -> JSON -> 파
    // 일

    private void Start()
    {
        
    }
    public void SaveUserData()
    {
        UserData.LastLogoutTime = System.DateTime.Now.Ticks;
        string json = JsonUtility.ToJson(UserData, true);
        File.WriteAllText(_savePath, json);
        if (isDebugMode)
        {
            Debug.Log($"저장 완료: {_savePath}");
        }
    }

    // [로드] 파일 -> JSON -> 데이터
    public void LoadUserData()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            UserData = JsonUtility.FromJson<CUserData>(json);
        }
        else
        {
            // 파일이 없으면 초기 데이터 생성
            UserData = new CUserData();
            SaveUserData();
        }
    }


    // 스테이지 레벨업
    public void MainStageLevelUP(int amount) // 보통 1로 쓸것
    {
        UserData.MainStageLevel += amount;
        UserData.CurrentStageLevel = UserData.MainStageLevel; // 메인 스테이지 레벨업 시 현재 스테이지도 같이 변경
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($" 최대 스테이지 : {UserData.MainStageLevel}");
        }
    }

    // 스테이지 변경
    public void ChangeCurrentStageLevel(int amount) // 보통 1로 쓸것
    {
        if(amount < 1 || amount > UserData.MainStageLevel)
        {
            if (isDebugMode)
            {
                Debug.Log($"스테이지 레벨 변경 취소");
            }
            return;
        }
        UserData.CurrentStageLevel = amount;
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($" 현재 스테이지 : {UserData.CurrentStageLevel}");
        }
    }

    // gold 추가
    public void AddGold(int amount)
    {
        UserData.Gold += amount;
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($"골드 추가: {amount} / 현재 골드: {UserData.Gold}");
        }
    }

    // gold 사용
    public bool SpendGold(int amount)     
    {
        if (UserData.Gold >= amount)
        {
            UserData.Gold -= amount;
            SaveUserData();
            return true;
        }

        if (isDebugMode)
        {
            Debug.Log($"골드가 부족합니다. 필요 골드: {amount}");
        }
        return false;
    }

    // Ruby 추가
    public void AddRubby(int amount)    
    {
        UserData.Ruby += amount;
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($"루비 추가: {amount} / 현재 루비: {UserData.Ruby}");
        }
    }

    // Ruby 사용
    public bool SpendRubby(int amount)  
    {
        if (UserData.Ruby >= amount)
        {
            UserData.Ruby -= amount;
            SaveUserData();
            return true;
        }

        if (isDebugMode)
        {
            Debug.Log($"루비가 부족합니다. 필요 루비: {amount}");
        }
        return false;
    }

    public void AddPickUpTicket(int amount)
    {
        UserData.PickUpTicket += amount;
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($"픽업티켓 추가: {amount} / 현재 픽업티켓: {UserData.PickUpTicket}");
        }
    }

    // 픽업 티켓 사용
    public bool SpendPickUpTicket(int amount)
    {
        if (UserData.PickUpTicket >= amount)
        {
            UserData.PickUpTicket -= amount;
            SaveUserData();
            return true;
        }

        if (isDebugMode)
        {
            Debug.Log($"픽업티켓 부족합니다. 필요 픽업티켓: {amount}");
        }
        return false;
    }

    // 유저 경험치 획득
    public void AddExp(int amount)
    {
        UserData.expPoint += amount;
        SaveUserData();
        if (isDebugMode)
        {
            Debug.Log($"경험치 추가: {amount} / 현재 경험치: {UserData.PickUpTicket}");
        }
    }

    // 유저 경험치 증가
    public bool SpendExp(int amount)
    {
        if (UserData.expPoint >= amount)
        {
            UserData.expPoint -= amount;
            SaveUserData();
            return true;
        }

        if (isDebugMode)
        {
            Debug.Log($"EXP 보유량이 부족합니다. 필요 EXP: {amount}");
        }
        return false;
    }

    // 공격력 강화
    public void UpgradeBaseDamage(int amount)
    {
        UserData.Atk_Level = amount;
        if (isDebugMode)
        {
            Debug.Log($"[강화] 기본 공격력 상승 현재: {UserData.Atk_Level}");
        }
        SaveUserData();
    }

    // 방어력 강화
    public void UpgradeBaseShield(int amount)
    {
        UserData.Def_Level = amount;
        if (isDebugMode)
        {
            Debug.Log($"[강화] 기본 방어력 상승 현재: {UserData.Def_Level}");
        }
        SaveUserData();
    }


    // 체력 강화
    public void UpgradeBaseHP(int amount)
    {
        UserData.Life_Level = amount;
        if (isDebugMode)
        {
            Debug.Log($"[강화] 기본 체력 상승 현재: {UserData.Life_Level}");
        }
        SaveUserData();
    }

    // 유저 강화 수치 
    public struct UserUpgradeStatus
    {
        public int UserAtkLevel;
        public int UserDefLevel;
        public int UserLifeLevel;
    }

    public UserUpgradeStatus GetUserUpgradeStatus()
    {
        return new UserUpgradeStatus
        {
            UserAtkLevel = UserData.Atk_Level,
            UserDefLevel = UserData.Def_Level,
            UserLifeLevel = UserData.Life_Level
        };
    }


    // 영웅 추가
    public void AddHeroData(EHeroID id)
    {
        // 보유 여부 확인
        var hero = GetHeroData(id);

        if (hero != null)
        {
            // 이미 보유 중
            if(hero.Quantity >= 5)
            {
                if (isDebugMode)
                {
                    Debug.Log($"영웅 최대 랭크입니다. (ID: {id} / 현재 랭크: {hero.Quantity})");
                }
                return;
            }
            hero.Quantity++;
            if (isDebugMode)
            {
                Debug.Log($"이미 보유 중인 영웅입니다. 레벨업! (ID: {id} / 현재 랭크: {hero.Quantity})");
            }
        }
        else
        {
            // 신규 획득
            UserData.HeroList.Add(new UserHeroData { HeroID = id, Quantity = 1 });
            if (isDebugMode)
            {
                Debug.Log($"새로운 영웅 획득! (ID: {id})");
            }   
        }
		if (CHeroManager.Instance != null)
		{
			CHeroManager.Instance.RefreshUpgradeStat(id);
		}
	}

    // 영웅 데이터 조회


    // id 영웅의 레벨을 level로 설정하는 함수
    public void SetHeroLevel(EHeroID id, int level)    
    {
        var hero = GetHeroData(id);
        // 영웅 보유 여부 확인
        if (hero != null)
        {
            hero.Level = level;
        }
        else
        {
            // 영웅 미보유시
            if (isDebugMode)
            {
                Debug.Log($"User가 보유하지 않은 영웅(ID: {id})의 레벨설정 불가.");
            }
        }
        SaveUserData();
    }

    
    // id 영웅의 레벨을 level만큼 추가하는 함수 
    public void AddHeroLevel(EHeroID id, int level)     
    {
        var hero = GetHeroData(id);
        // 영웅 보유 여부 확인
        if (hero != null)
        {
            hero.Level += level;
			
			if (CHeroManager.Instance != null)
			{
				CHeroManager.Instance.RefreshUpgradeStat(id);
			}
		}
        else
        {
            // 영웅 미보유시
            if (isDebugMode)
            {
                Debug.Log($"User가 보유하지 않은 영웅(ID: {id})의 레벨설정 불가.");
            }
        }
        SaveUserData();
    }

    public int CheckHeroArray(int x, int y)     // x,y 범위 : 0~3 , heroID : 0(미배치)~4002(영웅ID 최대값)  승래님과 Check
    {
        int arrayIndex = x + 4 * y;
        if (arrayIndex < 0 || arrayIndex >= UserData.Hero_Array.Length)
        {
            if (isDebugMode)
            {
                Debug.Log("영웅배치 좌표 범위 초과");
            }
            return -1; // 범위 초과시 -1 반환
        }
        return UserData.Hero_Array[arrayIndex];
    }
    public int CheckHeroArrayID(EHeroID heroID)     // heroID : 0(미배치)~4002(영웅ID 최대값) 
    {
        for (int i = 0; i < UserData.Hero_Array.Length; i++)
        {
            if (UserData.Hero_Array[i] == (int)heroID)
            { 
                return i; // heroID가 배치된 인덱스 반환
            }
        }
        return -1; // heroID가 배치되지 않았을 경우 -1 반환
    }

    // 영웅 Array배치 함수 x,y : 0~3, heroID : 0(미배치)~4002(영웅ID 최대값)
    public void AddUserHeroArray(int x, int y, EHeroID heroID) 
    {
        int arrayIndex = x + 4 * y ;
        int beforeIndex = -1;
        // 배열 범위 방지
        if (arrayIndex < 0 || arrayIndex >= UserData.Hero_Array.Length)
        {
            if (isDebugMode)
            {
                Debug.Log("영웅배치 좌표 범위 초과");
            }
            return;
        }
        // ID 0 입력시 구역 미배치 처리
        if((int)heroID == 0)
        {
            UserData.Hero_Array[arrayIndex] = 0;
            SaveUserData(); return;
        }
        // 기존 배치 중복 여부 확인
        for(int i = 0; i < UserData.Hero_Array.Length; i++)
        {
            if (UserData.Hero_Array[i] == (int)heroID)
            {
                beforeIndex = i;
                break;
            }
        }
        // 기존 배치 제거
        if (beforeIndex != -1 && UserData.Hero_Array[arrayIndex] != 0)
        {
            UserData.Hero_Array[beforeIndex] = UserData.Hero_Array[arrayIndex];
        }
        else if (beforeIndex != -1 && UserData.Hero_Array[arrayIndex] == 0)
        {
            UserData.Hero_Array[beforeIndex] = 0;
        }
            // 새로운 배치 설정
            UserData.Hero_Array[arrayIndex] = (int)heroID;

        SaveUserData(); return;
    }



    public UserHeroData GetHeroData(EHeroID id)
    {
        for (int i = 0; i < UserData.HeroList.Count; i++)
        {
            if (UserData.HeroList[i].HeroID == (EHeroID)id) return UserData.HeroList[i];
        }
        return null; // 없으면 null 반환
    }



    public struct FinalHeroStatus
    {
        public float HeroAtk;
        public float HeroDef;
        public float HeroHP;
        public float HeroCriticalRatio;
    }

    
    public FinalHeroStatus GetHeroFinalStatus(EHeroID heroID, UnitDataSO unitSO)
    {
        // 유저의 강화/레벨 데이터 로드
        UserUpgradeStatus upgrade = GetUserUpgradeStatus();
        UserHeroData heroData = GetHeroData(heroID);
        HeroDataSO heroSO;
        heroSO = unitSO as HeroDataSO;
        if (heroSO != null)
        { 
            // Debug.Log($"영웅 데이터 조회 성공 (ID: {heroID} / 레벨: {heroData.Level})");
        }
            // 최종 스탯 저장용
            FinalHeroStatus final = new FinalHeroStatus();

        // SO수치  ((영웅 기본 수치 * 영웅 레벨당 수치 * 유저 강화 레벨 * 영웅 승급)
        final.HeroAtk = (heroSO.BaseAttackDamage * (1 + 0.01f * upgrade.UserAtkLevel) * (1 + 0.1f * heroData.Level)*(1 + 0.05f * heroData.Quantity));
        final.HeroDef = (heroSO.BaseDefense * (1 + 0.01f * upgrade.UserDefLevel) * (1 + 0.1f * heroData.Level) * (1 + 0.05f * heroData.Quantity));
        final.HeroHP =  (heroSO.BaseMaxHp * (1 + 0.01f*upgrade.UserLifeLevel) * (1 + 0.1f * heroData.Level) * (1 + 0.05f * heroData.Quantity));
        final.HeroCriticalRatio = heroSO.CriticalChance* (1 + 0.1f * heroData.Level)*(1 + 0.05f * heroData.Quantity);

        return final;
    }



    public void AddItem(int id, int count)
    {
        // 인벤토리 보유 여부 확인
        UserItemData item = UserData.Inventory.Find(x => x.ItemID == id);

        if (item != null)
        {
            // 있으면 개수만 추가
            item.Quantity += count;
        }
        else
        {
            // 없으면 새로 리스트에 추가
            UserData.Inventory.Add(new UserItemData { ItemID = id, Quantity = count });
        }
        SaveUserData();
    }

    public bool SpendItem(int id, int count)
    {
        UserItemData item = UserData.Inventory.Find(x => x.ItemID == id);

        if (item != null && item.Quantity >= count)
        {
            item.Quantity -= count;

            // 사용후 개수가 0개시 처리 List 처리 여부
            // if (item.Quantity <= 0) { 필요시 처리 코드 추가 }

            SaveUserData();
            return true;
        }

        if (isDebugMode)
        {
            Debug.Log("아이템이 부족합니다.");
        }
        return false;
    }

    // 끝나는 시간 저장
    public double GetOfflineTimeSeconds()
    {
        if (UserData.LastLogoutTime == 0) return 0;

        DateTime lastTime = new DateTime(UserData.LastLogoutTime);
        TimeSpan span = DateTime.Now - lastTime;

        // 최대 방치 가능 시간 제한 (최대 24 시간까지만 보상)
        double seconds = span.TotalSeconds;
        return Math.Min(seconds, 3600*24); 
    }

    public void AddHeroDummy(EHeroID id)
    {
        // 보유 여부 확인
        UserHeroData hero = GetHeroData(id);
        if (hero == null)
        {
            if (id == EHeroID.Elga)
            {
                UserData.HeroList.Add(new UserHeroData { HeroID = id, Quantity = 1 });
                AddUserHeroArray(0, 0, id); // 엘가를 1번 슬롯에 배치

            }
            else
            {
                UserData.HeroList.Add(new UserHeroData { HeroID = id, Quantity = 0 });

            }
            Debug.Log($"[더미] 영웅 데이터 추가 (ID: {id})");
            SaveUserData();
        }

    }

}