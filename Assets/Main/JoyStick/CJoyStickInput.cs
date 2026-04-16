using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CJoyStickInput : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	#region 인스펙터
	[Header("Joystick_Visual")]
	[SerializeField] private RectTransform _visualContainer;
    [Header("조이스틱 배경")]
	[SerializeField] private RectTransform _bgRect;
	[Header("조이스틱 손잡이")]
	[SerializeField] private RectTransform _handleRect;
	[Header("조이스틱 투명도")]
	[SerializeField] private CanvasGroup _canvasGroup; 
	[Header("입력 값")]
	[SerializeField] private Vector2 _inputVector;

	[Header("설정")]
	[SerializeField] private float _longPressDelay = 0.15f;
	#endregion
		
	public Vector2 InputVector => _inputVector;

	private Coroutine _pressCheckRoutine;	// 시간 체크용 코루틴
	private bool _isJoystickActive = false;	// 조이스틱 활성화 상태인지 확인

    private void Awake()
    {
		// 시작할 때 조이스틱 숨기기
        if(_canvasGroup != null)
		{
			_canvasGroup.alpha = 0f;
		}
    }

	public void OnPointerDown(PointerEventData eventData)
	{
		if(_pressCheckRoutine != null)
		{
			StopCoroutine(_pressCheckRoutine);
		}

		_pressCheckRoutine = StartCoroutine(CheckLongPress(eventData));
		
	}

	private IEnumerator CheckLongPress(PointerEventData eventData)
	{
		Vector2 pressPosition = eventData.position;

		yield return new WaitForSeconds(_longPressDelay);

		// 설정한 시간이 지나면 조이스틱 활성화
		_isJoystickActive = true;

        // 조이스틱 세트를 터치한 화면 좌표로 이동
        _visualContainer.position = eventData.position;

        // 조이스틱 보여주기
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }

        // 핸들 위치 리셋
        _handleRect.anchoredPosition = Vector2.zero;

        // 누르자마자 드래그 계산
        OnDrag(eventData);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
		// 조이스틱 활성화중일때만 사용
		if(!_isJoystickActive)
		{
			return;
		}

		Vector2 pos;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_bgRect, eventData.position, eventData.pressEventCamera, out pos))
		{
			// 터치 위치
			float radius = _bgRect.sizeDelta.x / 2f;

			// 입력 벡터 계산
			_inputVector = pos / radius;
			_inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

			// 핸들 이미지 이동
			_handleRect.anchoredPosition = new Vector2(_inputVector.x * radius, _inputVector.y * radius);
		}
    }

	// 손에서 땔경우 조이스틱 숨기기
	public void OnPointerUp(PointerEventData eventData)
	{
		// 대기중이던 코루틴 중단
		if(_pressCheckRoutine != null)
		{
			StopCoroutine(_pressCheckRoutine);
			_pressCheckRoutine = null;
		}

		// 상태 초기화
		_isJoystickActive = false;
		_inputVector = Vector2.zero;
		_handleRect.anchoredPosition = Vector2.zero;

		// 조이스틱 숨기기
		if(_canvasGroup != null)
		{
			_canvasGroup.alpha = 0f;
		}
	}


}