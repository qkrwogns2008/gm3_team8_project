using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CDropInfo
{
    public string itemName;         // 아이템 이름
    public GameObject itemPrefab;   // 프리팹
    [Range(0f, 100f)]
    public float probability;       // 드랍 확률
}
