using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CGachaResultCard : MonoBehaviour
{
    #region 인스펙터
    [Serializable]
    public class RarityEffectSet
    {
        public CGachaDataSO.ERarity _rarity;
        public Sprite _backSprite;
        public GameObject _idleEffect;
    }

    [Header("Card UI 컴포넌트")]
    [SerializeField] private Image _unitBackgroundImage;        // 카드 등급별 배경색
    [SerializeField] private Image _unitBorderImage;            // 카드 테두리
    [SerializeField] private Image _unitIconImage;              // 유닛 초상화
    [SerializeField] private TMP_Text _nameText;                // 유닛 이름

    [Header("Card 연출")]
    [SerializeField] private GameObject _backCard;              // 카드 뒷면 
    [SerializeField] private Button _cardButton;                // 카드 뒷면 클릭 버튼
    [SerializeField] private RectTransform _Movingcard;         // 움직일 자식 카드

    [Header("Card 등급별 설정")]
    [SerializeField] private Image _backCardImage;              // 등급별 카드 뒷면
    [SerializeField] private List<RarityEffectSet> _effectSet;  // 등급별 카드 뒷면
    #endregion

    #region 내부 변수
    private bool _isReversed = false;                           // 카드의 뒤집힘 여부 
    #endregion

    private void Awake()
    {
        _cardButton.onClick.AddListener(ReverseCard);
    }

    public void SetHidden(CGachaDataSO data)
    {
        SetData(data);

        for (int i = 0; i < _effectSet.Count; i++)
        {
            if (_effectSet[i]._rarity == data._rarity)
            {
                _backCardImage.sprite = _effectSet[i]._backSprite;

                ResetEffect();

                if (_effectSet[i]._idleEffect != null)
                {
                    _effectSet[i]._idleEffect.SetActive(true);
                }
            }
        }

        
        _isReversed = false;
        _backCard.SetActive(true);
    }

    private void ResetEffect()
    {
        for (int i = 0; i < _effectSet.Count; i++)
        {
            if (_effectSet[i]._idleEffect != null)
            {
                _effectSet[i]._idleEffect.SetActive(false);
            }
        }
    }

    public void HideVisual() => _Movingcard.localScale = Vector3.zero;

    public void ShowVisual() => _Movingcard.localScale = Vector3.one;

    public void SpawnEffect()
    {
        StartCoroutine(CO_SpawnCard());
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
            
            _Movingcard.transform.localPosition = Vector3.Lerp(startPos, targetPos, move);

            yield return null;
        }

        _Movingcard.transform.localPosition = targetPos;
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

    private IEnumerator CO_FilpCard()
    {
        // 뒤집히는 시간
        float duration = 0.05f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float x = Mathf.Lerp(1f, 0f, timer / duration);
            
            transform.localScale = new Vector3(x, 1f, 1f);

            yield return null;
        }

        _backCard.SetActive(false);

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

    // Presenter에서 호출
    public void SetData(CGachaDataSO data)
    {
        if (data == null)
        {
            return;
        }

        // 유닛 이름
        if (_nameText != null)
        {
            _nameText.text = data._unitName;
        }

        // 유닛 초상화 아이콘
        if (_unitIconImage != null)
        {
            _unitIconImage.sprite = data._unitIcon;
        }

        // 등급별 배경 이미지
        if (_unitBackgroundImage != null)
        {
            _unitBackgroundImage.sprite = data._unitBackground;
        }

        // 등급별 테두리 이미지
        if (_unitBorderImage != null)
        {
            _unitBorderImage.sprite = data._unitBorder;
        }

    }

}
