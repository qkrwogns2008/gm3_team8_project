using TMPro;
using UnityEngine;

public class TMPTextInject8 : MonoBehaviour
{
    [SerializeField] private TMP_Text _msg;

    private void OnValidate()
    {
        if (_msg == null)
        {
            return;
        }

        _msg.text = "뒤늦게 합니다. 일이 너무 바빠서.ㅠ.ㅠ";
    }
}