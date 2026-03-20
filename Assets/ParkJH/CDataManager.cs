using UnityEngine;
using System.IO;

public class CDataManager : MonoBehaviour
{
    public static CDataManager Instance { get; private set; }

    // 게임 중 실시간으로 참조할 데이터
    public CUserData UserData { get; private set; }

    private string _savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
            LoadGame();
        }
        else { Destroy(gameObject); }
    }

    // [저장] 데이터 -> JSON -> 파일
    public void SaveGame()
    {
        UserData.LastLogoutTime = System.DateTime.Now.ToString();
        string json = JsonUtility.ToJson(UserData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"저장 완료: {_savePath}");
    }

    // [로드] 파일 -> JSON -> 데이터
    public void LoadGame()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            UserData = JsonUtility.FromJson<CUserData>(json);
        }
        else
        {
            // 파일이 없으면 초기 데이터 생성
            UserData = new CUserData();
            SaveGame();
        }
    }

    // 재화 추가 같은 공통 함수
    public void AddGold(int amount)
    {
        UserData.Gold += amount;
        // UI 친구에게 "골드 변했어!"라고 알려주는 이벤트 호출 지점
    }
}