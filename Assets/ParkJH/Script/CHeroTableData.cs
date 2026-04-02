using System;
using UnityEngine;
using static CDataManager;

[Serializable] 
public class HeroTableData
{
    [Header("ID, 이름")]
    public int HeroID;
    public string Name;
    [Header("기본Base 수치")]
    public float BaseAtk;
    public int BaseHP;
    public float BaseHPRegen;
    public int BaseDefense;
    [Header("성장 수치")]
    public float AtkPerLevel;
    public int HPPerLevel;
    public float HPRegenPerLevel;
    public int DefensePerLevel;
    // 최종 수치를 담는 상자

    public struct FinalHeroStatus
    {
        public float HeroAtk;
        public float HeroDef;
        public float HeroHP;
    }

    // 모든 강화 수치가 적용된 최종 데이터를 계산해서 반환하는 함수
    public FinalHeroStatus GetFinalData()
    {
        // 유저의 강화 레벨 가져오기
        UserUpgradeStatus upgrade = CDataManager.Instance.GetUserUpgradeStatus();
        UserHeroData heroData = CDataManager.Instance.GetHeroData(HeroID);

        // 최종 스탯
        FinalHeroStatus final = new FinalHeroStatus();

        // 계산식 적용 // 영웅승급, 계정
        final.HeroAtk = (BaseAtk + AtkPerLevel * upgrade.UserAtkLevel) * (1 + 0.1f*heroData.Level);
        final.HeroDef = (BaseDefense + DefensePerLevel * upgrade.UserDefLevel) * (1 + 0.1f * heroData.Level);
        final.HeroHP = (BaseHP + HPPerLevel * upgrade.UserLifeLevel) * (1 + 0.1f * heroData.Level);
        return final;
    }
}