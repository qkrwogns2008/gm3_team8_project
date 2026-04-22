using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBossSpawner : MonoBehaviour
{
    public static CBossSpawner Instance { get; private set; }

    #region 인스펙터

    [Header("보스 프리팹 설정")]
    [SerializeField] private GameObject[] _bossPrefabs;

    [Header("소환 위치")]
    [SerializeField] private Transform _spawnPoint;

    [Header("에너미스포너 참조")]
    [SerializeField] private CSpawnArea _spawnArea1;     // 1~20
    [SerializeField] private CSpawnArea _spawnArea2;     // 21~40
    [SerializeField] private CSpawnArea _spawnArea3;     // 41~60
    #endregion

    private GameObject _activeBoss = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OnClickBossSpawn()
    {
        if(_activeBoss != null)
        {
            return;
        }
        if (CDataManager.Instance.UserData.CurrentStageLevel == CDataManager.Instance.UserData.MainStageLevel)
        {
            SpawnBossByCurrentStage();
        }
        else
        {
            Debug.LogWarning("최대 스테이지에서 보스 소환이 가능합니다.");
        }
    }

    public void SpawnBossByCurrentStage()
    {
        if(CDataManager.Instance == null || _bossPrefabs.Length == 0)
        {
            Debug.LogWarning("데이터 매니저가 없거나 보스 프리펩 미싱");
            return;
        }

        // 기존에 있던 보스 제거
        if (_activeBoss != null)
        {
            Destroy(_activeBoss);
            _activeBoss = null;
        }
        StopAndClearSpawners();

        // 현재 스테이지 레벨 확인
        int currentStage = CDataManager.Instance.UserData.CurrentStageLevel;

        // 스테이지에 맞는 보스 설정 (20스테이지 마다)
        int bossIndex = (currentStage - 1) / 20;

        
        if(bossIndex >= _bossPrefabs.Length)
        {
            bossIndex = _bossPrefabs.Length - 1;
        }

        GameObject selectedBossPrefab = _bossPrefabs[bossIndex];

        if(selectedBossPrefab == null)
        {
            return;
        }
       

        // 보스 소환
        Vector3 spawnPos = (_spawnPoint != null) ? _spawnPoint.position : Vector3.zero;


        _activeBoss = Instantiate(selectedBossPrefab, spawnPos, Quaternion.identity);

        CBoss bossScript = _activeBoss.GetComponent<CBoss>();
        if (bossScript != null)
        {
            // 리스트에 보스 등록
            if(CEnemyManager.Instance != null)
            {
                CEnemyManager.Instance.RegisterEnemy(bossScript);
            }
        }
    }

    private void StopAndClearSpawners()
    {
        if(_spawnArea1 != null)
        {
            _spawnArea1.ClearAllMonsters();
        }
        if(_spawnArea2 != null)
        {
            _spawnArea2.ClearAllMonsters();
        }    
        if(_spawnArea3 != null)
        {
            _spawnArea3.ClearAllMonsters();
        }
    }

    public void ClearActiveBoss()
    {
        if (_activeBoss != null)
        {
            Destroy(_activeBoss);
            _activeBoss = null;
        }
    }
}
