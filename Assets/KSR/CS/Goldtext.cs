using UnityEngine;
using TMPro;

public class Goldtext : MonoBehaviour
{
    public TMP_Text GoldText; // 루비 텍스트

    void Update()
    {
        if (CDataManager.Instance == null) return; // 데이터매니저 체크

        int gold = CDataManager.Instance.UserData.Gold; // 골드 값 가져오기

        GoldText.text = gold.ToString(); // 텍스트 표시
    }
}