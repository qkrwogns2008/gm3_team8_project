using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CGachaView : MonoBehaviour
{
    [Header("재화 관련 설정")]
    public TMP_Text _rubyText;              // 루비 개수
    public TMP_Text _summonCard;            // 소환권 개수

    [Header("카테고리 탭 그룹 설정")]
    public CanvasGroup _categoryGroup;      // 영웅 / 펫 / 성물 버튼 그룹
    public CanvasGroup _heroTabGroup;       // 영웅 탭 그룹 (투명도)
    public CanvasGroup _petTabGroup;        // 펫 탭 그룹 (투명도)
    public CanvasGroup _holyTabGroup;       // 성물 탭 버튼 (투명도)

    [Header("카테고리 탭 버튼 설정")]
    public Button _heroTabButton;           // 영웅 탭 버튼
    public Button _petTabButton;            // 펫 탭 버튼
    public Button _holyTabButton;           // 성물 탭 버튼

    [Header("소환 레벨 경험치")]
    public Image _expFillImage;             // FIll Amount 조절용 이미지
    public TMP_Text _expText;               // 10 / 100 텍스트
    public TMP_Text _levelText;             // LV 표시 텍스트

    [Header("메인 이미지")]
    public Image _mainShopImage;            // 메인 이미지

    [Header("뽑기 버튼")]
    public Button _gachaOneButton;          // 1회 뽑기 버튼
    public Button _gachaTenButton;          // 10회 뽑기 버튼
    public Button _gachaThirtyButton;       // 30회 뽑기 버튼
    public Button _gachaTHundredButton;     // 300회 뽑기 버튼

    [Header("뽑기 창")]
    public GameObject _resultPanel;         // 뽑기 창
    public Transform _gachaTransform;       // 카드 출력 위치
    public Button _openAllCard;             // 모두 열기 버튼
    public Button _closeButton;             // 닫기 버튼

    [Header("미니 뽑기(300 뽑기) 창")]

    public GameObject _miniCardPanel;       // 미니 뽑기 창 
    public Transform _miniCardTransform;    // 미니 카드 출력 위치
    public Button _miniCardButton;          // 닫기 버튼
}
