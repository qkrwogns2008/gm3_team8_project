using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Button _activeButton;
	[SerializeField] private GameObject _arrow;
	[SerializeField] private GameObject _fxOutline;
	[SerializeField] private GameObject _fxArrow;
	#endregion

	#region 내부 변수
	private bool _isActive;
	#endregion

	private void OnEnable()
	{
		if (_activeButton != null)
		{
			_activeButton.onClick.AddListener(OnClickAutoPlayButton);
		}
	}

	private void OnDisable()
	{
		if (_activeButton != null)
		{
			_activeButton.onClick.RemoveListener(OnClickAutoPlayButton);
		}
	}

	public void OnClickAutoPlayButton()
	{
		Debug.Log($"{name} 버튼 클릭");
		ToggleAutoPlay();
	}

	private void ToggleAutoPlay()
	{
		_isActive = !_isActive;

		_arrow.SetActive(!_isActive);
		_fxOutline.SetActive(_isActive);
		_fxArrow.SetActive(_isActive);
	}
}