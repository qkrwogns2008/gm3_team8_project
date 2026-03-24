using UnityEngine;
using TMPro;

public class PowerText : MonoBehaviour
{
    [Header("참조")]
    public Dummy_Player player;   // Dummy_Player 연결
    public TMP_Text powerText;    // 표시할 텍스트

    void Update()
    {
        if (player == null || powerText == null) return;

        float value = player.power;

        // k / m 단위 출력
        powerText.text =
            value >= 1000000f ? (value / 1000000f).ToString("F1") + "m" :
            value >= 1000f ? (value / 1000f).ToString("F1") + "k" :
                                value.ToString("F0");
    }
}