using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class ButtonSpawner : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private StageNotificationUI _stageNotificationUI; // StageNotificationUI 프리팹
    [SerializeField] private GameObject _stageButtonPrefab; // StageButton 프리팹
    [SerializeField] private ScrollRect _scrollRect; // 스크롤 렉트 연결
    [SerializeField] private RectTransform _viewport; // 스크롤의 Viewport 영역
    [SerializeField] private int _totalStageCount = 60;     // 생성할 총 개수

    private List<StageButton> _allButtons = new List<StageButton>();
    private StageButton _currentFocusedButton;
    private bool _isAutoScrolling = false; // 자동 스크롤 중인지 여부

    void Start()
    {
        if (_stageNotificationUI == null)
        {
            _stageNotificationUI = GetComponentInParent<StageNotificationUI>();
        }
        SpawnStageButtons();
    }
    void Update()
    {
        // 실시간으로 중앙에 가장 가까운 버튼을 찾습니다.
        if (!_isAutoScrolling)
        {
            FindCenterButton();
        }
    }
    public IEnumerator CoScrollToStage(int targetStageNum)
    {
        _isAutoScrolling = true;
        yield return new WaitForSeconds(0.1f); // 약간의 딜레이를 줘서 UI가 먼저 업데이트되도록 함


        // 1. 전체 스테이지 중 목표 스테이지의 위치 비율 계산 (0~1)
        // (현재번호 - 1) / (전체개수 - 1)
        float targetPos = (float)(targetStageNum - 1) / (_totalStageCount - 1);

        // 2. 스크롤 위치 적용
        // 바로 순간이동시키려면 아래 코드 사용
        _scrollRect.horizontalNormalizedPosition = targetPos;
        _stageNotificationUI._currentStage = targetStageNum;
        // 3. 테두리 강제 갱신을 위해 FindCenterButton() 호출
        FindCenterButton();
        _isAutoScrolling = false;

    }

        void FindCenterButton()
    {
        if (_allButtons.Count == 0) return;

        // 1. 뷰포트의 중앙 지점 (월드 좌표) 계산
        Vector3[] corners = new Vector3[4];
        _viewport.GetWorldCorners(corners);
        float centerWorldX = (corners[0].x + corners[2].x) / 2f;

        StageButton closestButton = null;
        float minDistance = float.MaxValue;

        // 2. 모든 버튼 중 중앙점과 가장 가까운 버튼 찾기
        foreach (var btn in _allButtons)
        {
            float distance = Mathf.Abs(btn.transform.position.x - centerWorldX);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestButton = btn;
            }
        }

        // 3. 포커스가 바뀌었을 때만 테두리 업데이트
        if (closestButton != null && _currentFocusedButton != closestButton)
        {
            if (_currentFocusedButton != null) _currentFocusedButton.SetSelect(false);

            _currentFocusedButton = closestButton;
            _currentFocusedButton.SetSelect(true);
            _stageNotificationUI._currentStage = _currentFocusedButton._stageNum;
            // 여기서 현재 선택된 스테이지 번호를 데이터 매니저에 갱신할 수도 있습니다.
            Debug.Log($"현재 포커스 스테이지: {_currentFocusedButton.name }");
        }

    }

    public void SpawnStageButtons()
    {

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _allButtons.Clear();

        // 버튼을 생성합니다.
        for (int i = 1; i <= _totalStageCount; i++)
        {
            // 자신을 부모로 설정합니다.
            GameObject go = Instantiate(_stageButtonPrefab, transform);

            // 생성된 버튼의 스크립트를 가져와서 번호를 먹여줍니다.
            StageButton sb = go.GetComponent<StageButton>();
            if (sb != null)
            {
                sb.SetStageStatus(i); // 팀장님이 만드신 함수 호출!
                _allButtons.Add(sb);
            }
        }
    }
    public void ScrollToStage(int targetStageNum)
    {
        if (_allButtons == null || _allButtons.Count == 0) return;

        // 1. 전체 스테이지 중 목표 스테이지의 위치 비율 계산 (0~1)
        // (현재번호 - 1) / (전체개수 - 1)
        float targetPos = (float)(targetStageNum - 1) / (_totalStageCount - 1);

        // 2. 스크롤 위치 적용
        // 바로 순간이동시키려면 아래 코드 사용
        _scrollRect.horizontalNormalizedPosition = targetPos;
        _stageNotificationUI._currentStage = targetStageNum;
        // 3. 테두리 강제 갱신을 위해 FindCenterButton() 호출
        FindCenterButton();
    }
}