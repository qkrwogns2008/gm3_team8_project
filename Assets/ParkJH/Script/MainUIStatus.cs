using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using static CDataManager;

public class MainUIStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TeamForceText;
    [SerializeField] private TextMeshProUGUI _DiamondText;
    [SerializeField] private TextMeshProUGUI _RubyText;

    EHeroID heroID;
    UnitDataSO unitSO;

    void Start()
    {

    }
    void Update()
    {
        GetMainUIDataInText();
    }



    public void GetMainUIDataInText()
    {
        List<EHeroID> HeroID = new List<EHeroID>();
        List<FinalHeroStatus> heroStatus = new List<FinalHeroStatus>();
        UserHeroData userHeroData;
        for (int i = 0; i < 4; i++)
        {
            for (int e = 0; e < 4; e++)
            {
                int num = CDataManager.Instance.CheckHeroArray(e, i);
                if(num != 0)
                {
                    HeroID.Add((EHeroID)num); ;
                }
            }
        }
        for(int i = 0; i < HeroID.Count; i++)
        {
            //UnitDataSO UnitSO = CDataManager.Instance.GetUnitDataSO(HeroID[i]);
            //heroStatus.Add(CDataManager.Instance.GetHeroFinalStatus(HeroID[i],UnitSO));
        }

        // 전투력
        if (_TeamForceText != null)
        {

        }
            
        // 다이아
        if (_DiamondText != null)
            _DiamondText.text = CDataManager.Instance.UserData.Diamond.ToString("N0");
        // 루비
        if (_RubyText != null)
            _RubyText.text = CDataManager.Instance.UserData.Ruby.ToString("N0");
        /*
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
        // 경험치 요구량
        if (_expText != null)
            _expText.text = (userHeroData.Level * 10f).ToString("");
        // 경험치 보유량
        if (_userExpText != null)
            _userExpText.text = (CDataManager.Instance.UserData.expPoint).ToString("");
        */
    }
}
