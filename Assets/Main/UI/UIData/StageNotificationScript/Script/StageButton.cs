using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageButton : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] Image _backgroundImage;
    [SerializeField] TextMeshProUGUI _stageText;
    [SerializeField] GameObject _selectionFrame; // 강조용 테두리 오브젝트 (Inspector에서 연결)
    [SerializeField] private StageNotificationUI _stageNotificationUI; // StageNotificationUI 프리팹

    [Header("상태별 스프라이트/컬러")]
    [SerializeField] Sprite _beforeMainSprite;  // 이전
    [SerializeField] Sprite _CurrentSprite;   // 현재
    [SerializeField] Sprite _AfterMainSprite; // 미클리어
    [SerializeField] public int _stageNum=1;
    private void Start()
    {
        if(_stageNotificationUI == null)
        {
            _stageNotificationUI = GetComponentInParent<StageNotificationUI>();
        }
    }
    public void SetSelect(bool isSelect)
    {
        if (_selectionFrame != null)
        {
            _selectionFrame.SetActive(isSelect); // 테두리 오브젝트를 켜거나 끔
        }
    }

    public void SetStageStatus(int num)
    {
        _stageNum = num;
        _stageText.text = _stageNum.ToString();
        int mainStage = CDataManager.Instance.UserData.MainStageLevel; // 현재 클리어한 스테이지 레벨
        int currentStage = CDataManager.Instance.UserData.CurrentStageLevel; // 현재 클리어한 스테이지 레벨


        if (_stageNum == currentStage)
        {
            // 현재 스테이지: 빨간색 (강조 효과)
            _backgroundImage.sprite = _CurrentSprite;
            GetComponent<Button>().interactable = true;
        }
        else if (_stageNum < mainStage)
        {
            // 이전 스테이지: 파란색
            _backgroundImage.sprite = _beforeMainSprite;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            // 미클리어: 검은색 (잠금 처리)
            _backgroundImage.sprite = _AfterMainSprite;
            GetComponent<Button>().interactable = false; // 클릭 방지
        }
        if (_stageNotificationUI != null && _stageNum == _stageNotificationUI._currentStage)
        {
            SetSelect(true);
        }
        else
        {
            SetSelect(false);
        }
    }
}