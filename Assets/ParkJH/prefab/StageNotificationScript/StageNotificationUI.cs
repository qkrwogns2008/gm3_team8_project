using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNotificationUI : MonoBehaviour
{
    private int _currentStage = 1;
    [SerializeField]private CanvasGroup _mainCanvasGroup;
    [SerializeField]private CanvasGroup _themeCanvasGroup1;
    [SerializeField]private CanvasGroup _themeCanvasGroup2;
    [SerializeField]private CanvasGroup _themeCanvasGroup3;
    private CanvasGroup _currentCanvasGroup;
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
        Debug.Log("다음 테마 버튼 클릭됨. 현재 스테이지: " + _currentStage);
        if (_currentStage <= 20)
        {
            // 다음 테마 시작인 21로 점프
            _currentStage = 21;
        }
        else if (_currentStage <= 40)
        {
            // 다음 테마 시작인 41로 점프
            _currentStage = 41;
        }
        else if (_currentStage <= 60)
        {
            return;
        }
    }
    public void OnClickBeforeTheme()
    {
        Debug.Log("이전 테마 버튼 클릭됨. 현재 스테이지: " + _currentStage);
        if (_currentStage >= 41)
        {
            // 이전 테마 시작인 1로 점프
            _currentStage = 40;
        }
        else if (_currentStage >= 21)
        {
            // 이전 테마 시작인 21로 점프
            _currentStage = 20;
        }
        else if (_currentStage >= 1)
        {
            return;
        }
    }

    public void StageNotificationUIOn()
    {
        SetAlpha(_mainCanvasGroup, 1f);
        _currentStage  = CDataManager.Instance.UserData.CurrentStageLevel;
    }
}
