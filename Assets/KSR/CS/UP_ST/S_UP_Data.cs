using System;
using UnityEngine;

// =============================
// 개별 능력치 데이터 (스테이지 제거됨)

[Serializable]
public class StatData
{
    public int currentLevel;   // 현재 레벨
    public float currentStat;  // 전체 누적 능력치
    public float stageStat;    // 현재 단계 표시용 능력치
    public float currentCost;  // 현재 비용
}

// =============================
// 전체 능력치 데이터 + 공통 스테이지 추가

[Serializable]
public class S_UP_Data
{
    public int currentStage; // 전체 공통 스테이지 (단일 관리)

    public StatData Attack = new StatData();   // 공격 데이터
    public StatData Defense = new StatData();  // 방어 데이터
    public StatData HP = new StatData();       // 체력 데이터
}