using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CJoyStickInput : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	#region 인스펙터
	[Header("조이스틱 배경")]
	[SerializeField] private RectTransform _bgRect;
	[Header("조이스틱 손잡이")]
	[SerializeField] private RectTransform _handleRect;
	[Header("입력 값")]
	[SerializeField] private Vector2 _inputVector;
	#endregion

	public Vector2 InputVector => _inputVector;

	// 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
		Vector2 pos;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_bgRect, eventData.position, eventData.pressEventCamera, out pos))
		{
			pos.x = (pos.x / _bgRect.sizeDelta.x);
			pos.y = (pos.y / _bgRect.sizeDelta.y);

			_inputVector = new Vector2(pos.x * 2, pos.y * 2);
			_inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

			_handleRect.anchoredPosition = new Vector2(_inputVector.x * (_bgRect.sizeDelta.x / 2), _inputVector.y * (_bgRect.sizeDelta.y / 2));
		}
    }

	public void OnPointerUp(PointerEventData eventData)
	{
		_inputVector = Vector2.zero;
		_handleRect.anchoredPosition = Vector2.zero;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

}