using System;
using System.Collections.Generic;
using UnityEngine;

// Quest Enum을 관리하는 스크립트
public enum EQuestType
{
    None,                   // 디폴트
    GachaSummon = 1,        // 영웅 소환
    StateUp = 2,            // 스테이지 업
    UseItem = 3,            // 아이템 사용
    Atk_Level = 4,          // 공격력 영향력 레벨업 
    Def_Level = 5,          // 방어력 영향력 레벨업 
    Life_Level = 6,         // 생명력 영향력 레벨업 
}

public enum EQuestState
{
    Progress,               // 진행중
    Clear,                  // 받기
}
