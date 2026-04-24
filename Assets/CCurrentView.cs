using TMPro;
using UnityEngine;


public class CCurrentView : MonoBehaviour
{

    [SerializeField] private TMP_Text currenctRubyText;
    [SerializeField] private TMP_Text currenctTicketText;

    private void OnEnable()
    {
        RefreshUI();

        CQuestManager.Instance.OnDataUpdate += RefreshUI;
    }

    private void OnDisable()
    {
        if (CQuestManager.Instance != null)
        {
            CQuestManager.Instance.OnDataUpdate -= RefreshUI;
        }
    }

    private void RefreshUI()
    {
        currenctRubyText.text = CDataManager.Instance.UserData.Ruby.ToString();
        currenctTicketText.text = CDataManager.Instance.UserData.PickUpTicket.ToString();
    }
}
