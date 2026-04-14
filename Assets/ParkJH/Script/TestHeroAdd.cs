using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHeroAdd : MonoBehaviour
{
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
        CDataManager.Instance.AddHeroData(EHeroID.Baskin);
        CDataManager.Instance.AddHeroData(EHeroID.Nami);
        CDataManager.Instance.AddHeroData(EHeroID.Loto);
        CDataManager.Instance.AddHeroData(EHeroID.Jak);
        CDataManager.Instance.AddHeroData(EHeroID.Sarah);
        CDataManager.Instance.AddHeroData(EHeroID.Elga);
        CDataManager.Instance.AddHeroData(EHeroID.Karon);
        CDataManager.Instance.AddHeroData(EHeroID.Rook);
        CDataManager.Instance.AddHeroData(EHeroID.Snipper);
        CDataManager.Instance.AddHeroData(EHeroID.Shane);
        CDataManager.Instance.AddHeroData(EHeroID.Evan);
        CDataManager.Instance.AddHeroData(EHeroID.Alice);
        CDataManager.Instance.AddHeroData(EHeroID.Teo);
        CDataManager.Instance.AddHeroData(EHeroID.Yeonhee);
        CDataManager.Instance.AddHeroData(EHeroID.Radgrid);
        CDataManager.Instance.AddHeroData(EHeroID.Ecila);
        CGroupManager.instance.SetUpGroupFromDB();
    }
}
