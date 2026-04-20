using UnityEngine;

public enum GameState
{
    Title,      // 시작 전
    MainStage,      // 메인스테이지
    Dungeon,      // 던전 스테이지
    UIScreen,      // UI 화면
    Paused,     // 일시 정지
    GameOver    // 스테이지 패배 Or 보스 사냥 실패
}

public class CGameManager : MonoBehaviour
{
    public static CGameManager Instance;

    [Header("게임 상태")]
    public GameState CurrentState;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 초기 상태 
        ChangeState(GameState.Title);
    }

    // 상태 변경 함수
    public void ChangeState(GameState newState)
    {

        CurrentState = newState;
        if(GameState.Paused != CurrentState && GameState.GameOver != CurrentState)
        {
            Time.timeScale = 1; // 일시 정지 해제
        }

        switch (CurrentState)
        {
            case GameState.Title:
                Debug.Log("타이틀 화면으로 전환");
                // 타이틀 화면 UI
                break;
            case GameState.MainStage:
                    Debug.Log("메인 스테이지로 전환");
                // 메인 스테이지 UI 및 활성화
                break;
            case GameState.Dungeon:
                Debug.Log("던전으로 전환");
                // 던전 UI 및 활성화
                break;
            case GameState.UIScreen:
                Debug.Log("UI스크린으로 전환");
                // 전체화면 사이즈 UI 활성화
                Time.timeScale = 0;
                break;
            case GameState.Paused:
                Debug.Log("일시정지로 전환");
                // 일시정지 관련 UI로직 활성화
                Time.timeScale = 0;
                break;
            case GameState.GameOver:
                Debug.Log("게임오버로 전환");
                // 게임오버 관련 UI로직 활성화
                Time.timeScale = 0;
                break;
        }
    }
}