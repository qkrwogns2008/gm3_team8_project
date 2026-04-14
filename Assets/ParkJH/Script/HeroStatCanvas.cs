using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using static CDataManager;

public class HeroStatCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _armorText;
    [SerializeField] private TextMeshProUGUI _criticalRatioText;
    [SerializeField] private TextMeshProUGUI _attackSpeedText;
    [SerializeField] private TextMeshProUGUI _criticalDamageText;
    [SerializeField] private TextMeshProUGUI _moveSpeedText;
    [SerializeField] private TextMeshProUGUI _reduceRatioText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _rankText;

    [Header("영웅 SO 데이터")]
    [SerializeField] private HeroDataSO _heroDataSO;

    void Start()
    {
        
    }
	void Update()
    {
        
    }



    public void GetHeroDataInText( HeroDataSO heroSO)
    {
        EHeroID HeroID;
        HeroID = heroSO.HeroID;
        UserHeroData userHeroData;
        userHeroData = CDataManager.Instance.GetHeroData(HeroID);
        FinalHeroStatus finalStats = CDataManager.Instance.GetHeroFinalStatus(HeroID, heroSO);

        // 채력
        if (_hpText != null)
            _hpText.text = finalStats.HeroHP.ToString("N0");
        // 공격력
        if (_attackText != null)
            _attackText.text = finalStats.HeroAtk.ToString("N0");
        // 방어력
        if (_armorText != null)
            _armorText.text = finalStats.HeroDef.ToString("N0");
        // 치명타 확률
        if (_criticalRatioText != null)
            _criticalRatioText.text = (finalStats.HeroCriticalRatio).ToString("F1") + "%";
        // 공격 속도 (SO 기준 공속 Multiplier 으로 책정)
        if (_attackSpeedText != null)
            _attackSpeedText.text = (heroSO.AttackSpeedMultiplier).ToString("F1");
        // 치명타 피해량 (확인후 책정하기)
        if (_criticalDamageText != null)
            _criticalDamageText.text = (200f).ToString("F1") + "%";
        // 이동 속도 (SO 기준 Multiplier 으로 책정)
        if (_moveSpeedText != null)
            _moveSpeedText.text = (heroSO.BaseMoveSpeed).ToString("F1");
        // 피해 감소율 항상 0%으로 책정
        if (_reduceRatioText != null)
            _reduceRatioText.text = (heroSO.DamageReductionChance).ToString("") + "%";
        // 레벨
        if (_levelText != null)
            _levelText.text = (userHeroData.Level).ToString("");
        // 랭크
        if (_rankText != null)
            _rankText.text = (userHeroData.Quantity).ToString("");

    }
}
