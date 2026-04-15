using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Gacha/GachaCategory", fileName ="GachaCategory_")]
public class CGachaCategorySO : ScriptableObject
{
    [Header("카테고리 설정")]
    public string CategoryName;                 // 카테고리 이름
    public Sprite MainShopImage;                // 상점 메인 이미지

    [Header("소환 테이블 연결")]
    public CGachaTableSO GachaTable;            // TableSO 연결

    // 레벨업 경험치 테이블 = 1: 100, 2: 300, 3: 1000, 4: 2500, 5: 5000, 6: 10000, 7: 15000, 8:25000, 9: 35000
    public int[] _maxExpTable = { 100, 300, 1000, 2500, 5000, 10000, 15000, 25000, 35000};                   

    // 레벨 표시
    public int GetLevel(int totalExp)
    {
        int level = 1;
        int tempExp = totalExp;

        for (int i = 0; i < _maxExpTable.Length; i++)
        {
            if (tempExp >= _maxExpTable[i])
            {
                tempExp -= _maxExpTable[i];
                level++;
            }

            else
            {
                break;
            }

        }
        return Mathf.Min(level, _maxExpTable.Length + 1);
    }

    // 현재 경험치
    public int GetCurrentExp(int totalExp)
    {
        int tempExp = totalExp;

        for (int i = 0; i < _maxExpTable.Length; i++)
        {
            if (tempExp >= _maxExpTable[i])
            {
                tempExp -= _maxExpTable[i];
            }

            else break;
        }

        return tempExp;
    }

}
