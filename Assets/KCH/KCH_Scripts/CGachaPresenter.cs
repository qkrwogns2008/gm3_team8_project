using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    private bool _isRolling = false;                                            // 뽑기 중 유무
    private Queue<CGachaResultCard> _cardPool = new Queue<CGachaResultCard>();  // 카드 큐
    #endregion

    private void Awake()
    {
        // 버튼 리스너 연결
        // 뽑기 버튼
        _gachaView._gachaOneButton.onClick.AddListener(() => OnClickButton(1));
        _gachaView._gachaTenButton.onClick.AddListener(() => OnClickButton(10));
        _gachaView._gachaThirtyButton.onClick.AddListener(() => OnClickButton(30));
        _gachaView._gachaTHundredButton.onClick.AddListener(() => OnClickButton(300));

        // 카테고리 버튼
        _gachaView._heroTabButton.onClick.AddListener(() => ChangeCatergory(0));
        _gachaView._petTabButton.onClick.AddListener(() => ChangeCatergory(1));
        _gachaView._holyTabButton.onClick.AddListener(() => ChangeCatergory(2));

        // 모두 열기 버튼
        _gachaView._openAllCard.onClick.AddListener(() => StartCoroutine(CO_OpenAllCard()));

        // 닫기 버튼
        _gachaView._closeButton.onClick.AddListener(OnClickClose);

        // 카테고리 영웅으로 시작
        ChangeCatergory(0);

        // 처음에는 뽑기창 비활성화
        _gachaView._resultPanel.SetActive(false);
    }

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

    private void OnClickButton(int count)
    {
        if (_isRolling)
        {
            return;
        }

        StartCoroutine(Co_GachaClick(count));
    }

    private void OnClickClose()
    {
        _gachaView._resultPanel.SetActive(false);
    }

    private IEnumerator Co_GachaClick(int count)
    {
        // 뽑기 중 활성
        _isRolling = true;

        // 이전 카드 삭제
        for (int i = 0; i < _cardList.Count; i++)
        {
            if (_cardList[i] != null)
            {
                Destroy(_cardList[i].gameObject);
            }

        }

        // 리스트 초기화
        _cardList.Clear();

        // 뽑을 횟수 
        List<CGachaDataSO> results = _gachaModel.RollGacha(count);
        
        // 경험치 증가
        _gachaModel.AddExp(count); 
        UpdateCategoryUI();

        // 뽑기 결과 창 활성화
        _gachaView._resultPanel.SetActive(true);
        _gachaView._closeButton.gameObject.SetActive(false);

        for (int i = 0; i < results.Count; i++)
        {
            // 프리펩 생성
            GameObject card = Instantiate(_cardPrefab, _gachaView._gachaTransform);

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
            // 카드 보여 주기
            _cardList[i].ShowVisual();
            // 카드 내려오는 이펙트
            _cardList[i].SpawnEffect();

            // 순번으로 출력
            yield return new WaitForSeconds(0.05f);
        }


        // 뽑기 중 해제
        _isRolling = false;

        if (!_isRolling)
        {
            _gachaView._openAllCard.gameObject.SetActive(true);
        }

    }

    // 모두 열기 기능 함수
    private IEnumerator CO_OpenAllCard()
    {
        _gachaView._openAllCard.gameObject.SetActive(false);

        for (int i = 0; i < _cardList.Count; i++)
        {
            _cardList[i].ReverseCard();

            yield return new WaitForSeconds(0.05f);
        }

        _gachaView._closeButton.gameObject.SetActive(true);
    }


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
}
