using TMPro;
using UnityEngine;

public class TMPTextInject5 : MonoBehaviour
{
    [SerializeField] private TMP_Text _msg;

    private void OnValidate()
    {
        if (_msg == null)
        {
            return;
        }

        _msg.text = "¥ æÓ¡Æº≠ ¡Àº€«’¥œ¥Ÿ...";
    }
}