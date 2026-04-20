using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/RewardData", fileName = "RewardData_")]
public class CRewardDataSO : ScriptableObject
{
    [Serializable]
    public struct RewardData
    {
        public EQuestReward Type;           // 보상 타입
        public Sprite Icon;                 // 보상 아이콘
        public string Name;                 // 보상 이름
        public Sprite Background;           // 보상 배경
        public Sprite Outline;              // 보상 테두리
    }

    public List<RewardData> data;           // 보상 리스트

    // 아이콘 변경 함수
    public Sprite GetIcon(EQuestReward reward)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Type == reward)
            {
                return data[i].Icon;
            }
        }
        return null;
    }

    // 데이터를 통째로 가져오는 함수
    public RewardData GetRewardData(EQuestReward type)
    {
        for (int i = 0; i<data.Count; i++)
        {
            if (data[i].Type == type)
            {
                return data[i];
            }
        }
        return default;
    }
}

