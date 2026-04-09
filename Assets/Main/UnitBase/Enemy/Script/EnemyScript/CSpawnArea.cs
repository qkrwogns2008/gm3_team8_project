using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpawnArea : MonoBehaviour
{

    #region 인스펙터
    [SerializeField] private GameObject _monsterPrefab;         // 몬스터 프리팹
    [SerializeField] private float _spawnRadius = 30f;           // 구역 크기
    [SerializeField] private int _maxMonsterCount = 10;         // 구역당 최대 스폰수


    #endregion

    #region 내부 변수

    private List<CEnemyBase> _activeMonsters = new List<CEnemyBase>();
    private float _spawnCoolTime = 3f;

    private int _waitSpawnCount = 0;
    #endregion


    void Start()
    {
        CEnemyBase enemyScript = _monsterPrefab.GetComponent<CEnemyBase>();


        if (enemyScript != null && enemyScript.EnemyData != null)
        {
            _spawnCoolTime = enemyScript.EnemyData.SpawnCooltime;
        }

        for (int i = 0; i< _maxMonsterCount; i++)
        {
            SpawnMonster();
        }
    }


    void Update()
    {
        CleanUpList();

        int currentTotal = _activeMonsters.Count + _waitSpawnCount;

        if(currentTotal < _maxMonsterCount)
        {
            StartCoroutine(CoRespawnMonster());
            currentTotal++;
        }
    }

    // 개별 몬스터 리스폰 관리
    private IEnumerator CoRespawnMonster()
    {
        _waitSpawnCount++;

        yield return new WaitForSeconds(_spawnCoolTime);

        SpawnMonster();

        _waitSpawnCount--;
    }

    private void SpawnMonster()
    {
        if (_monsterPrefab == null)
        {
            return;
        }
        Vector2 randomPoint = Random.insideUnitCircle * _spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0f);
        spawnPos.z = 0f;

        GameObject monster = PoolManager.Instance.Pop(_monsterPrefab, spawnPos, Quaternion.identity);
        if (monster != null)
        {

            CEnemyBase enemyScript = monster.GetComponent<CEnemyBase>();

            if (enemyScript != null)
            {
                _activeMonsters.Add(enemyScript);
                CAutoEnemyMove ai = enemyScript.GetComponent<CAutoEnemyMove>();
                ai.ChangeState(EUnitState.Idle);
            }
            
        }
    }

    private void CleanUpList()
    {
        for(int i = _activeMonsters.Count - 1; i>= 0; i--)
        {
            CEnemyBase enemy = _activeMonsters[i];

            // 비활성화 수거
            if(enemy == null || !enemy.gameObject.activeSelf || enemy.IsUnitDead)
            {
                _activeMonsters.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.15f);
        Gizmos.DrawSphere(transform.position, _spawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
