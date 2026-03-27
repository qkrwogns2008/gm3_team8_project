using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CGachaModel : MonoBehaviour
{

    [Header("보유 재화(임시)")]
    public int _rubyCount = 30000;         // 현재 루비
    public int _summonCardCount = 300;      // 현재 소환권

    private CGachaCategorySO _currentCategory;

    // 카테고리 세팅
    public void SetCategory(CGachaCategorySO category)
    {
        _currentCategory = category;
    }

    public List<CGachaDataSO> RollGacha(int count)
    {
        // 결과 값 리스트
        List<CGachaDataSO> result = new List<CGachaDataSO>();

        // 카운트 횟수
        for (int i = 0; i < count; i++)
        {
            // 횟수 만큼 실행후 값 추가
            result.Add(_currentCategory._gachaTable.WeightRandomGacha());
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
    public bool CheckMoney(int count)
    {
        if (_summonCardCount >= count || _rubyCount >= count * 100)
        {
            return true;
        }

        return false;
    }

    public void PayMoney(int count)
    {
        if (_summonCardCount >= count) 
        {
            _summonCardCount -= count;
        }

        else
        {
            _rubyCount -= (count * 100);
        }
    }
}
