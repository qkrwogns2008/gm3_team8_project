using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 뽑기 결과로 보여질 카드의 비주얼과 애니메이션 표현 로직을 관리하는 클래스입니다.
/// </summary>
public class CGachaResultCard : MonoBehaviour
{
    #region 인스펙터
    [Serializable]
    public class RarityEffectSet
    {
        public CGachaDataSO.ERarity Rarity;                     // 대상 등급
        public Sprite BackSprite;                               // 해당 등급 뒷면 이미지
        public GameObject IdleEffect;                           // 해당 등급 이펙트
        public GameObject ReverseEffect;                        // 해당 등급 뒤집힘 이펙트
    }

    [Header("Card UI 컴포넌트")]
    [SerializeField] private Image _unitBackgroundImage;        // 카드 등급별 배경색
    [SerializeField] private Image _unitBorderImage;            // 카드 테두리
    [SerializeField] private Image _unitIconImage;              // 유닛 초상화
    [SerializeField] private TMP_Text _nameText;                // 유닛 이름

    [Header("Card 연출")]
    [SerializeField] private GameObject _backCard;              // 카드 뒷면 
    [SerializeField] private Button _cardButton;                // 카드 뒷면 클릭 버튼
    [SerializeField] private RectTransform _movingCard;         // 움직일 자식 카드

    [Header("Card 등급별 설정")]
    [SerializeField] private Image _backCardImage;              // 등급별 카드 뒷면
    [SerializeField] private List<RarityEffectSet> _effectSet;  // 등급별 카드 뒷면
    #endregion

    #region 내부 변수
    private Sprite _originBackSprite;                           // 카드 기본 뒷면
    private bool _isReversed = false;                           // 카드의 뒤집힘 여부 
    private CGachaDataSO _currentData;                          // 현재 카드 데이터
    #endregion

    private void Awake()
    {
        // 카드 뒤집기 함수 연결
        _cardButton.onClick.AddListener(ReverseCard);

        // 카드 기본 뒷면 저장
        if (_backCardImage)
        {
            _originBackSprite = _backCardImage.sprite;
        }
    }

    //  CGachaDataSO 데이터를 세팅
    public void SetData(CGachaDataSO data)
    {
        if (data == null)
        {
            return;
        }

        // 현재 카드 데이터
        _currentData = data;

        // 유닛 이름
        if (_nameText != null)
        {
            _nameText.text = data.UnitName;
        }

        // 유닛 초상화 아이콘
        if (_unitIconImage != null)
        {
            _unitIconImage.sprite = data.UnitIcon;
        }

        // 등급별 배경 이미지
        if (_unitBackgroundImage != null)
        {
            _unitBackgroundImage.sprite = data.UnitBackground;
        }

        // 등급별 테두리 이미지
        if (_unitBorderImage != null)
        {
            _unitBorderImage.sprite = data.UnitBorder;
        }
    }

    // 일반 뽑기 연출
    public void SetHidden(CGachaDataSO data)
    {
        gameObject.SetActive(true);

        // 이펙트 초기화
        ResetEffect();

        // 기본 뒷면 초기화
        _backCardImage.sprite = _originBackSprite;
        _isReversed = false;

        if (_backCard != null)
        {
            _backCard.SetActive(true);
        }

        SetData(data);
        
        // 에픽(2) 등급 부터 이미지 변경과 이펙트 활성화
        if ((int)data.Rarity >= 2)
        {
            for (int i = 0; i < _effectSet.Count; i++)
            {
                if (_effectSet[i].Rarity == data.Rarity)
                {
                    _backCardImage.sprite = _effectSet[i].BackSprite;

                    if (_effectSet[i].IdleEffect != null)
                    {
                        _effectSet[i].IdleEffect.SetActive(true);
                    }
                    break;
                }
            }
        }

        // 유닛 데이터 세팅
        SetData(data);

        // 비주얼 활성화
        ShowVisual();
    }

