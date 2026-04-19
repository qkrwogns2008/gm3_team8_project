using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStageNotificationUI : MonoBehaviour
{
    [SerializeField] private StageNotificationUI _stageUI;
    public void OnClickStageMoveStageNotificationUIOn()
    {
        _stageUI.StageNotificationUIOn();
    }
    
}
