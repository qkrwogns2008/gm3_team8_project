using TMPro;
using UnityEngine;

public class TMPTextInject1 : MonoBehaviour
{
	[SerializeField] private TMP_Text _msg;

	private void OnValidate()
	{
		if (_msg == null)
		{
			return;
		}

		_msg.text = "이렇게 적어볼까요?";
	}
}