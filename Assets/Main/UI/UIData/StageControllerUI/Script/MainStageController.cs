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
            CBossSpawner.Instance.ClearActiveBoss();
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(true);


            Debug.Log($"메인스테이지3 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            _spawnArea3.ReStartStage();

        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 21)
        {
            ClearEnemies();
            CBossSpawner.Instance.ClearActiveBoss();
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(true);
            _mainStageTheme3.gameObject.SetActive(false);
            Debug.Log($"메인스테이지2 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            _spawnArea2.ReStartStage();

        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 1)
        {
            ClearEnemies();
            CBossSpawner.Instance.ClearActiveBoss();
            _mainStageTheme1.gameObject.SetActive(true);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(false);
            Debug.Log($"메인스테이지1 전환");

            Scene currentScene = SceneManager.GetActiveScene();
            Time.timeScale = 1f;
            _spawnArea1.ReStartStage();

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
        ClearEnemies();
        CDataManager.Instance.MainStageLevelUP(1);
        SetMainStageTheme();
    }

}
