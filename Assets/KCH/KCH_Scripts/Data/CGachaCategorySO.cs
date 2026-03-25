using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Gacha/GachaCategory", fileName ="GachaCategory_")]
public class CGachaCategorySO : ScriptableObject
{
    [Header("카테고리 설정")]
    public string _categoryName;
    public Sprite _mainShopImage;

    [Header("소환 테이블 연결")]
    public CGachaTableSO _gachaTable;           // TableSO 연결

    [Header("소환 레벨과 경험치 설정")]
    public int _currentLevel = 1;               // 소환 레벨
    public int _currentExp = 0;                 // 소환 경험치

    // 레벨업 경험치 테이블 = 1: 100, 2: 300, 3: 1000, 4: 2500, 5: 5000, 6: 10000, 7: 15000, 8:25000, 9: 35000
    public int[] _maxExpTable = { 100, 300, 1000, 2500, 5000, 10000, 15000, 25000, 35000};                   

    public void AddExp(int amount)
    {
        _currentExp += amount;

        // 현재 레벨이 최대 레벨이 같거나 크면 레벨업, 최대 레벨보다 작을 때만 레벨업
        while (_currentExp >= _maxExpTable[_currentLevel - 1] && _currentLevel < _maxExpTable.Length)
        {
            // 누적 경험치
            _currentExp -= _maxExpTable[_currentLevel - 1];
            // 레벨 업
            _currentLevel++;
        }
    }
}
