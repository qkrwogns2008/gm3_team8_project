using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public class CTestQuest : MonoBehaviour
{
    void Update()
    {
        // 숫자 1을 누르면 공격력 레벨 강제 상승 (모니터링 테스트)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CDataManager.Instance.UserData.Atk_Level += 50;
            Debug.Log($"[Test] 공격력 레벨 상승! 현재: {CDataManager.Instance.UserData.Atk_Level}");

        }

        // 숫자 2를 누르면 소환 10회 이벤트 방송 (이벤트 테스트)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CDataManager.Instance.UserData.Def_Level += 50;
            Debug.Log($"[Test] 방어력 레벨 상승! 현재: {CDataManager.Instance.UserData.Def_Level}");

        }

        // 숫자 3을 누르면 10번 퀘스트(루비 테스트) 보상 받기 시도
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CQuestManager.Instance.RewardQuest(10); // ID 10번 보상 수령
        }
    }
}