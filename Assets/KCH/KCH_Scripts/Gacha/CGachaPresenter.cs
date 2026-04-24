using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CGachaPresenter : MonoBehaviour
{
    public static CGachaPresenter Instance { get; private set; }

    #region 인스펙터
    [Header("Gacha 시스템 연결")]
    [SerializeField] private CGachaModel _gachaModel;                           // 모델 연결 
    [SerializeField] private CGachaView _gachaView;                             // 뷰 연결

    [Header("Gacha 카테고리 List 설정")]
    [SerializeField] private List<CGachaCategorySO> _gachaCategoryList;         // 소환 리스트 SO (영웅SO : 0, 펫SO : 1)
    [SerializeField] private CTabChange _tabChange;                             // 카테고리 탭 변경
    [SerializeField] private int _currentCategoryIndex = 0;                     // 카테고리 현재 인덱스
    [SerializeField] private Sprite _petBackSprite;                             // 펫 전용 이미지

    [Header("Card 프리팹 설정")]
    [SerializeField] private GameObject _cardPrefab;                            // 카드 프리팹
    [SerializeField] private GameObject _miniCardPrefab;                        // 카드 프리팹

    [Header("Card 오디오 설정")]
    [SerializeField] private UIAudioSO _cardAudioSet;                           // 카드 두는 오디오
    [SerializeField] private UIAudioSO _buttonAudioSet;                         // 버튼 두는 오디오
    #endregion

    #region 내부 변수
    private List<CGachaResultCard> _cardList = new List<CGachaResultCard>();    // 카드 리스트
    private Sprite _ticketSprite;                                               // 소환권 이미지
    private Sprite _rubySprite;                                                 // 루비 이미지
    private bool _isRolling = false;                                            // 뽑기 중 유무
    private bool _isAutoRoll = false;                                           // 자동 소환 유무
    private bool _isLegendShow = false;                                         // 레전드 연출 중 유무
    private int _currentCount = 0;                                              // 뽑기중인 개수
    private bool _isFirstInit = true;                                           // 처음 갱신
    #endregion

    public bool IsRolling => _isRolling;
    public bool IsLegendShow => _isLegendShow;

    public bool IsClickCard => !_isRolling && !_isLegendShow;


    private void Awake()
    {
        Instance = this;

        // 버튼 리스너 연결
        // 뽑기 버튼
        _gachaView.GachaOneButton.onClick.AddListener(() => OnClickButton(1));
        _gachaView.GachaTenButton.onClick.AddListener(() => OnClickButton(10));
        _gachaView.GachaThirtyButton.onClick.AddListener(() => OnClickButton(30));
        _gachaView.GachaTHundredButton.onClick.AddListener(() => OnClickButton(300));

        // 재뽑기 버튼
        _gachaView.ReRollTenButton.onClick.AddListener(() => OnClickButton(10));
        _gachaView.ReRollThirtyButton.onClick.AddListener(() => OnClickButton(30));
        _gachaView.ReRollMiniThirtyButton.onClick.AddListener(() => OnClickButton(30));
        _gachaView.ReRollMiniHThirtyButton.onClick.AddListener(() => OnClickButton(300));

        // 모두 열기 버튼
        _gachaView.OpenAllCard.onClick.AddListener(() => StartCoroutine(CO_OpenAllCard()));

        // 자동 소환 토글 버튼
        _gachaView.AllOpenAutoGachaButton.onClick.AddListener(ToggleAutoRoll);
        _gachaView.AutoGachaButton.onClick.AddListener(ToggleAutoRoll);

        // 닫기 버튼
        _gachaView.CloseButton.onClick.AddListener(OnClickClose);
        _gachaView.MiniCardButton.onClick.AddListener(OnClickClose);

        // 화살표
        _gachaView.HeroCategoryButtonR.onClick.AddListener(SwitchCategory);
        _gachaView.HeroCategoryButtonL.onClick.AddListener(SwitchCategory);

        // 처음에는 뽑기창 비활성화
        _gachaView.ResultPanel.SetActive(false);
    }


    private void Start()
    {
        if (CQuestManager.Instance != null)
        {
            CQuestManager.Instance.OnDataUpdate += UpdateMoneyUI;
        }

        if (_tabChange != null)
        {
            // 탭 이벤트 연결
            _tabChange.OnTabChange += ChangeCatergory;

            // 시작 시 0번 탭
            _tabChange.SelectTab(0);
        }

        // 재화 표시
        UpdateMoneyUI();
    }

    private void OnDestroy()
    {
        if (CQuestManager.Instance != null)
        {
            CQuestManager.Instance.OnDataUpdate -= UpdateMoneyUI;
        }

        if (_tabChange != null)
        {
            _tabChange.OnTabChange -= ChangeCatergory;
        }
    }

    // 인덱스에 따라 소환 카테고리 변경
    private void ChangeCatergory(int index)
    {
        if (!_isRolling && !_isFirstInit)
        {
            SoundManager.Instance.PlayUISFX(_buttonAudioSet.uiOn);
        }

        // 소리 체크 끝
        _isFirstInit = false;

        // 현재 인덱스 선택
        _currentCategoryIndex = index;
        // 카테고리 선택
        CGachaCategorySO select = _gachaCategoryList[index];

        // 카테고리 세팅
        _gachaModel.SetCategory(select, index);

        // 배너 이미지 교체
        _gachaView.MainShopImage.sprite = select.MainShopImage;

        // 카테고리 UI 업데이트
        UpdateCategoryUI();
    }

    // 뽑기 버튼 클릭
    private void OnClickButton(int count)
    {
        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);


        Debug.Log($"버튼 클릭됨! 현재 count: {count}, isRolling: {_isRolling}");
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
            // 재화 부족 팝업
            NotEnoughMoneyPopup();
        }

    }

    // 자동 뽑기 토글 함수
    private void ToggleAutoRoll()
    {
        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);
        _isAutoRoll = !_isAutoRoll;

        if (_gachaView.AutoGachaCheckIcon != null)
        {
            _gachaView.AutoGachaCheckIcon.gameObject.SetActive(_isAutoRoll);
        }

        if (_gachaView.AllOpenAutoGachaCheckIcon != null)
        {
            _gachaView.AllOpenAutoGachaCheckIcon.gameObject.SetActive(_isAutoRoll);
        }

        // 깜빡이는 코루틴
        if (_isAutoRoll)
        {
            StartCoroutine(CO_PlayAutoGachaBlick());
        }
    }

    // 결과 창 닫고 초기화
    private void OnClickClose()
    {
        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);

        // 자동 소환
        if (_isAutoRoll)
        {
            StopAutoRoll();

            return;
        }

        _gachaView.ResultPanel.SetActive(false);
        _gachaView.MiniCardPanel.SetActive(false);
        _isRolling = false;
    }

    // 재화 부족 팝업
    private void NotEnoughMoneyPopup()
    {

        if (_isAutoRoll)
        {
            _gachaView.MsgPopupText.text = "재화가 부족하여 자동 소환을 종료합니다.";
        }

        else
        {
            _gachaView.MsgPopupText.text = "소환의 필요한 재화가 부족 합니다.";
        }
        CanvasGroup canvasGroup = _gachaView.MsgPopupPanel.GetComponent<CanvasGroup>();

        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        _gachaView.MsgPopupPanel.SetActive(true);

        StopCoroutine("CO_HidePopup");
        StartCoroutine("CO_HidePopup");
    }

    // 레전드 이펙트 팝업
    private IEnumerator CO_ShowLegendEffect(CGachaDataSO data)
    {
        yield return new WaitForSeconds(0.5f);

        _gachaView.LegendPopup.SetActive(true);

        _gachaView.LegendTimerText.text = "";

        float nameTargetY = -250f;
        float nameStartY = nameTargetY + 50f;

        if (data != null)
        {
            // 초기 세팅
            _gachaView.LegendIllust.skeletonDataAsset = data.IllustAsset;
            _gachaView.LegendSD.skeletonDataAsset = data.SDAsset;

            _gachaView.LegendIllust.Initialize(true);
            _gachaView.LegendSD.Initialize(true);

            Vector2 targetPos = data.IllustOffset;
            Vector2 startPos = targetPos + new Vector2(0, -100f);

            _gachaView.LegendIllust.rectTransform.anchoredPosition = startPos;
            _gachaView.LegendIllust.rectTransform.localScale = Vector3.one * (data.IllustScale * 1.1f);
            _gachaView.LegendIllust.color = Color.black;

            // UI 그룹 안보이게
            if (_gachaView.LegendNameGroup != null)
            {
                _gachaView.LegendNameGroup.gameObject.SetActive(false);
                _gachaView.LegendNameGroup.alpha = 0f;
            }

            if (_gachaView.LegendBottomGroup != null)
            {
                _gachaView.LegendBottomGroup.gameObject.SetActive(false);
                _gachaView.LegendBottomGroup.alpha = 0f;
            }

            yield return new WaitForSeconds(0.1f);

            // 일러스트 줌 아웃, 점점 밝아짐
            float mainDuration = 0.6f;
            float mainTimer = 0f;

            while (mainTimer < mainDuration)
            {
                mainTimer += Time.deltaTime;

                float ratio = mainTimer / mainDuration;
                float curve = Mathf.SmoothStep(0, 1, ratio);

                _gachaView.LegendIllust.rectTransform.localScale = Vector3.Lerp(Vector3.one * (data.IllustScale * 1.1f), Vector3.one * data.IllustScale, curve);
                _gachaView.LegendIllust.color = Color.Lerp(Color.black, Color.white, curve);

                yield return null;
            }

            // 일러스트 위로 이동
            float bottomDuration = 0.25f;
            float bottomTimer = 0f;

            while (bottomTimer < bottomDuration)
            {
                bottomTimer += Time.deltaTime;
                float ratio = bottomTimer / bottomDuration;
                float curve = Mathf.SmoothStep(0, 1, ratio);

                _gachaView.LegendIllust.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, curve);

                yield return null;  
            }

            _gachaView.LegendIllust.rectTransform.anchoredPosition = targetPos;

            // 이름, 하단 그룹 활성화
            if (_gachaView.LegendBottomGroup != null)
            {
                _gachaView.LegendNameText.text = $"{data.UnitName}";
                _gachaView.LegendNameGroup.gameObject.SetActive(true);
                _gachaView.LegendNameGroup.alpha = 0f;
            }

            if (_gachaView.LegendBottomGroup != null)
            {
                _gachaView.LegendBottomGroup.gameObject.SetActive(true);
                _gachaView.LegendBottomGroup.alpha = 1f;
            }

            // 이름표 내려오는 애니메이션
            float nameDuration = 0.3f;
            float nameTimer = 0f;
            while (nameTimer < nameDuration)
            {
                nameTimer += Time.deltaTime;
                float ratio = nameTimer / nameDuration;
                float curve = Mathf.SmoothStep(0, 1, ratio);

                if (_gachaView.LegendNameGroup != null)
                {
                    _gachaView.LegendNameGroup.alpha = ratio;
                    float currentY = Mathf.Lerp(nameStartY, nameTargetY, curve);
                    _gachaView.LegendNameGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentY);
                }

                yield return null;
            }

            var AnimSpeed = _gachaView.LegendSD.AnimationState.SetAnimation(0, "Appear", false);
            AnimSpeed.TimeScale = 0.3f;

            _gachaView.LegendSD.AnimationState.AddAnimation(0, "Idle", true, 0.2f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 1; i <= 10; i++)
        {
            _gachaView.LegendTimerText.text = $"{11 - i}초 후 다음 결과로 넘어갑니다.";

            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;

                // 꺼질 때 까지
                if (!_gachaView.LegendPopup.activeSelf || Input.GetMouseButtonDown(0))
                {
                    break;
                }

                yield return null;
            }

            if (!_gachaView.LegendPopup.activeSelf || Input.GetMouseButtonDown(0))
            {
                break;
            }
        }

        _gachaView.LegendPopup.SetActive(false);
        _isLegendShow = false;
    }

    // 팝업을 점점 투명하게 변경 코루틴
    private IEnumerator CO_HidePopup()
    {
        yield return new WaitForSeconds(1.5f);

        CanvasGroup canvasGroup = _gachaView.MsgPopupPanel.GetComponent<CanvasGroup>();

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

        _gachaView.MsgPopupPanel.SetActive(false);
    }

    // 자동 뽑기 깜빡임 연출
    private IEnumerator CO_PlayAutoGachaBlick()
    {
        CanvasGroup autoCanvas = _gachaView.AutoGachaPopup.GetComponent<CanvasGroup>();

        // 알파 값 변경
        while (_isAutoRoll)
        {
            if (autoCanvas != null)
            {
                autoCanvas.alpha = Mathf.PingPong(Time.time * 0.5f, 1f);

                yield return null;
            }

        }

        // 알파 값 복구
        if (autoCanvas != null)
        {
            autoCanvas.alpha = 1f;
            _gachaView.AutoGachaPopup.SetActive(false);
        }
    }

    // 슬라이드 올라오는 연출
    private IEnumerator CO_SlideUpMenu(RectTransform menu)
    {
        Vector2 startPos = new Vector2(0, -400f);
        Vector2 targetPos = new Vector2(0, 50f);

        menu.anchoredPosition = startPos;
        menu.gameObject.SetActive(true);

        float duration = 0.5f;
        float timer = 0f;

        while(timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // 커브 함수
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            menu.anchoredPosition = Vector2.Lerp(startPos, targetPos  , smoothProgress);

            yield return null;
        }

        menu.anchoredPosition = targetPos;
    }

    // 뽑기 연출, 데이터 처리 코루틴
    private IEnumerator Co_GachaClick(int count)
    {
        _currentCount = count;

        // 재화 검사 차감
        if (!_gachaModel.CheckRuby(count))
        {
            Debug.Log("재화가 없다..");
            yield break;
        }

        _gachaModel.PayRuby(count);
        UpdateMoneyUI();

        // 자동 뽑기 시 닫기만 출력
        if (_isAutoRoll)
        {
            RectTransform target = (count == 300) ? _gachaView.GachaMiniMenu : _gachaView.GachaMenu;

            target.anchoredPosition = new Vector2(0, 50f);
            target.gameObject.SetActive(true);

            _gachaView.ReRollTenButton.gameObject.SetActive(false);
            _gachaView.ReRollThirtyButton.gameObject.SetActive(false);

            _gachaView.ReRollMiniThirtyButton.gameObject.SetActive(false);
            _gachaView.ReRollMiniHThirtyButton.gameObject.SetActive(false);

            _gachaView.AutoGachaButton.gameObject.SetActive(false);
            _gachaView.AllOpenAutoGachaButton.gameObject.SetActive(false);

            _gachaView.CloseButton.gameObject.SetActive(true);
        }

        else
        {
            _gachaView.GachaMenu.gameObject.SetActive(false);
            _gachaView.GachaMiniMenu.gameObject.SetActive(false);
        }

        // 뽑기 중 활성
        _isRolling = true;

        // 뽑을 횟수 
        List<CGachaDataSO> results = _gachaModel.RollGacha(count);

        // 경험치 증가
        UpdateCategoryUI();

        // 300회 뽑기 시 로직
        if (count == 300)
        {
            _gachaView.MiniCardPanel.SetActive(true);
            _gachaView.ResultPanel.SetActive(false);
     
            // 미니 카드 풀에 반납 (역순)
            for (int i = _gachaView.MiniCardTransform.childCount - 1; i >= 0; i--)
            {
                PoolManager.Instance.Push(_miniCardPrefab, _gachaView.MiniCardTransform.GetChild(i).gameObject);
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
            keys.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));

            // 정렬된 순으로 미니 카드 생성
            for (int i = 0; i < keys.Count; i++)
            {
                CGachaDataSO key = keys[i];
                GameObject miniCard = PoolManager.Instance.Pop(_miniCardPrefab, Vector3.zero, Quaternion.identity);

                miniCard.transform.SetParent(_gachaView.MiniCardTransform, false);
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
            SoundManager.Instance.PlayUISFX(_cardAudioSet.uiOff);

            UpdateMoneyUI();

            _isRolling = false;

            if (!_isAutoRoll)
            {
                _gachaView.ReRollMiniThirtyButton.gameObject.SetActive(true);
                _gachaView.ReRollMiniHThirtyButton.gameObject.SetActive(true);
                _gachaView.AutoGachaButton.gameObject.SetActive(true);
                _gachaView.CloseButton.gameObject.SetActive(true);

                StartCoroutine(CO_SlideUpMenu(_gachaView.GachaMiniMenu));
            }

            else
            {
                _gachaView.ReRollMiniThirtyButton.gameObject.SetActive(false);
                _gachaView.ReRollMiniHThirtyButton.gameObject.SetActive(false);
            }

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
            _gachaView.ResultPanel.SetActive(true);
            _gachaView.MiniCardPanel.SetActive(false);

            for (int i = 0; i < results.Count; i++)
            {
                // 프리펩 생성
                GameObject card = PoolManager.Instance.Pop(_cardPrefab, Vector3.zero, Quaternion.identity);
                card.transform.SetParent(_gachaView.GachaTransform, false);
                card.transform.SetAsLastSibling();

                card.transform.localScale = Vector3.one;
                card.transform.localPosition = Vector3.zero;

                CGachaResultCard cardUI = card.GetComponent<CGachaResultCard>();

                // 카드 UI에 데이터 세팅
                if (cardUI != null)
                {

                    // 뒷면 카드 세팅
                    cardUI.SetHidden(results[i]);

                    // 펫 소환 이면 뒷면 변경
                    if (_currentCategoryIndex == 1 && _petBackSprite != null)
                    {
                        cardUI.SetBackSprite(_petBackSprite);
                    }

                    // 카드 위치 먼저 세팅
                    cardUI.HideVisual();

                    // 이벤트 구독 초기화
                    cardUI.OnFliped -= FilpLegendCard;

                    // 이벤트 구독
                    cardUI.OnFliped += FilpLegendCard;

                    cardUI.OnFlipStart = () =>
                    {
                        SoundManager.Instance.PlayUISFX(_cardAudioSet.uiOff);
                    };

                    _cardList.Add(cardUI);
                }
            }

            for (int i = 0; i < _cardList.Count; i++)
            {
                // 카드 보여 주기      
                _cardList[i].ShowVisual();
                // 카드 내려오는 이펙트
                _cardList[i].SpawnEffect();

                SoundManager.Instance.PlayUISFX(_cardAudioSet.buttonClick);

                // 순번으로 출력
                yield return new WaitForSeconds(0.05f);
            }

            // 카드 세팅 대기
            yield return new WaitForSeconds(0.3f);

            // 뽑기 중 해제
            _isRolling = false;

            UpdateMoneyUI();

            // 모두 열기 버튼 표시
            if (!_isAutoRoll)
            {
                _gachaView.OpenAllCard.gameObject.SetActive(true);
            }

        }

        // 이벤트 전달
        if (_currentCategoryIndex == 0)
        {
            CQuestEvent.Publish(EQuestType.GachaSummon, count);
        }
        else
        {
            CQuestEvent.Publish(EQuestType.PetSummon, count);
        }

        // 자동 뽑기 시 재 시작
        if (_isAutoRoll)
        {
            // 깜빡이는 팝업
            _gachaView.AutoGachaPopup.SetActive(true);
            StartCoroutine(CO_PlayAutoGachaBlick());

            // 300회 제외 카드 오픈
            if (count < 300)
            {
                yield return StartCoroutine(CO_OpenAllCard());

                yield return new WaitForSeconds(0.5f);
            }

            // 300회
            else
            {
                yield return new WaitForSeconds(1.5f);
            }

            // 재뽑기 재귀
            if (_isAutoRoll && _gachaModel.CheckRuby(count))
            {
                StartCoroutine(Co_GachaClick(count));
            }

            // 재화 부족
            else
            {
                StopAutoRoll();

                if (count == 300)
                {
                    _gachaView.ReRollMiniThirtyButton.gameObject.SetActive(true);
                    _gachaView.ReRollMiniHThirtyButton.gameObject.SetActive(true);
                    _gachaView.AutoGachaButton.gameObject.SetActive(true);
                    StartCoroutine(CO_SlideUpMenu(_gachaView.GachaMiniMenu));
                }

                if (!_gachaModel.CheckRuby(count))
                {
                    NotEnoughMoneyPopup();
                }
            }
        }
    }

    // 모두 열기 기능 함수
    private IEnumerator CO_OpenAllCard()
    {
        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);

        _gachaView.OpenAllCard.gameObject.SetActive(false);

        _isRolling = true;

        for (int i = 0; i < _cardList.Count; i++)
        {
            // 뒤집힌 카드 스킵
            if (_cardList[i].IsReversed)
            {
                continue;
            }

            // 카드 뒤집기 로직
            _cardList[i].AllReverseCard();
            SoundManager.Instance.PlayUISFX(_cardAudioSet.uiOff);

            bool isLegend = _currentCategoryIndex == 0 && _cardList[i].CurrentData.Rarity == CGachaDataSO.ERarity.Legend;

            // 레전드 카드
            if (isLegend)
            {
                yield return new WaitForSeconds(0.15f);
                SoundManager.Instance.PlayUISFX(_cardAudioSet.heroPickUp);

                // 팝업 띄워진동안 대기
                while (_isLegendShow)
                {
                    yield return null;
                }
            }

            // 레전드 카드 제외
            else
            {
                yield return new WaitForSeconds(0.03f);
            }
        }

        UpdateMoneyUI();

        if (!_isAutoRoll && !_gachaView.GachaMenu.gameObject.activeSelf)
        {
            // 버튼 활성화
            _gachaView.AllOpenAutoGachaButton.gameObject.SetActive(true);
            _gachaView.ReRollTenButton.gameObject.SetActive(true);
            _gachaView.ReRollThirtyButton.gameObject.SetActive(true);
            _gachaView.CloseButton.gameObject.SetActive(true);

            // 슬라이드로 메뉴 활성화
            StartCoroutine(CO_SlideUpMenu(_gachaView.GachaMenu));
        }

        _isRolling = false;
    }

    // 레전드 카드 판정 함수
    private void FilpLegendCard(CGachaResultCard card)
    {
        if (_currentCategoryIndex == 0 && card.CurrentData.Rarity == CGachaDataSO.ERarity.Legend)
        {
            _isLegendShow = true;

            StartCoroutine(CO_ShowLegendEffect(card.CurrentData));
        }

        CheckAllCardsOpend();
    }

    // 카드 뒤집힘 확인 함수
    private void CheckAllCardsOpend()
    {
        if (_currentCount == 300)
        {
            return;
        }

        for (int i = 0; i < _cardList.Count; i++)
        {
            if (!_cardList[i].IsReversed)
            {
                return;
            }
        }

        // 자동 소환중 리턴
        if (_isAutoRoll)
        {
            return;
        }

        _gachaView.OpenAllCard.gameObject.SetActive(false);
        UpdateMoneyUI();

        _gachaView.AllOpenAutoGachaButton.gameObject.SetActive(true);
        _gachaView.ReRollTenButton.gameObject.SetActive(true);
        _gachaView.ReRollThirtyButton.gameObject.SetActive(true);
        _gachaView.CloseButton.gameObject.SetActive(true);

        // 슬라이드로 메뉴 활성화
        StartCoroutine(CO_SlideUpMenu(_gachaView.GachaMenu));
    }

    // 자동 소환 종료 함수
    private void StopAutoRoll()
    {
        _isAutoRoll = false;

        if (_gachaView.AutoGachaPopup != null)
        {
            _gachaView.AutoGachaPopup.SetActive(false);
        }

        // 체크 아이콘 표시
        if (_gachaView.AutoGachaCheckIcon != null)
        {
            _gachaView.AutoGachaCheckIcon.gameObject.SetActive(false);
        }

        if (_gachaView.AllOpenAutoGachaCheckIcon != null)
        {
            _gachaView.AllOpenAutoGachaCheckIcon.gameObject.SetActive(false);
        }
    }

    // 카테고리 경험치 게이지 업데이트
    private void UpdateCategoryUI()
    {
        CGachaCategorySO current = _gachaCategoryList[_currentCategoryIndex];

        int totalExp = (_currentCategoryIndex == 0) ?
            CDataManager.Instance.UserData.HeroPickUpLevel :
            CDataManager.Instance.UserData.PetPickUpLevel;

        Debug.Log($"[Gacha] 현재 카테고리: {current.CategoryName}, 누적 경험치: {totalExp}");

        int currentLevel = current.GetLevel(totalExp);
        int currentExp = current.GetCurrentExp(totalExp);

        int maxExp = 0;
        float fillAmount = 0f;
        string expStr = "";

        if (currentLevel <= current._maxExpTable.Length)
        {
            maxExp = current._maxExpTable[currentLevel - 1];
            fillAmount = (float)currentExp / maxExp;
            expStr = currentExp.ToString("N0") + " / " + maxExp.ToString("N0");
        }

        else
        {
            fillAmount = 1.0f;
            expStr = "MAX";
        }

        if (_currentCategoryIndex == 0)
        {
            _gachaView.LevelTextHero.text = current.CategoryName + "소환 레벨 " + currentLevel;
            _gachaView.ExpFillImageHero.fillAmount = fillAmount;
            _gachaView.ExpTextHero.text = expStr;
            _gachaView.HeroTab.gameObject.SetActive(true);
            _gachaView.PetTab.gameObject.SetActive(false);

            
        }

        if (_currentCategoryIndex == 1)
        {
            _gachaView.LevelTextPet.text = current.CategoryName + "소환 레벨 " + currentLevel;
            _gachaView.ExpFillImagePet.fillAmount = fillAmount;
            _gachaView.ExpTextPet.text = expStr;

            if (_gachaView.HeroTab != null && _gachaView.HeroTab.gameObject.activeSelf)
            {
                _gachaView.PetTab.gameObject.SetActive(true);
                _gachaView.HeroTab.gameObject.SetActive(false);
            }
        }

        _gachaView.HeroTabGroup.alpha = (_currentCategoryIndex == 0) ? 1.0f : 0.0f;
        _gachaView.PetTabGroup.alpha = (_currentCategoryIndex == 1) ? 1.0f : 0.0f;
    }

    // 재화 정보  UI 업데이트
    private void UpdateMoneyUI()
    {
        _gachaView.SummonRuby.text = _gachaModel.RubyCount.ToString("");
        _gachaView.SummonCard.text = _gachaModel.TicketCount.ToString("");

        // 뽑기 별로 비용, 이미지 변경
        SetButtonCostUI(_gachaView.GachaOneButton, 1);
        SetButtonCostUI(_gachaView.GachaTenButton, 10);
        SetButtonCostUI(_gachaView.GachaThirtyButton, 30);
        SetButtonCostUI(_gachaView.GachaTHundredButton, 300);

        SetButtonCostUI(_gachaView.ReRollTenButton, 10);
        SetButtonCostUI(_gachaView.ReRollThirtyButton, 30);

        if (_gachaView.ReRollMiniThirtyButton != null)
        {
            SetButtonCostUI(_gachaView.ReRollMiniThirtyButton, 30);
        }

        if (_gachaView.ReRollMiniHThirtyButton != null)
        {
            SetButtonCostUI(_gachaView.ReRollMiniHThirtyButton, 300);
        }
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
        if (_gachaModel.TicketCount >= count)
        {
            costText.text = count.ToString();
            iconImage.sprite = _gachaView.TicketIcon;
            buttonImage.sprite = _gachaView.NormalGachaButton;
        }

        // 소환권 부족, 루비 충분
        else if (_gachaModel.RubyCount >= rubyPrice)
        {
            costText.text = rubyPrice.ToString();
            iconImage.sprite = _gachaView.RubyIcon;
            buttonImage.sprite = _gachaView.NormalGachaButton;
        }

        // 소환권 부족, 루비 부족
        else
        {
            costText.text = rubyPrice.ToString();
            iconImage.sprite = _gachaView.RubyIcon;
            buttonImage.sprite = _gachaView.DisableGachaButton;
        }

        // 재화 부족 시 버튼 비활성화
        if (_gachaModel.CheckRuby(count))
        {
            costText.color = Color.white;
            buttonImage.sprite = _gachaView.NormalGachaButton;
            buttonTransform.sizeDelta = _gachaView.NormalButtonSize;
        }

        else
        {
            costText.color = Color.white;
            buttonImage.sprite = _gachaView.DisableGachaButton;
            buttonTransform.sizeDelta = _gachaView.DisabledButtonSize;
        }
    }

    // 화살표 스위칭
    private void SwitchCategory()
    {
        SoundManager.Instance.PlayUISFX(_buttonAudioSet.buttonClick);

        if (_isRolling)
        {
            return;
        }

        int nextIndex = (_currentCategoryIndex == 0) ? 1 : 0;

        if (_tabChange != null)
        {
            _tabChange.SelectTab(nextIndex);
        }
        else
        {
            ChangeCatergory(nextIndex);
        }

        UpdateCategoryUI();
    }
    
}
