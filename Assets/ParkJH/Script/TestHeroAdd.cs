using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestHeroAdd : MonoBehaviour
{
    
    IEnumerator Start()
    {
        yield return null; // 한 프레임 대기 (모든 Awake/Start 완료 보장)
        /*
        CDataManager.Instance.AddUserHeroArray(0, 0, EHeroID.Evan);
        CDataManager.Instance.AddUserHeroArray(0, 3, EHeroID.Teo);
        CDataManager.Instance.AddUserHeroArray(1, 1, EHeroID.Radgrid);
        CDataManager.Instance.AddUserHeroArray(1, 2, EHeroID.Shane);
        CDataManager.Instance.AddUserHeroArray(1, 3, EHeroID.Sarah);
        CDataManager.Instance.AddUserHeroArray(3, 3, EHeroID.Elga);
        */
        /*
        CDataManager.Instance.AddHeroDummy(EHeroID.Baskin);
        CDataManager.Instance.AddHeroDummy(EHeroID.Nami);
        CDataManager.Instance.AddHeroDummy(EHeroID.Loto);
        CDataManager.Instance.AddHeroDummy(EHeroID.Jak);
        CDataManager.Instance.AddHeroDummy(EHeroID.Sarah);
        CDataManager.Instance.AddHeroDummy(EHeroID.Elga);
        CDataManager.Instance.AddHeroDummy(EHeroID.Karon);
        CDataManager.Instance.AddHeroDummy(EHeroID.Rook);
        CDataManager.Instance.AddHeroDummy(EHeroID.Snipper);
        CDataManager.Instance.AddHeroDummy(EHeroID.Shane);
        CDataManager.Instance.AddHeroDummy(EHeroID.Evan);
        CDataManager.Instance.AddHeroDummy(EHeroID.Alice);
        CDataManager.Instance.AddHeroDummy(EHeroID.Teo);
        CDataManager.Instance.AddHeroDummy(EHeroID.Yeonhee);
        CDataManager.Instance.AddHeroDummy(EHeroID.Radgrid);
        CDataManager.Instance.AddHeroDummy(EHeroID.Ecila);
        */
        CGameManager.Instance.CurrentState = GameState.MainStage; // 게임 상태를 MainStage로 설정
        MainStageController.Instance.SetMainStageTheme();
        CDataManager.Instance.UserData.IsHeroArrayChanged = false; // 변경 플래그 초기화
        CDataManager.Instance.SaveUserData(); // 데이터 저장
        Debug.Log("TestHeroAdd작동 완료");
        
        if (CDataManager.Instance.CheckHeroArray(CDataManager.Instance.UserData.Hero_Camera_Array % 4, CDataManager.Instance.UserData.Hero_Camera_Array / 4) == 0)
        {
            for (int a = 0; a < 4; a++)
            {
                for (int b = 0; b < 4; b++)
                {
                    if (CDataManager.Instance.CheckHeroArray(a, b) != 0)
                    {
                        CDataManager.Instance.UserData.Hero_Camera_Array = a + 4 * b;
                        CDataManager.Instance.SaveUserData();
                    }
                }
            }
        }
        
    }
}
