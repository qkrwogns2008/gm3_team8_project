using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1. SO 데이터들을 List에 담고 한번에 관리하는 SO
/// 2. 가중치 랜덤 로직 함수
/// </summary>

[System.Serializable]
public class CRarityWeight
{
    public float Normal;            // 등급별 뽑기 확륙 가중치 Normal
    public float Rare;              // 등급별 뽑기 확륙 가중치 Rare
    public float Epic;              // 등급별 뽑기 확륙 가중치 Epic
    public float Unique;            // 등급별 뽑기 확륙 가중치 Unique
    public float Legend;            // 등급별 뽑기 확륙 가중치 Legend

    // 가중치들의 배열
    public float[] WeightArray()
    {
        return new float[] { Normal, Rare, Epic, Unique, Legend };
    }      
}

[CreateAssetMenu(menuName = "Gacha/GachaTable", fileName = "GachaTable_")]
public class CGachaTableSO : ScriptableObject
{
    [Header("소환 대상 리스트")]
    public List<CGachaDataSO> GachaList = new List<CGachaDataSO>();

    [Header("레벨별 등급 가중치 리스트")]
    public List<CRarityWeight> RarityWeightList = new List<CRarityWeight>();

    // 등급별로 데이터 딕셔너리 캐싱
    private Dictionary<CGachaDataSO.ERarity, List<CGachaDataSO>> _rarityGroups;

    public void BuildRarityMaps()
    {
        if (_rarityGroups != null)
        {
            return;
        }

        _rarityGroups = new Dictionary<CGachaDataSO.ERarity, List<CGachaDataSO>>();

        for (int i = 0; i < GachaList.Count; i++)
        {
            CGachaDataSO data = GachaList[i];

            if (_rarityGroups.ContainsKey(data.Rarity) == false)
            {
                _rarityGroups[data.Rarity] = new List<CGachaDataSO>();
            }

            _rarityGroups[data.Rarity].Add(data);
        }
    }

    public CGachaDataSO WeightRandomGacha(int level, System.Random randomSeed)
    {
        // 리스트가 없거나 0이면 null반환
        if (GachaList == null || GachaList.Count == 0)
        {
            return null;
        }

        BuildRarityMaps();

        // 현재 레벨에 등급별 확률 부여
        int levelIndex = Mathf.Clamp(level - 1, 0, RarityWeightList.Count - 1);
        CRarityWeight currentWeights = RarityWeightList[levelIndex];

        // 랜덤 등급 선정
        CGachaDataSO.ERarity selectedRarity = RollRarity(currentWeights, randomSeed);

        // 해당 등급에서 한명 선정
        List<CGachaDataSO> selectList = _rarityGroups[selectedRarity];

        // 해당 등급이 없으면 노말
        if (selectList.Count == 0)
        {
            selectList = _rarityGroups[CGachaDataSO.ERarity.Normal];
        }

        // 모든 가중치의 합 변수
        float totalWeight = 0f;

        // 가중치의 합계
        for (int i = 0; i < selectList.Count; i++)
        {
            totalWeight += selectList[i].Weight;
        }

        // 랜덤 숫자 선정
        float randomValue = (float)(randomSeed.NextDouble() * totalWeight);

        // 누적 가중치 변수
        float currentWegiht = 0f;

        for (int i = 0; i < selectList.Count; i++)
        {
            // 현재 누적 가중치의 합계
            currentWegiht += selectList[i].Weight;

            // 크거나 같으면 당첨
            if (randomValue <= currentWegiht)
            {
                // 당첨값 리턴
                return selectList[i];
            }
        }

        // 혹시 모를 값 그냥 노멀 등급 리턴
        return selectList[0];
    }

    private CGachaDataSO.ERarity RollRarity(CRarityWeight weights, System.Random randomSeed)
    {
        float[] weightArray = weights.WeightArray();
        float total = 0;

        for (int i = 0;i < weightArray.Length;i++)
        {
            total += weightArray[i];
        }

        float randomValue = (float)(randomSeed.NextDouble() * total);
        float currentWegiht = 0;

        for (int i = 0; i < weightArray.Length; i++)
        {
            // 현재 누적 가중치의 합계
            currentWegiht += weightArray[i];

            // 크거나 같으면 당첨
            if (randomValue <= currentWegiht)
            {
                // 당첨값 리턴
                return (CGachaDataSO.ERarity)i;
            }
        }

        // 혹시 모를 값 그냥 노멀 등급 리턴
        return CGachaDataSO.ERarity.Normal;
    }
}
