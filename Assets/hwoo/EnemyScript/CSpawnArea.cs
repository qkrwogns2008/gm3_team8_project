using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpawnArea : MonoBehaviour
{

    #region 인스펙터
    [SerializeField] private GameObject _monsterPrefab;         // 몬스터 프리팹
    [SerializeField] private float _spawnRadius = 5f;           // 구역 크기
    [SerializeField] private int _maxMonsterCount = 10;         // 구역당 최대 스폰수


    #endregion

    #region 내부 변수

    private List<GameObject> _activeMonsters = new List<GameObject>();
    private float _spawnTimer = 0f;
    private float _spawnCoolTime = 3f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CEnemyBase enemyScript = _monsterPrefab.GetComponent<CEnemyBase>();

        if(enemyScript != null && enemyScript.EnemyData != null)
        {
            _spawnCoolTime = enemyScript.EnemyData.SpawnCooltime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CleanUpList();
        if(_activeMonsters.Count < _maxMonsterCount)
        {
            _spawnTimer += Time.deltaTime;
            if(_spawnTimer >= _spawnCoolTime )
            {
                SpawnMonster();
                _spawnTimer = 0f;
            }
        }
    }

    private void SpawnMonster()
    {
        if(_monsterPrefab == null)
        {
            return;
        }
        Vector2 randomPoint = Random.insideUnitCircle * _spawnRadius;

        Vector3 spawnPos = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0f);

        GameObject monster = Instantiate(_monsterPrefab, spawnPos, Quaternion.identity);
        _activeMonsters.Add(monster);
    }

    private void CleanUpList()
    {
        for(int i = _activeMonsters.Count - 1; i>= 0; i--)
        {
            if (_activeMonsters[i] == null)
            {
                _activeMonsters.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.15f);
        Gizmos.DrawSphere(transform.position, _spawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
