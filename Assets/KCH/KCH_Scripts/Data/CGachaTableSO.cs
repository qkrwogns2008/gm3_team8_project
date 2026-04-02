using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1. SO 데이터들을 List에 담고 한번에 관리하는 SO
/// 2. 가중치 랜덤 로직 함수
/// </summary>
 
[CreateAssetMenu(menuName = "Gacha/GachaTable", fileName = "GachaTable_")]
public class CGachaTableSO : ScriptableObject
{
    [Header("소환 대상 리스트")]
    public List<CGachaDataSO> GachaList = new List<CGachaDataSO>();

    public CGachaDataSO WeightRandomGacha()
    {
        // 리스트가 없거나 0이면 null반환
        if (GachaList == null || GachaList.Count == 0)
        {
            return null;
        }

        // 모든 가중치의 합 변수
        float totalWeight = 0f;

        // 가중치의 합계
        for (int i = 0; i < GachaList.Count; i++)
        {
            totalWeight += GachaList[i].Weight;
        }

        // 랜덤 숫자 선정
        float randomValue = Random.Range(0, totalWeight);
        // 누적 가중치 변수
        float currentWegiht = 0f;

        for (int i = 0; i < GachaList.Count; i++)
        {
            // 현재 누적 가중치의 합계
            currentWegiht += GachaList[i].Weight;

            // 크거나 같으면 당첨
            if (randomValue <= currentWegiht)
            {
                // 당첨값 리턴
                return GachaList[i];
            }
        }

        // 혹시 모를 값 그냥 노멀 등급 리턴
        return GachaList[0];
    }
}
