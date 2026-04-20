using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStageController : MonoBehaviour
{
    public static MainStageController Instance { get; private set; }
    [SerializeField] Transform _mainStageTheme1;
    [SerializeField] Transform _mainStageTheme2;
    [SerializeField] Transform _mainStageTheme3;

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
        if(CGameManager.Instance.CurrentState != GameState.MainStage)
        {
            return; // 메인 스테이지가 아닐 때는 함수 종료
        }
        if (CDataManager.Instance.UserData.CurrentStageLevel >= 41)
        {
            ClearEnemies();
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(true);
        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 21)
        {
            ClearEnemies();
            _mainStageTheme1.gameObject.SetActive(false);
            _mainStageTheme2.gameObject.SetActive(true);
            _mainStageTheme3.gameObject.SetActive(false);

        }
        else if (CDataManager.Instance.UserData.CurrentStageLevel >= 1)
        {
            ClearEnemies();
            _mainStageTheme1.gameObject.SetActive(true);
            _mainStageTheme2.gameObject.SetActive(false);
            _mainStageTheme3.gameObject.SetActive(false);

        }
    }
    private void ClearEnemies()
    {
        CEnemyBase[] enemies = Object.FindObjectsByType<CEnemyBase>(FindObjectsSortMode.None);

        foreach (CEnemyBase enemy in enemies)
        {
            // 적 오브젝트 삭제
            Destroy(enemy.gameObject);
        }

        Debug.Log($"[정리] 기존 적 {enemies.Length}마리를 제거했습니다.");
    }
}
