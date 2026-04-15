using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CGachaModel : MonoBehaviour
{


    public int RubyCount => CDataManager.Instance.UserData.Ruby;                // 현재 루비
    public int TicketCount => CDataManager.Instance.UserData.PickUpTicket;      // 현재 소환권

    private CGachaCategorySO _currentCategory;

    private System.Random GachaRandom;                                          // 시스템 랜덤 생성

    // 시드 리셋 기능 함수
    public void ResetSeed(int seed)
    {
        GachaRandom = new System.Random(seed);
    }

    // 카테고리 세팅
    public void SetCategory(CGachaCategorySO category)
    {
        _currentCategory = category;

        // 랜덤 시드 값
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
            CGachaDataSO data = _currentCategory.GachaTable.WeightRandomGacha(GachaRandom);

            // 횟수 만큼 실행후 값 추가
            result.Add(data);

            CDataManager.Instance.AddHeroData(data.HeroID);
        }

        CDataManager.Instance.SaveUserData();
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
        var userData = CDataManager.Instance.UserData;

        int needRuby = count * 100;

        return (userData.PickUpTicket >= count) || (userData.Ruby >= needRuby);
    }

    public void PayRuby(int count)
    {
        var userData = CDataManager.Instance;

        if (userData.SpendPickUpTicket(count))
        {
            return;
        }

        int needRuby = count * 100;

        if (userData.SpendRubby(needRuby))
        {
            Debug.Log($"{needRuby} 루비 결제 완료");
        }

        else
        {
            Debug.Log("결제 실패 : 재화 부족");
        }
    }
}
