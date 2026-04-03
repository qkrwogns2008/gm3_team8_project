using UnityEngine;

// Quest Enum을 관리하는 스크립트
public enum EQuestType
{
    None,                   // 디폴트
    GachaSummon = 1,        // 영웅 소환
    MonsterKill = 2,        // 몬스터 처치
    UseItem = 3,            // 아이템 사용
}

public enum EQuestState
{
    Progress,               // 진행중
    Clear,                  // 받기
}
