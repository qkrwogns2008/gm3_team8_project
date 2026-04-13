using System;
using System.Collections.Generic;

[Serializable]
public class CUserData
{
    // 기본 유저 세팅
    public float BGMVolume = 1;
    public float SFXVolume = 1;
    public float UIVolume = 1;

    // 재화 및 기본 정보
    public string UserName = "GM3_8Team_FiveKnights_Account";
    public int UserLevel = 100;
    // 재화 관련 함수
    public int Gold = 10000;
    public int Diamond = 10000; // 현금 재화
    public int goldDungeonTicket = 5;
    public int expDungeonTicket = 5;
    public int expPoint = 10000;
    // 픽업 관련 함수
    public int Ruby = 10000;
    public int PickUpTicket = 10000;
    public int HeroPickUpLevel = 0;
    public int PetPickUpLevel = 0;
    // 스테이지 관련 함수
    public int MainStageLevel = 1;
    public int CurrentStageLevel = 1;
    public int GoldDungeonLevel = 1;
    public int EXPDungeonLevel = 1;
    // 강화 UI 관련 함수
    public int Atk_Level = 1; // ATK
    public int Atk_Stat = 1;
    public int Atk_Cost = 1;
    public int Def_Level = 1;
    public int Def_Stat = 1;
    public int Def_Cost = 1;
    public int Life_Level = 1;
    public int Life_Stat = 1;
    public int Life_Cost = 1;
    //영웅 배치 관련 함수
    public int[] Hero_Array = new int [16];


    // 보유 중인 영웅ID, 레벨 정보
    public List<UserHeroData> HeroList = new List<UserHeroData>();
    // 보유 중인 아이템ID, 개수 정보
    public List<UserItemData> Inventory = new List<UserItemData>();
    // 퀘스트 정보창
    public List<UserQuestData> QuestList = new List<UserQuestData>();
    // 마지막 접속 시간
    public long LastLogoutTime = 0;
}

[Serializable]
public class UserHeroData
{
    public EHeroID HeroID;  // 영웅 고유 번호
    public int Level;   // 영웅 레벨
    public int Quantity;   // 보유 개수
}

[Serializable]
public class UserItemData
{
    public int ItemID;     // 아이템 고유 번호
    public int Quantity;   // 보유 개수
}


[Serializable]
public class UserQuestData
{
    public int QuestID;
    public int CurrentGague;
    public int ReewardCount;
}
