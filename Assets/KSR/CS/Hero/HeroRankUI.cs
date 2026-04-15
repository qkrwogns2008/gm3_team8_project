using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class HeroRankUI : MonoBehaviour
{
    [Header("보유 카드 텍스트 (숫자 또는 n/형식)")]
    [SerializeField] private TextMeshProUGUI ownedText;

    [Header("출력 텍스트")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI progressText;

    void Update()
    {
        if (ownedText == null) return;

        int ownedCount = ParseNumber(ownedText.text);

        UpdateRankUI(ownedCount);
    }

    // 문자열에서 숫자만 추출
    int ParseNumber(string text)
    {
        string number = Regex.Match(text, @"\d+").Value;

        int result = 0;
        int.TryParse(number, out result);

        return result;
    }

    void UpdateRankUI(int ownedCount)
    {
        int rank = 0;
        int current = ownedCount;

        int[] need = { 3, 4, 5, 6, 7 };

        // 단계별 차감
        for (int i = 0; i < need.Length; i++)
        {
            if (current >= need[i])
            {
                current -= need[i];
                rank++;
            }
            else break;
        }

        int required = rank < need.Length ? need[rank] : 0;

        if (rankText != null)
            rankText.text = rank.ToString();

        if (progressText != null)
            progressText.text = required > 0 ? current + "/" + required : "";
    }
}