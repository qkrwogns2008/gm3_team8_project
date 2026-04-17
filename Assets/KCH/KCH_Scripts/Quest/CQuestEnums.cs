using System;
using System.Collections.Generic;
using UnityEngine;

// Quest Enum을 관리하는 스크립트
public enum EQuestType
{
    GachaSummon = 0,            // 영웅 소환
    PetSummon = 1,              // 펫소환
    StateUp = 2,                // 스테이지 업
    UseItem = 3,                // 아이템 사용
    AtkLevel = 4,               // 공격력 영향력 레벨업 
    DefLevel = 5,               // 방어력 영향력 레벨업 
    LifeLevel = 6,              // 생명력 영향력 레벨업 
    HeroLevel = 7,              // 영웅 레벨업
    DungeonClear = 8,           // 던전 클리어
}

public enum EQuestReward
{
    Ruby = 0,                   // 루비 보상
    Ticket = 1,                 // 티켓 보상
    Exp = 2,                    // 경험치 보상
}

public enum EQuestState
{
    Progress,                   // 진행중
    Clear,                      // 받기
}

[Serializable]
public struct SQuestReward
{
    public EQuestReward Type;   // 퀘스트 보상 타입
    public int Amount;          // 퀘스트 보상 수량
}
