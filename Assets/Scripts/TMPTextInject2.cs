using TMPro;
using UnityEngine;

public class TMPTextInject2 : MonoBehaviour
{
	[SerializeField] private TMP_Text _msg;

	private void OnValidate()
	{
		if (_msg == null)
		{
			return;
		}

		_msg.text = "네 그게 좋겠군요!!";
	}
}