using System;
using System.Collections.Generic;

[Serializable]
public class CUserData
{
    // 재화 및 기본 정보
    public string UserName = "GM3_8Team_FiveKnights_Account";
    public int Gold = 0;
    public int Diamond = 0;
    public int CurrentStage = 1;

    // 보유 중인 영웅들의 레벨 정보 (영웅 ID : 레벨)
    public List<HeroSaveData> HeroList = new List<HeroSaveData>();

    // 마지막 접속 시간 (오프라인 보상 계산용)
    public string LastLogoutTime = string.Empty;
}

[Serializable]
public class HeroSaveData
{
    public int HeroID;
    public int Level;
}