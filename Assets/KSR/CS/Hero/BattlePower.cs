using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattlePowerCalculator : MonoBehaviour
{
    [Header("영웅 데이터")]
    public UnitDataSO[] heroDatabase;

    [Header("출력 텍스트 리스트")]
    public List<TMP_Text> powerTexts = new List<TMP_Text>(); // 여러 텍스트

    void Update()
    {
        float totalPower = GetTotalPower();
        string formatted = FormatNumber(totalPower);

        // 모든 텍스트에 동일 값 출력
        for (int i = 0; i < powerTexts.Count; i++)
        {
            if (powerTexts[i] == null) continue;

            powerTexts[i].text = formatted;
        }
    }

    // =============================
    // 전체 전투력 계산

    float GetTotalPower()
    {
        if (CDataManager.Instance == null) return 0;

        int[] heroArray = CDataManager.Instance.UserData.Hero_Array;

        float total = 0f;

        for (int i = 0; i < heroArray.Length; i++)
        {
            int id = heroArray[i];
            if (id == 0) continue;

            EHeroID heroID = (EHeroID)id;

            UnitDataSO unitSO = GetUnitData(heroID);
            if (unitSO == null) continue;

            var stat = CDataManager.Instance.GetHeroFinalStatus(heroID, unitSO);

            float power = stat.HeroAtk + stat.HeroDef + stat.HeroHP;

            total += power;
        }

        return total;
    }

    // =============================
    // heroID → SO 찾기

    UnitDataSO GetUnitData(EHeroID id)
    {
        for (int i = 0; i < heroDatabase.Length; i++)
        {
            HeroDataSO heroSO = heroDatabase[i] as HeroDataSO;

            if (heroSO != null && heroSO.HeroID == id)
                return heroDatabase[i];
        }

        return null;
    }

    // =============================
    // 숫자 포맷 (k/m + 4자리)

    string FormatNumber(float value)
    {
        if (value >= 1000000f)
        {
            float v = value / 1000000f;
            return LimitDigits(v) + "m";
        }
        else if (value >= 1000f)
        {
            float v = value / 1000f;
            return LimitDigits(v) + "k";
        }
        else
        {
            return LimitDigits(value);
        }
    }

    // =============================
    // 최대 4자리 유지

    string LimitDigits(float value)
    {
        if (value >= 1000f)
            return value.ToString("F0");
        else if (value >= 100f)
            return value.ToString("F1");
        else if (value >= 10f)
            return value.ToString("F2");
        else
            return value.ToString("F3");
    }
}