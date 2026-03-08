using TMPro;
using UnityEngine;

public class TMPTextInject7 : MonoBehaviour
{
    [SerializeField] private TMP_Text _msg;

    private void OnValidate()
    {
        if (_msg == null)
        {
            return;
        }

        _msg.text = "다들 화이팅입니다..!";
    }
}