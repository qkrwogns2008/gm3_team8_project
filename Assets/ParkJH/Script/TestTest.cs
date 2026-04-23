using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TestTest : MonoBehaviour
{
    IEnumerator Start()
    {

        int stage = CDataManager.Instance.UserData.CurrentStageLevel;
        string stageName;
        yield return null; // 한 프레임 대기 (모든 Awake/Start 완료 보장)
        if (CDataManager.Instance.UserData.CurrentStageLevel >= 41)
        {
            stageName = "복수자의 지옥";
        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 21)
        {
            stageName = "눈보라의 대지";
        }
        else
        {
            stageName = "신비의 숲";
        }
        string text1 = "스테이지  " + stage;
        string text2 = stageName;
        MainNotification.Instance.StartMainNotification(text1, text2);
    }
    
}
