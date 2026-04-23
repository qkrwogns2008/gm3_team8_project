using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStageController : MonoBehaviour
{
    public static MainStageController Instance { get; private set; }
    [SerializeField] Transform _mainStageTheme1;
    [SerializeField] Transform _mainStageTheme2;
    [SerializeField] Transform _mainStageTheme3;
    [SerializeField] CSpawnArea _spawnArea1;
    [SerializeField] CSpawnArea _spawnArea2;
    [SerializeField] CSpawnArea _spawnArea3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMainStageTheme()
    {
        Debug.Log($"SetMainStageTheme 작동");
        if (CDataManager.Instance.UserData.CurrentStageLevel >= 41)
        {
            ClearEnemies();
            if (!CBossSpawner.IsBossMode)
            {
                CBossSpawner.Instance.ClearActiveBoss();
            }
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(true);


            Debug.Log($"메인스테이지3 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            if (!CBossSpawner.IsBossMode && _spawnArea3.gameObject.activeInHierarchy)
            {
                _spawnArea3.ReStartStage();
            }


        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 21)
        {
            ClearEnemies();
            if (!CBossSpawner.IsBossMode)
            {
                CBossSpawner.Instance.ClearActiveBoss();
            }
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(true);
            _mainStageTheme3.gameObject.SetActive(false);
            Debug.Log($"메인스테이지2 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            if (!CBossSpawner.IsBossMode && _spawnArea2.gameObject.activeInHierarchy)
            {
                _spawnArea2.ReStartStage();
            }

        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 1)
        {
            ClearEnemies();
            if (!CBossSpawner.IsBossMode)
            {
                CBossSpawner.Instance.ClearActiveBoss();
            }
            _mainStageTheme1.gameObject.SetActive(true);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(false);
            Debug.Log($"메인스테이지1 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            if (!CBossSpawner.IsBossMode && _spawnArea1.gameObject.activeInHierarchy)
            {
                _spawnArea1.ReStartStage();
            }
        }
    }
    private void ClearEnemies()
    {
        CEnemyBase[] enemies = Object.FindObjectsByType<CEnemyBase>(FindObjectsSortMode.None);

        foreach (CEnemyBase enemy in enemies)
        {

            // 적 오브젝트 삭제
            _spawnArea1.ClearAllMonsters();
            _spawnArea2.ClearAllMonsters();
            _spawnArea3.ClearAllMonsters();
        }

        Debug.Log($"[정리] 기존 적 {enemies.Length}마리를 제거했습니다.");
    }
    public void MainStageUp()
    {
        StartCoroutine(CO_SafeStageTransition());
    }
    private IEnumerator CO_SafeStageTransition()
    {
        // 생성 우선 중단
        StopAllSpawnersOnly();

        // 타겟팅 막기
        if(CEnemyManager.Instance != null )
        {
            CEnemyManager.Instance.ClearEnemyList();
        }

        
        if(CGroupManager.instance != null)
        {
            CGroupManager.instance.BroadcastSharedTarget(null);
        }

        // 애니메이션 종료 대기시간
        yield return new WaitForSeconds(1f);

        // 몬스터 정리
        ClearAllMonsters();

        // 보스 스포너 참조 정리
        if(CBossSpawner.Instance != null )
        {
            CBossSpawner.Instance.ClearActiveBoss();
        }

        CDataManager.Instance.MainStageLevelUP(1);
        SetMainStageTheme();
        
        
    }

    // 다음 스테이지 스포너 돌리기
    private void RestartNewStageSpawner()
    {
        int currentStage = CDataManager.Instance.UserData.CurrentStageLevel;

        if(currentStage >= 41)
        {
            if(_spawnArea3)
            {
                _spawnArea3.ReStartStage();
            }
        }
        if (currentStage >= 21)
        {
            if (_spawnArea2)
            {
                _spawnArea2.ReStartStage();
            }
        }
        if (currentStage >= 0)
        {
            if (_spawnArea1)
            {
                _spawnArea1.ReStartStage();
            }
        }
    }

    #region 코루틴 정지용

    private void StopAllSpawnersOnly()
    {
        if (_spawnArea1 != null)
        {
            _spawnArea1.StopSpawning();
        }
        if (_spawnArea2 != null)
        {
            _spawnArea2.StopSpawning();
        }
        if (_spawnArea3 != null)
        {
            _spawnArea3.StopSpawning();
        }
    }

    private void ClearAllMonsters()
    {
        if (_spawnArea1 != null)
        {
            _spawnArea1.ClearAllMonsters();
        }
        if (_spawnArea2 != null)
        {
            _spawnArea2.ClearAllMonsters();
        }
        if (_spawnArea3 != null)
        {
            _spawnArea3.ClearAllMonsters();
        }
    }
    
    #endregion
}
