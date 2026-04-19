using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNotificationUI : MonoBehaviour
{
    [SerializeField]private CanvasGroup _mainCanvasGroup;
    [SerializeField]private CanvasGroup _themeCanvasGroup1;
    [SerializeField]private CanvasGroup _themeCanvasGroup2;
    [SerializeField]private CanvasGroup _themeCanvasGroup3;
    [SerializeField] private ButtonSpawner _buttonSpawner;
    private CanvasGroup _currentCanvasGroup;

    public  int _currentStage;
    private int _MaxStage;
    void Start()
    {
        
    }
	void Update()
    {
        if (_currentStage <= 20)
        {
            SetAlpha(_themeCanvasGroup1, 1f);
            SetAlpha(_themeCanvasGroup2, 0f);
            SetAlpha(_themeCanvasGroup3, 0f);
        }
        else if (_currentStage <= 40)
        {
            SetAlpha(_themeCanvasGroup1, 0f);
            SetAlpha(_themeCanvasGroup2, 1f);
            SetAlpha(_themeCanvasGroup3, 0f);
        }
        else if (_currentStage <= 60)
        {
            SetAlpha(_themeCanvasGroup1, 0f);
            SetAlpha(_themeCanvasGroup2, 0f);
            SetAlpha(_themeCanvasGroup3, 1f);
        }
    }
    private void SetAlpha(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = alpha;
        // 완전히 불투명(1)할 때만 클릭을 막고, 그 외엔 통과시킴
        canvasGroup.interactable = (alpha >= 1f);
        canvasGroup.blocksRaycasts = (alpha >= 1f);
    }
    public void OnClickNextTheme()
    {
        _MaxStage = CDataManager.Instance.UserData.MainStageLevel;
        if (_currentStage <= 20 && _MaxStage >= 21)
        {
            // 다음 테마 시작인 21로 점프
            _currentStage = 21;
        }
        else if (_currentStage <= 40 && _MaxStage >= 41)
        {
            // 다음 테마 시작인 41로 점프
            _currentStage = 41;
        }
        else if (_currentStage <= 60)
        {
            return;
        }
        Debug.Log("다음 테마 버튼 클릭됨. 현재 스테이지: " + _currentStage);
        if (_buttonSpawner != null) _buttonSpawner.ScrollToStage(_currentStage);
    }
    public void OnClickBeforeTheme()
    {
        _MaxStage = CDataManager.Instance.UserData.MainStageLevel;
        if (_currentStage >= 41)
        {
            // 이전 테마 시작인 1로 점프
            _currentStage = 21;
        }
        else if (_currentStage >= 21)
        {
            // 이전 테마 시작인 21로 점프
            _currentStage = 1;
        }
        else if (_currentStage >= 1)
        {
            return;
        }
        Debug.Log("이전 테마 버튼 클릭됨. 현재 스테이지: " + _currentStage);
        if (_buttonSpawner != null) _buttonSpawner.ScrollToStage(_currentStage);
    }

    public void StageNotificationUIOn()
    {
        this.gameObject.SetActive(true);
        _currentStage  = CDataManager.Instance.UserData.CurrentStageLevel;
        _MaxStage = CDataManager.Instance.UserData.MainStageLevel;
        if (_buttonSpawner != null)
        {
            _buttonSpawner.SpawnStageButtons();
            StartCoroutine(_buttonSpawner.CoScrollToStage(_currentStage));
        }
    }
    public void OnClickStageMove()
    {
        if (_currentStage > _MaxStage)
        {
            Debug.Log($"{_currentStage} 는 아직 클리어하지 못했습니다!");
            return;
        }
            // 1. 현재 포커스된 스테이지 번호를 가져와서 UserData에 저장
            int selectedStage = _currentStage; // StageButton에 이 함수가 있어야 합니다.
            CDataManager.Instance.UserData.CurrentStageLevel = selectedStage;
            CDataManager.Instance.SaveUserData();
            CGameManager.Instance.ChangeState(GameState.MainStage);
            MainStageController.Instance.SetMainStageTheme();
            Debug.Log($"[이동] {selectedStage} 스테이지로 설정을 변경했습니다!");
            this.gameObject.SetActive(false);
            // 2. 실제 스테이지 씬으로 이동하거나 팝업을 닫는 로직 추가
            // SceneManager.LoadScene("GameScene"); 혹은 UI 닫기
        
    }
}
