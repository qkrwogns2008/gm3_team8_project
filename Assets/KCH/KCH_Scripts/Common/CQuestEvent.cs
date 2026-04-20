using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class CQuestEvent
{
    // 퀘스트 진행 수치 전달 이벤트
    public static event Action<EQuestType, int> QuestProgressAction;

    // 이벤트 함수
    public static void Publish(EQuestType type, int value)
    {
        QuestProgressAction.Invoke(type, value);
    }
}
