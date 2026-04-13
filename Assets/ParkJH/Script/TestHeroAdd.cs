using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHeroAdd : MonoBehaviour
{
    CGroupManager groupManager;
    void Awake()
    {

    }
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
        CGroupManager.instance.SetUpGroupFromDB();
    }
}