    // 300 회 뽑기 연출
    public void SetMiniCard(CGachaDataSO data, int count)
    {
        this.gameObject.SetActive(true);

        // 카드 위치,크기 초기화
        if (_movingCard != null)
        {
            _movingCard.gameObject.SetActive(true);
            _movingCard.localScale = Vector3.one;
            _movingCard.localPosition = Vector3.zero;
        }

        _isReversed = true;

        if (_backCard != null)
        {
            _backCard.SetActive(false);
        }

        SetData(data);

        // 이름 대신 갯수로 출력
        if (_nameText != null)
        {
            _nameText.text = count.ToString();
        }
    }

    // 모든 이펙트 초기화
    private void ResetEffect()
    {
        if (_effectSet == null)
        {
            return;
        }

        for (int i = 0; i < _effectSet.Count; i++)
        {
            if (_effectSet[i] == null)
            {
                continue;
            }

            // 자신 제외 이펙트 오브젝트 비활성화
            if (_effectSet[i].IdleEffect != null && _effectSet[i].IdleEffect != gameObject)
            {
                _effectSet[i].IdleEffect.SetActive(false);
            }
        }

        // 역순 삭제
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (child != _backCard && child != _movingCard.gameObject)
            {
                child.SetActive(false);
                Destroy(child);
            }
        }
    }

    // 카드를 안보이게 크기 조절
    public void HideVisual() => _movingCard.localScale = Vector3.zero;

    // 카들를 보이게 크기 조절
    public void ShowVisual() => _movingCard.localScale = Vector3.one;

    // 카드 소환 이펙트
    public void SpawnEffect()
    {
        // 코루틴 사용시 Active 체크
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CO_SpawnCard());
        }
    }

    // 카드가 떨어지는 함수
    private IEnumerator CO_SpawnCard()
    {
        float duration = 0.3f;
        float timer = 0f;

        Vector3 startPos = new Vector3(0f, 100f, 0f);
        Vector3 targetPos = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float move = timer / duration;

            _movingCard.transform.localPosition = Vector3.Lerp(startPos, targetPos, move);

            yield return null;
        }

        _movingCard.transform.localPosition = targetPos;
    }

    // 카드를 뒤집을 때 함수
    public void ReverseCard()
    {
        if (_isReversed)
        {
            return;
        }

        _isReversed = true;

        // 카드 뒤집히는 연출
        StartCoroutine(CO_FilpCard());
    }
    private void PlayReverseEffect()
    {
        if (_currentData == null || (int)_currentData.Rarity < 2)
        {
            return;
        }

        for (int i = 0; i < _effectSet.Count; i++)
        {
            if (_effectSet[i].Rarity == _currentData.Rarity)
            {
                if (_effectSet[i].ReverseEffect != null)
                {
                    GameObject effect = Instantiate(_effectSet[i].ReverseEffect, transform);

                    effect.transform.localPosition = Vector3.zero;
                    effect.transform.localScale = Vector3.one;

                    effect.SetActive(true);

                    Destroy(effect, 2f);
                }
                break;
            }
        }

    }

    // X축을 늘렸다가 다시 늘려 뒤집히는 연출
    private IEnumerator CO_FilpCard()
    {
        // 뒤집히는 시간
        float duration = 0.05f;
        float timer = 0f;

        // 1 ~ 0
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float x = Mathf.Lerp(1f, 0f, timer / duration);
            
            transform.localScale = new Vector3(x, 1f, 1f);

            yield return null;
        }

        // 0
        _backCard.SetActive(false);

        ResetEffect();

        // 뒤집을때 이펙트
        PlayReverseEffect();

        // 0 ~ 1
        timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;

            float x = Mathf.Lerp(0f, 1f, timer / duration);

            transform.localScale = new Vector3(x, 1f, 1f);

            yield return null;
        }

        transform.localScale = Vector3.one;
    }


}
