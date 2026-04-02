using System;
using System.Collections.Generic;

[Serializable]
public class CUserData 
{
    // 재화 및 기본 정보
    public string UserName = "GM3_8Team_FiveKnights_Account";
    public int UserLevel = 0;
    public int Gold = 0;
    public int Diamond = 0;
    public int PickUpTicket = 0;
    public int CurrentStage = 1;
    public int BaseDamage = 1;
    public int Baseshield = 1;
    public int BaseHP = 1;

    // 보유 중인 영웅ID, 레벨 정보
    public List<UserHeroData> HeroList = new List<UserHeroData>();
    // 보유 중인 아이템ID, 개수 정보
    public List<UserItemData> Inventory = new List<UserItemData>();

    // 마지막 접속 시간
    public long LastLogoutTime = 0;
}

[Serializable]
public class UserHeroData
{
    public int HeroID;  // 영웅 고유 번호
    public int Level;   // 영웅 레벨
}

[Serializable]
public class UserItemData
{
    public int ItemID;     // 아이템 고유 번호
    public int Quantity;   // 보유 개수
}