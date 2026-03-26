using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRewardManager : MonoBehaviour
{
    public static CRewardManager Instance;
    [SerializeField] private GameObject _goldPrefab;
    [SerializeField] private GameObject _expPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void GiveReward(EnemyBaseSO data, Vector3 spawnPosition)
    {
        if(data == null)
        {
            return;
        }

        if(data.GoldReward > 0)
        {
            SpawnRewardItem();
        }
    }
    private void SpawnRewardItem()
    {

    }
}
