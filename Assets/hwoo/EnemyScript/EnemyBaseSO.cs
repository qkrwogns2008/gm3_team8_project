using Spine.Unity;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "Monster_Data(SO)")]
public class EnemyBaseSO : UnitDataSO
{
    [SerializeField] protected int _goldReward;             // 드롭 골드
    [SerializeField] protected int _expReward;              // 드롭 경험치
    [SerializeField] protected int _itemReward;             // 드롭 아이템

    [SerializeField] protected float _spawnCooltime;        // 스폰 쿨타임
    [SerializeField] protected float _detectionRange = 8f;  // 플레이어 감지 거리
    [SerializeField] protected float _giveUpRange = 12f;    // 추격 포기 거리


    [SerializeField] protected GameObject _attackEffectPrefab;      // 공격 이펙트 프리펩


    public int GoldReward => _goldReward;
    public int ExpReward => _expReward;
    public int ItemReward => _itemReward;

    public float SpawnCooltime => _spawnCooltime;
    public float DetectionRange => _detectionRange;
    public float GiveUpRange => _giveUpRange;

    public GameObject AttackEffectPrefab => _attackEffectPrefab;
   
}