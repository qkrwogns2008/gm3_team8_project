using System;
using System.IO;
using UnityEngine;

public class CDataManager : MonoBehaviour
{
    public static CDataManager Instance { get; private set; }

    public CUserData UserData { get; private set; }

    private string _savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            #if UNITY_EDITOR
            // 유니티 에디터(PC 작업 중)일 때는 프로젝트 폴더 바로 옆에 저장 (찾기 쉬움)
            // _savePath = Path.Combine(Application.dataPath, "..", "SaveData.json");
            #else
            // 실제 빌드(모바일, PC 빌드본)일 때는 안전한 영구 저장소에 저장
            _savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
            #endif
            LoadUserData();
        }
        else { Destroy(gameObject); }
    }
    // [저장] 데이터 -> JSON -> 파
    // 일
    public void SaveUserData()
    {
        UserData.LastLogoutTime = System.DateTime.Now.Ticks;
        string json = JsonUtility.ToJson(UserData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"저장 완료: {_savePath}");
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

    // gold 추가
    public void AddGold(int amount)       // 승래님과 check
    {
        UserData.Gold += amount;
        SaveUserData();
        Debug.Log($"골드 추가: {amount} / 현재 골드: {UserData.Gold}");
    }

    // gold 사용
    public bool SpendGold(int amount)       // 승래님과 check
    {
        if (UserData.Gold >= amount)
        {
            UserData.Gold -= amount;
            SaveUserData();
            return true;
        }

        Debug.Log($"골드가 부족합니다. 필요 골드: {amount}");
        return false;
    }
    

    // 공격력 강화
    public void UpgradeBaseDamage(int amount)
    {
        UserData.Atk_Level += amount;
        Debug.Log($"[강화] 기본 공격력 상승 현재: {UserData.Atk_Level}");
        SaveUserData();
    }

    // 방어력 강화
    public void UpgradeBaseShield(int amount)
    {
        UserData.Def_Level += amount;
        Debug.Log($"[강화] 기본 방어력 상승 현재: {UserData.Def_Level}");
        SaveUserData();
    }

    // 체력 강화
    public void UpgradeBaseHP(int amount)
    {
        UserData.Life_Level += amount;
        Debug.Log($"[강화] 기본 체력 상승 현재: {UserData.Life_Level}");
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
    public void AddHeroData(int id)
    {
        // 보유 여부 확인
        var hero = GetHeroData(id);

        if (hero != null)
        {
            // 이미 보유 중
            hero.Level++;
            Debug.Log($"이미 보유 중인 영웅입니다. 레벨업! (ID: {id} / 현재 레벨: {hero.Level})");
        }
        else
        {
            // 신규 획득
            UserData.HeroList.Add(new UserHeroData { HeroID = id, Level = 1 });
            Debug.Log($"새로운 영웅 획득! (ID: {id})");
        }

        SaveUserData();
    }

    // 영웅 데이터 조회
    

    // hero 레벨 설정
    public void SetHeroLevel(int id, int level)     // 우재님과 check
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
            Debug.Log($"User가 보유하지 않은 영웅(ID: {id})의 레벨설정 불가.");
        }
        SaveUserData();
    }
    public void AddHeroLevel(int id, int level)     // 우재님과 check
    {
        var hero = GetHeroData(id);
        // 영웅 보유 여부 확인
        if (hero != null)
        {
            hero.Level += level;
        }
        else
        {
            // 영웅 미보유시
            Debug.Log($"User가 보유하지 않은 영웅(ID: {id})의 레벨설정 불가.");
        }
        SaveUserData();
    }

    public UserHeroData GetHeroData(int id)         // 우재님과 check
    {
        for (int i = 0; i < UserData.HeroList.Count; i++)
        {
            if (UserData.HeroList[i].HeroID == id) return UserData.HeroList[i];
        }
        return null; // 없으면 null 반환
    }



    public struct FinalHeroStatus
    {
        public float HeroAtk;
        public float HeroDef;
        public float HeroHP;
    }

    // 팀원들이 이 함수만 부르면 팀장님의 공식으로 계산된 결과가 나갑니다.
    public FinalHeroStatus GetHeroFinalStatus(int heroID, UnitDataSO unitSO)
    {
        // 유저의 강화/레벨 데이터 로드
        UserUpgradeStatus upgrade = GetUserUpgradeStatus();
        UserHeroData heroData = GetHeroData(heroID);
        HeroDataSO heroSO;
        heroSO = unitSO as HeroDataSO;
        if (heroSO != null)
        { 
            Debug.Log($"영웅 데이터 조회 성공 (ID: {heroID} / 레벨: {heroData.Level})");
        }
            // 최종 스탯 저장용
            FinalHeroStatus final = new FinalHeroStatus();

        // SO수치  ((영웅 기본 수치 + 영웅 레벨당 수치 * 유저 강화 레벨 )* 영웅 레벨)
        //final.HeroAtk = (heroSO.BaseAtk + heroSO.AtkPerLevel * upgrade.UserAtkLevel) * (1 + 0.1f * heroData.Level);
        //final.HeroDef = (heroSO.BaseDefense + heroSO.DefensePerLevel * upgrade.UserDefLevel) * (1 + 0.1f * heroData.Level);
        //final.HeroHP =  (heroSO.BaseHP + heroSO.HPPerLevel * upgrade.UserLifeLevel) * (1 + 0.1f * heroData.Level);

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

        Debug.Log("아이템이 부족합니다.");
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


}