using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ProcessDrop(List<CMonsterItemData> dropTable, Vector3 spawnPosition)
    {
        if (dropTable == null || dropTable.Count == 0)
        {
            Debug.Log("드랍 테이블 비어있음");
            return;
        }

        int stage = CDataManager.Instance.UserData.MainStageLevel;
        foreach(var info in dropTable)
        {
            if (info.itemPrefab == null)
            {
                continue;
            }
           
            if (Random.Range(0f, 100f) <= info.probability)
            {
                // 아이템 꺼내기
                GameObject itemObj = PoolManager.Instance.Pop(info.itemPrefab, spawnPosition, Quaternion.identity);

                if(itemObj != null)
                {
                    itemObj.transform.localScale = info.itemScale;

                    var dropScript = itemObj.GetComponent<CDropItem>();
                    if(dropScript != null)
                    {
                        dropScript.Init(info.itemPrefab);
                    }
                }

                int finalAmount = Mathf.RoundToInt(info.dropAmount * Mathf.Pow(1.5f, stage - 1));

                switch(info.itemName)
                {
                    case "Gold":
                        CDataManager.Instance.AddGold(info.dropAmount);
                        break;
                    case "Ruby":
                        CDataManager.Instance.AddRubby(info.dropAmount);
                        break;
                    case "Exp":
                        CDataManager.Instance.AddExp(info.dropAmount);
                        break;
                }
            }
        }
    }
}
