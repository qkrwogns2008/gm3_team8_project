using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CGachaResultCard : MonoBehaviour
{

    #region 인스펙터
    [Header("Card UI 컴포넌트")]
    [SerializeField] private Image _unitBackgroundImage;    // 카드 등급별 배경색
    [SerializeField] private Image _unitBorderImage;        // 카드 테두리
    [SerializeField] private Image _unitIconImage;          // 유닛 초상화
    [SerializeField] private TMP_Text _nameText;            // 유닛 이름

    [Header("Card 연출")]
    [SerializeField] private GameObject _backCard;          // 카드 뒷면 
    [SerializeField] private Button _cardButton;            // 카드 뒷면 클릭 버튼
    #endregion

    #region 내부 변수
    private bool _isReversed = false;                       // 카드의 뒤집힘 여부 
    #endregion


    private void Awake()
    {
        _cardButton.onClick.AddListener(ReverseCard);
    }

    public void SetHidden(CGachaDataSO data)
    {
        SetData(data);
        
        _isReversed = false;
        _backCard.SetActive(true);
    } 

    public void SpawnEffect()
    {
        StartCoroutine(CO_SpawnCard());
    }

    private IEnumerator CO_SpawnCard()
    {
        yield return null;
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
        float duration = 0.05f;      // 뒤집히는 시간
        float timer = 0f;            // 

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
