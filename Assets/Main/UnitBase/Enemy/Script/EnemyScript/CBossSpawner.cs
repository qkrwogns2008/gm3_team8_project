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
    [SerializeField] private CSpawnArea _spawnArea;

    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }


    public void OnClickBossSpawn()
    {
        SpawnBossByCurrentStage();
    }

    public void SpawnBossByCurrentStage()
    {
        if(CDataManager.Instance == null || _bossPrefabs.Length == 0)
        {
            return;
        }

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

        // 기존 소환 몹들 정리
        if(_spawnArea != null)
        {
            _spawnArea.ClearAllMonsters();
        }

        // 보스 소환
        Vector3 spawnPos = (_spawnPoint != null) ? _spawnPoint.position : Vector3.zero;
        GameObject bossObj = PoolManager.Instance.Pop(selectedBossPrefab, spawnPos, Quaternion.identity);

        CBoss bossScript = bossObj.GetComponent<CBoss>();

        if (bossScript != null)
        {
            /// 보스 등장시 로직 필요시 추가
        }
    }

}
