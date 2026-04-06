using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ProcessDrop(List<CDropInfo> dropTable, Vector3 spawnPosition)
    {
        if (dropTable == null || dropTable.Count == 0)
        {
            Debug.Log("드랍 테이블 비어있음");
            return;
        }
        foreach(var info in dropTable)
        {
            if (info.itemPrefab == null)
            {
                continue;
            }
           
            if (Random.Range(0f, 100f) <= info.probability)
            {
                PoolManager.Instance.Pop(info.itemPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
