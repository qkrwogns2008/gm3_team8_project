using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CGachaPresenter : MonoBehaviour
{
    #region 인스펙터
    [Header("Gacha 시스템 연결")]
    [SerializeField] private CGachaModel _gachaModel;                           // 모델 연결 
    [SerializeField] private CGachaView _gachaView;                             // 뷰 연결

    [Header("Gacha 데이터 설정")]
    [SerializeField] private List<CGachaCategorySO> _gachaCategoryList;         // 소환 리스트 SO (영웅SO : 0, 펫SO : 1)
    [SerializeField] private int _currentCategoryIndex = 0;                     // 카테고리 현재 인덱스

    [Header("Card 프리팹 설정")]
    [SerializeField] private GameObject _cardPrefab;                            // 카드 프리팹
    [SerializeField] private GameObject _miniCardPrefab;                        // 카드 프리팹

    [Header("Card 풀링 설정")]
    [SerializeField] private int _poolSize = 30;                                // 풀링 사이즈
    #endregion

    #region 내부 변수
    private List<CGachaResultCard> _cardList = new List<CGachaResultCard>();    // 카드 리스트
    private Sprite _ticketSprite;                                               // 소환권 이미지
    private Sprite _rubySprite;                                                 // 루비 이미지
    private bool _isRolling = false;                                            // 뽑기 중 유무
    #endregion

    private void Awake()
    {
        // 버튼 리스너 연결
        // 뽑기 버튼
        _gachaView._gachaOneButton.onClick.AddListener(() => OnClickButton(1));
        _gachaView._gachaTenButton.onClick.AddListener(() => OnClickButton(10));
        _gachaView._gachaThirtyButton.onClick.AddListener(() => OnClickButton(30));
        _gachaView._gachaTHundredButton.onClick.AddListener(() => OnClickButton(300));

        // 재뽑기 버튼
        _gachaView._reRollTenButton.onClick.AddListener(() => OnClickButton(10));
        _gachaView._reRollThirtyButton.onClick.AddListener(() => OnClickButton(30));

        // 카테고리 버튼
        _gachaView._heroTabButton.onClick.AddListener(() => ChangeCatergory(0));
        _gachaView._petTabButton.onClick.AddListener(() => ChangeCatergory(1));
        _gachaView._holyTabButton.onClick.AddListener(() => ChangeCatergory(2));

        // 모두 열기 버튼
        _gachaView._openAllCard.onClick.AddListener(() => StartCoroutine(CO_OpenAllCard()));

        // 닫기 버튼
        _gachaView._closeButton.onClick.AddListener(OnClickClose);
        _gachaView._miniCardButton.onClick.AddListener(OnClickClose);

        // 카테고리 영웅으로 시작
        ChangeCatergory(0);

        // 재화 이미지
        _ticketSprite = _gachaView._summonCard.transform.parent.GetComponentInChildren<Image>().sprite;
        _rubySprite = _gachaView._summonRuby.transform.parent.GetComponentInChildren<Image>().sprite;

        // 재화 표시
        UpdateMoneyUI();

        // 처음에는 뽑기창 비활성화
        _gachaView._resultPanel.SetActive(false);
    }


    // 인덱스에 따라 소환 카테고리 변경
    private void ChangeCatergory(int index)
    {
        // 현재 인덱스 선택
        _currentCategoryIndex = index;
        // 카테고리 선택
        CGachaCategorySO select = _gachaCategoryList[index];

        // 카테고리 세팅
        _gachaModel.SetCategory(select);
        // 배너 이미지 교체
        _gachaView._mainShopImage.sprite = select._mainShopImage;

        // 카테고리 UI 업데이트
        UpdateCategoryUI();
    }

    // 뽑기 버튼 클릭
    private void OnClickButton(int count)
    {
        if (_isRolling)
        {
            return;
        }

        if (_gachaModel.CheckRuby(count))
        {
            StartCoroutine(Co_GachaClick(count));
        }
        else
        {
            NotEnoughMoneyPopup();
        }

    }

    // 결과 창 닫고 초기화
    private void OnClickClose()
    {
        _gachaView._resultPanel.SetActive(false);
        _gachaView._miniCardPanel.SetActive(false);

        _isRolling = false;
    }

    // 재화 부족 팝업
    private void NotEnoughMoneyPopup()
    {
        _gachaView._msgPopupText.text = "루비가 부족합니다.";
        CanvasGroup canvasGroup = _gachaView._msgPopupPanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        _gachaView._msgPopupPanel.SetActive(true);

        StopCoroutine("CO_HidePopup");
        StartCoroutine("CO_HidePopup");
    }

    // 팝업을 점점 투명하게 변경 코루틴
    private IEnumerator CO_HidePopup()
    {
        yield return new WaitForSeconds(0.5f);

        CanvasGroup canvasGroup = _gachaView._msgPopupPanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            float duration = 0.5f;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float progress = timer / duration;

                canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

                yield return null;
            }
        }

        canvasGroup.alpha = 0f; 

        _gachaView._msgPopupPanel.SetActive(false);
    }

    // 뽑기 연출, 데이터 처리 코루틴
    private IEnumerator Co_GachaClick(int count)
    {
        // 재화 검사 차감
        if (!_gachaModel.CheckRuby(count))
        {
            Debug.Log("재화가 없다..");
            yield break;
        }

        _gachaModel.PayRuby(count);
        UpdateMoneyUI();

        // 뽑기 전 UI 제어
        _gachaView._resultButtonGroup.SetActive(false);
        _gachaView._closeButton.gameObject.SetActive(false);
        _gachaView._openAllCard.gameObject.SetActive(false);

        // 뽑기 중 활성
        _isRolling = true;

        // 뽑을 횟수 
        List<CGachaDataSO> results = _gachaModel.RollGacha(count);

        // 경험치 증가
        _gachaModel.AddExp(count);
        UpdateCategoryUI();

        // 300회 뽑기 시 로직
        if (count == 300)
        {
            _gachaView._miniCardPanel.SetActive(true);
            _gachaView._resultPanel.SetActive(false);

            _gachaView._openAllCard.gameObject.SetActive(false);
            _gachaView._closeButton.gameObject.SetActive(true);

            // 미니 카드 풀에 반납 (역순)
            for (int i = _gachaView._miniCardTransform.childCount - 1; i >= 0; i--)
            {
                PoolManager.Instance.Push(_miniCardPrefab, _gachaView._miniCardTransform.GetChild(i).gameObject);
            }

            // 획득 개수 카운팅
            Dictionary<CGachaDataSO, int> miniCardDict = new Dictionary<CGachaDataSO, int>();

            for (int i = 0; i < results.Count; i++)
            {
                CGachaDataSO data = results[i];
                if (miniCardDict.ContainsKey(data))
                {
                    miniCardDict[data]++;
                }

                else
                {
                    miniCardDict[data] = 1;
                }
            }

            // 희귀도 순으로 Sort정렬
            List<CGachaDataSO> keys = new List<CGachaDataSO>(miniCardDict.Keys);
            keys.Sort((a, b) => b._rarity.CompareTo(a._rarity));

            // 정렬된 순으로 미니 카드 생성
            for (int i = 0; i < keys.Count; i++)
            {
                CGachaDataSO key = keys[i];
                GameObject miniCard = PoolManager.Instance.Pop(_miniCardPrefab, Vector3.zero, Quaternion.identity);

                miniCard.transform.SetParent(_gachaView._miniCardTransform, false);
                miniCard.transform.SetAsLastSibling();

                miniCard.transform.localScale = Vector3.one;
                miniCard.transform.localPosition = Vector3.zero;

                CGachaResultCard cardUI = miniCard.GetComponent<CGachaResultCard>();
                if (cardUI != null)
                {
                    // 데이터 세팅
                    cardUI.SetMiniCard(key, miniCardDict[key]);

                    // 미니 카드 즉시 보여 주기
                    cardUI.ShowVisual();
                }
            }

            _isRolling = false;
            _gachaView._closeButton.gameObject.SetActive(true);
        }
        
        // 1, 10, 30회 뽑기 시 로직
        else
        {
            // 기존 카드 초기화
            for (int i = 0; i < _cardList.Count; i++)
            {
                PoolManager.Instance.Push(_cardPrefab, _cardList[i].gameObject);
            }

            // 리스트 초기화
            _cardList.Clear();

            // 뽑기 결과 창 활성화
            _gachaView._resultPanel.SetActive(true);
            _gachaView._miniCardPanel.SetActive(false);
            _gachaView._closeButton.gameObject.SetActive(false);

            for (int i = 0; i < results.Count; i++)
            {
                // 프리펩 생성
                GameObject card = PoolManager.Instance.Pop(_cardPrefab, Vector3.zero, Quaternion.identity);
                card.transform.SetParent(_gachaView._gachaTransform, false);
                card.transform.SetAsLastSibling();

                card.transform.localScale = Vector3.one;
                card.transform.localPosition = Vector3.zero;

                CGachaResultCard cardUI = card.GetComponent<CGachaResultCard>();

                // 카드 UI에 데이터 세팅
                if (cardUI != null)
                {
                    // 뒷면 카드 세팅
                    cardUI.SetHidden(results[i]);

                    // 카드 위치 먼저 세팅
                    cardUI.HideVisual();

                    _cardList.Add(cardUI);
                }

            }

            for (int i = 0; i < _cardList.Count; i++)
            {
                // 카드 보여 주기 `       
                _cardList[i].ShowVisual();
                // 카드 내려오는 이펙트
                _cardList[i].SpawnEffect();

                // 순번으로 출력
                yield return new WaitForSeconds(0.05f);
            }


            // 뽑기 중 해제
            _isRolling = false;
            _gachaView._openAllCard.gameObject.SetActive(true);

        }
        
    }

    // 모두 열기 기능 함수
    private IEnumerator CO_OpenAllCard()
    {
        _gachaView._openAllCard.gameObject.SetActive(false);

        for (int i = 0; i < _cardList.Count; i++)
        {
            // 카드 뒤집기 로직
            _cardList[i].ReverseCard();

            yield return new WaitForSeconds(0.05f);
        }

        // 카드 오픈 시 재뽑기, 닫기 버튼 활성화
        _gachaView._resultButtonGroup.SetActive(true);
        _gachaView._closeButton.gameObject.SetActive(true);
    }

    // 카테고리 경험치 게이지 업데이트
    private void UpdateCategoryUI()
    {
        CGachaCategorySO current = _gachaCategoryList[_currentCategoryIndex];

        int maxExp = current._maxExpTable[current._currentLevel - 1];

        _gachaView._levelText.text = current._categoryName + "소환 레벨 " + current._currentLevel;
        _gachaView._expText.text = current._currentExp + " / " + maxExp;
        _gachaView._expFillImage.fillAmount = (float)current._currentExp / maxExp;

        _gachaView._heroTabGroup.alpha = (_currentCategoryIndex == 0) ? 1.0f : 0.0f;
        _gachaView._petTabGroup.alpha = (_currentCategoryIndex == 1) ? 1.0f : 0.0f;
        _gachaView._holyTabGroup.alpha = (_currentCategoryIndex == 2) ? 1.0f : 0.0f;
    }

    // 재화 정보  UI 업데이트
    private void UpdateMoneyUI()
    {
        _gachaView._summonRuby.text = _gachaModel._rubyCount.ToString("N0");
        _gachaView._summonCard.text = _gachaModel._ticketCount.ToString("N0");

        // 뽑기 별로 비용, 이미지 변경
        SetButtonCostUI(_gachaView._gachaOneButton, 1);
        SetButtonCostUI(_gachaView._gachaTenButton, 10);
        SetButtonCostUI(_gachaView._gachaThirtyButton, 30);
        SetButtonCostUI(_gachaView._gachaTHundredButton, 300);

        SetButtonCostUI(_gachaView._reRollTenButton, 10);
        SetButtonCostUI(_gachaView._reRollThirtyButton, 30);
    }

    // 버튼과 재화 상태 변경(소환권 우선 소모)
    private void SetButtonCostUI(Button button, int count)
    {
        string iconName = "ButtonIcon";
        string textName = "ButtonText";

        int rubyPrice = count * 100;

        RectTransform buttonTransform = button.GetComponent<RectTransform>();
        Image buttonImage = button.GetComponent<Image>();

        // ButtonIcon 만 찾아서 변경
        Image iconImage = button.transform.Find(iconName)?.GetComponent<Image>();
        // ButtonText 만 찾아서 변경
        TMP_Text costText = button.transform.Find(textName)?.GetComponent<TMP_Text>();

        if (iconImage == null || costText == null)
        {
            return;
        }

        // 재화 변경시 아이콘, 텍스트 변경
        // 소환권 충분
        if (_gachaModel._ticketCount >= count)
        {
            costText.text = count.ToString();
            iconImage.sprite = _ticketSprite;
            buttonImage.sprite = _gachaView._normalGachaButton;
        }

        // 소환권 부족, 루비 충분
        else if (_gachaModel._rubyCount >= rubyPrice)
        {
            costText.text = rubyPrice.ToString("N0");
            iconImage.sprite = _rubySprite;
            buttonImage.sprite = _gachaView._normalGachaButton;
        }

        // 소환권 부족, 루비 부족
        else
        {
            costText.text = rubyPrice.ToString("N0");
            iconImage.sprite = _rubySprite;
            buttonImage.sprite = _gachaView._disableGachaButton;
        }

        // 재화 부족 시 버튼 비활성화
        if (_gachaModel.CheckRuby(count))
        {
            costText.color = Color.white;
            buttonImage.sprite = _gachaView._normalGachaButton;
            buttonTransform.sizeDelta = _gachaView._normalButtonSize;
        }

        else
        {
            costText.color = Color.white;
            buttonImage.sprite = _gachaView._disableGachaButton;
            buttonTransform.sizeDelta = _gachaView._disabledButtonSize;
        }
    }

}
