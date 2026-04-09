using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CGachaModel : MonoBehaviour
{

    [Header("보유 재화(임시)")]
    public int RubyCount = 30000;               // 현재 루비
    public int TicketCount = 30;                // 현재 소환권

    private CGachaCategorySO _currentCategory;

    private System.Random GachaRandom;

    public void ResetSeed(int seed)
    {
        GachaRandom = new System.Random(seed);
    }

    // 카테고리 세팅
    public void SetCategory(CGachaCategorySO category)
    {
        _currentCategory = category;

        GachaRandom = new System.Random(12345);
    }

    // 가차 실행 함수
    public List<CGachaDataSO> RollGacha(int count)
    {
        // 결과 값 리스트
        List<CGachaDataSO> result = new List<CGachaDataSO>();

        // 카운트 횟수
        for (int i = 0; i < count; i++)
        {
            // 횟수 만큼 실행후 값 추가
            result.Add(_currentCategory.GachaTable.WeightRandomGacha(GachaRandom));
        }

        return result;
    }

    // 카테고리에 현재 경험치 증가 함수
    public void AddExp(int amount)
    {
        if (_currentCategory != null)
        {
            _currentCategory.AddExp(amount);
        }

    }
    public bool CheckRuby(int count)
    {
        int needRuby = count * 100;

        if (TicketCount >= count)
        {
            return true;
        }

        if (RubyCount >= needRuby)
        {
            return true;
        }

        return false;
    }

    public void PayRuby(int count)
    {
        if (TicketCount >= count) 
        {
            TicketCount -= count;
        }

        else
        {
            RubyCount -= (count * 100);
        }
    }
}
