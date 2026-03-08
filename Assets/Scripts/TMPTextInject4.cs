using TMPro;
using UnityEngine;

public class TMPTextInject4 : MonoBehaviour
{
    [SerializeField] private TMP_Text _msg;

    private void OnValidate()
    {
        if (_msg == null)
        {
            return;
        }

        _msg.text = "푸쉬 한줄 알았는데 안되었네요...";
    }
}