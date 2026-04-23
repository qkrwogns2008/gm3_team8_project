using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CTitleScene_FiveKnight : MonoBehaviour
{
    #region 인스펙터
    [Header("타이틀 UI 설정")]
    [SerializeField] private TMP_Text _titleText;               // 타이틀 로딩 텍스트
    [SerializeField] private TMP_Text _tipTexr;                 // 로딩 팁 텍스트
    [SerializeField] private GameObject _loadingUI;             // 로딩 UI
    [SerializeField] private string[] _loadingTips;             // 로딩 Tip
    [SerializeField] private float _textChangeTime = 1.75f;     // 로딩 변경 주기 시간
    #endregion

    #region 내부 변수
    private bool _isReady = false;                              // 게임 준비 확인 
    #endregion

    public void Start()
    {
        StartCoroutine(CO_InitializeGame());
    }

    private void Update()
    {
        // 화면 터치시 코루틴 실행
        if (_isReady == true && Input.GetMouseButtonDown(0))
        {
            _isReady = false;
            StartCoroutine(CO_StartLoading());
        }
    }

    // Start와 동시에 시작
    private IEnumerator CO_InitializeGame()
    {
        _titleText.text = "리소스 확인 중...";


        for (int i = 0; i < 3; i++)
        {
            //_titleText.text = $"리소스 확인 중.";
            yield return new WaitForSeconds(1.5f);
        }

        _titleText.text = "시작하려면 터치하세요";
        _isReady = true;
    }

    // 비동기 로딩후 씬 전환
    private IEnumerator CO_StartLoading()
    {
        _titleText.gameObject.SetActive(false);

        if(_loadingUI != null)
        {
            _loadingUI.SetActive(true);
        }

        UpdateRandomTip();

        // 비동기 씬 로딩
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene");

        // 로딩 완료 후 대기
        op.allowSceneActivation = false;

        float timer = 0;
        float totalWaitTime = 0;

        while (op.progress < 0.9f || totalWaitTime < 4.0f)
        {
            timer += Time.deltaTime;
            totalWaitTime += Time.deltaTime;

            if (timer >= _textChangeTime)
            {
                UpdateRandomTip();
                timer = 0f;
            }

            yield return null;
        }

        // 메인씬 전환
        CGameManager.Instance.ChangeState(GameState.MainStage);

        // 씬 활성화
        op.allowSceneActivation = true;
    }

    // 랜덤 텍스트
    private void UpdateRandomTip()
    {
        if (_loadingTips.Length > 0)
        {
            int randomIndex = Random.Range(0, _loadingTips.Length);
            _tipTexr.text = _loadingTips[randomIndex];
        }
    }
}
