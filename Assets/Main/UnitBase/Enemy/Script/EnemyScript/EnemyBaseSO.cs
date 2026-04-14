using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CMonsterItemData
{
    public string itemName;         // 아이템 이름
    public int dropAmount;          // 수량
    public GameObject itemPrefab;   // 프리팹
    [Range(0f, 100f)]
    public float probability;       // 드랍 확률
    public Vector3 itemScale = Vector3.one;
}


[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "Monster_Data(SO)")]
public class EnemyBaseSO : UnitDataSO
{


    [SerializeField] protected int _goldReward;             // 드롭 골드
    [SerializeField] protected int _expReward;              // 드롭 경험치
    [SerializeField] protected int _itemReward;             // 드롭 아이템

    [SerializeField] protected float _spawnCooltime;        // 스폰 쿨타임
    [SerializeField] protected float _giveUpRange = 12f;    // 추격 포기 거리

    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected GameObject _attackEffectPrefab;      // 공격 이펙트 프리펩

    [Header("드랍 테이블")]
    public List<CMonsterItemData> _dropTable = new List<CMonsterItemData>();


    public int GoldReward => _goldReward;
    public int ExpReward => _expReward;
    public int ItemReward => _itemReward;

    public float SpawnCooltime => _spawnCooltime;
    //public float DetectionRange => _detectionRange;
    public float GiveUpRange => _giveUpRange;

    public float AttackSpeed => _attackSpeed;
    public GameObject AttackEffectPrefab => _attackEffectPrefab;
   
}