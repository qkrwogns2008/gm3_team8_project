using TMPro;
using UnityEngine;

public class TMPTextInject6 : MonoBehaviour
{
    [SerializeField] private TMP_Text _msg;

    private void OnValidate()
    {
        if (_msg == null)
        {
            return;
        }

        _msg.text = "프로젝트 Log 및 vs공유 안되게 수정했습니다!";
    }
}