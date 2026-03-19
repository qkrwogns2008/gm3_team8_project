using TMPro;
using UnityEngine;

public class TMPTextInject3 : MonoBehaviour
{
	[SerializeField] private TMP_Text _msg;

	private void OnValidate()
	{
		if (_msg == null)
		{
			return;
		}

		_msg.text = "다른 분들은 잘 되세요?";
	}
}