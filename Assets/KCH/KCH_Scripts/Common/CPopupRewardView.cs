using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CPopupRewardView : MonoBehaviour
{
    #region 인스펙터
    [Header("애니메이션 이펙트 대상")]
    public List<RectTransform> PopupAnimation = new List<RectTransform>();  // 팝업 애니메이션 대상

    [Header("단일 보상 팝업 UI 요소")]
    public GameObject SingleGroup;                                          // 단일 보상 그룹
    public Image RewardIcon;                                                // 팝업 아이콘
    public TMP_Text AmountText;                                             // 팝업 보상 텍스트

    [Header("다중 보상 팝업 UI 요소")]
    public GameObject ListGroup;                                            // 다중 보상 그룹
    public GameObject MultiPrefab;                                          // 다중 보상 프리팹
    public Transform PopupGroup;                                            // 팝업 그룹 위치
    public TMP_Text TitleText;                                              // 팝업 타이틀

    [Header("보상 아이콘 리소스")]
    public CRewardDataSO RewardIconSO;                                      // 보상 SO
    #endregion

    #region 내부 변수
    private Coroutine _autoCloseCoroutine;                                  // 자동 사라짐 팝업
    private bool _canInput = false;                                         // 팝업 가능 여부
    #endregion
    private void Start()
    {
        Invoke("EnableInput", 0.3f);
    }

    private void Update()
    {
        if (_canInput && Input.GetMouseButtonDown(0))
        {
            ClosePopup();
        }
    }

    private void EnableInput() => _canInput = true;

    // 단일 보상 팝업
    public void SetPopup(string title, Sprite icon, int amount)
    {
        // 제목 설정
        if(TitleText != null)
        {
            TitleText.text = title;
        }

        // 단일 오브젝트 활성화
        SingleGroup.SetActive(true);
        ListGroup.SetActive(false);

        // 아이콘 설정
        if (RewardIcon != null)
        {
            RewardIcon.sprite = icon;
        }

        // 수량 설정
        if (AmountText != null)
        {
            AmountText.text = $"{amount}";
        }

        // 올라오는 코루틴
        PlayAnims();
        StartTimer();
    }

    // 다중 보상 팝업
    public void SetPopupList(string title, List<SQuestReward> rewards, CRewardDataSO data)
    {
        if (TitleText)
        {
            TitleText.text = title;
        }

        SingleGroup.SetActive(false);
        ListGroup.SetActive(true);

        // 기존 오브젝트 역순 삭제
        for (int i = PopupGroup.childCount - 1; i >= 0; i--)
        {
            Destroy(PopupGroup.GetChild(i).gameObject);
        }


        // 중복체크
        Dictionary<EQuestReward, int> rewardsDict = new Dictionary<EQuestReward, int>();

        for (int i = 0; i < rewards.Count; i++)
        {
            EQuestReward type = rewards[i].Type;
            int amount = rewards[i].Amount;

            // 중복시 누적
            if (rewardsDict.ContainsKey(type))
            {
                rewardsDict[type] += amount;
            }

            // 아니면 추가
            else
            {
                rewardsDict.Add(type, amount);
            }

        }

        foreach (var reward in rewardsDict)
        {
            // 프리팹 생성
            if (MultiPrefab == null) continue;

            // 프리팹 캐싱 컴포넌트 사용
            GameObject item = Instantiate(MultiPrefab, PopupGroup);

            RectTransform rect = item.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localScale = Vector3.one;
                rect.anchoredPosition3D = Vector3.zero;
            }

            CRewardItemSlot slot = item.GetComponent<CRewardItemSlot>();

            if (slot != null)
            {

                var rewardData = data.GetRewardData(reward.Key);
                // 아이템 세팅
                slot.SetItem(data.GetIcon(reward.Key), reward.Value, slot.Background.sprite, slot.Outline.sprite);
            }
        }

        PlayAnims();
        StartTimer();
    }

    private void PlayAnims()
    {
        // 올라오는 코루틴
        for (int i = 0; i < PopupAnimation.Count; i++)
        {
            StartCoroutine(CO_SlideUpMenu(PopupAnimation[i]));
        }
    }

    // 타이머 시작
    private void StartTimer()
    {
        if (_autoCloseCoroutine != null)
        {
            StopCoroutine(_autoCloseCoroutine);
        }

        // 5초 타이머 시작 코루틴
        _autoCloseCoroutine = StartCoroutine(Co_AutoClose(5f));
    }

    // 5초 대기 코루틴
    private IEnumerator Co_AutoClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePopup();
    }

    // 종료 로직 함수
    private void ClosePopup()
    {
        if (_autoCloseCoroutine != null)
        {
            StopCoroutine(_autoCloseCoroutine);
            Destroy(gameObject);
        }
    }

    // 슬라이드 올라오는 연출
    private IEnumerator CO_SlideUpMenu(RectTransform menu)
    {
        if (menu == null) yield break;

        Vector2 startPos = new Vector2(0, -400f);
        Vector2 targetPos = new Vector2(0, -100f);

        menu.anchoredPosition = startPos;

        float duration = 0.4f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // 커브 함수
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            menu.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothProgress);

            yield return null;
        }

        menu.anchoredPosition = targetPos;
    }
}


