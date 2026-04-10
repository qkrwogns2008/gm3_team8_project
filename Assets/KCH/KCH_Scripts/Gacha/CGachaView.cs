using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CGachaView : MonoBehaviour
{
    //[Header("상점 패널 설정")]
    //public GameObject GachaMainPanel;      // 뽑기 패널
    //public GameObject ShopMainPanel;       // 상점 패널

    [Header("카테고리 탭 그룹 설정")]
    public CanvasGroup CategoryGroup;      // 영웅 / 펫 / 성물 버튼 그룹
    public CanvasGroup HeroTabGroup;       // 영웅 탭 그룹 (투명도)
    public CanvasGroup PetTabGroup;        // 펫 탭 그룹 (투명도)
    public CanvasGroup HolyTabGroup;       // 성물 탭 버튼 (투명도)

    [Header("카테고리 탭 버튼 설정")]
    public Button HeroTabButton;           // 영웅 탭 버튼
    public Button PetTabButton;            // 펫 탭 버튼
    public Button HolyTabButton;           // 성물 탭 버튼

    [Header("영웅 소환 레벨 경험치")]
    public Image ExpFillImageHero;         // FIll Amount 조절용 이미지
    public TMP_Text ExpTextHero;           // 10 / 100 텍스트
    public TMP_Text LevelTextHero;         // LV 표시 텍스트

    [Header("펫 소환 레벨 경험치")]
    public Image ExpFillImagePet;          // FIll Amount 조절용 이미지
    public TMP_Text ExpTextPet;            // 10 / 100 텍스트
    public TMP_Text LevelTextPet;          // LV 표시 텍스트

    [Header("메인 이미지")]
    public Image MainShopImage;            // 메인 이미지

    [Header("재화 관련 설정")]
    public TMP_Text SummonRuby;            // 루비 개수
    public TMP_Text SummonCard;            // 소환권 개수

    [Header("재화 부족 팝업 설정")]
    public GameObject MsgPopupPanel;       // 젠체 화면 팝업 투명 패널
    public TMP_Text MsgPopupText;          // 중앙 팝업 메세지

    [Header("비활성화 스타일 설정")]
    public Color DisabledTextColor;        // 비활성화 시 글자 색    
    public Vector2 DisabledButtonSize;     // 비활성화 시 버튼 크기
    public Vector2 NormalButtonSize;       // 일반 버튼 크기

    [Header("뽑기 버튼")]
    public Button GachaOneButton;          // 1회 뽑기 버튼
    public Button GachaTenButton;          // 10회 뽑기 버튼
    public Button GachaThirtyButton;       // 30회 뽑기 버튼
    public Button GachaTHundredButton;     // 300회 뽑기 버튼

    [Header("뽑기 버튼 리소스 설정")]
    public Sprite TicketIcon;              // 소환권 아이콘
    public Sprite RubyIcon;                // 루비 아이콘
    public Sprite NormalGachaButton;       // 뽑기 버튼 일반 이미지
    public Sprite DisableGachaButton;      // 뽑기 버튼 비활성화 이미지

    [Header("뽑기 창")]
    public GameObject ResultPanel;         // 뽑기 창
    public Transform GachaTransform;       // 카드 출력 위치
    public Button OpenAllCard;             // 모두 열기 버튼
    public Button CloseButton;             // 닫기 버튼

    [Header("미니 뽑기(300 뽑기) 창")]
    public GameObject MiniCardPanel;       // 미니 뽑기 창 
    public Transform MiniCardTransform;    // 미니 카드 출력 위치
    public Button MiniCardButton;          // 닫기 버튼

    [Header("결과창 재뽑기 버튼")]
    public Button ReRollTenButton;         // 결과창 10회 재뽑기 버튼
    public Button ReRollThirtyButton;      // 결과창 30회 재뽑기 버튼
    public GameObject ResultButtonGroup;   // 결과창 닫기/재뽑기 버튼 그룹

}   
