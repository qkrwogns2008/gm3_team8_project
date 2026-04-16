using TMPro;
using UnityEngine;

public class BP : MonoBehaviour
{
    [Header("영웅 데이터")]
    [SerializeField] private HeroDataSO heroDataSO;

    [Header("출력 텍스트")]
    [SerializeField] private TextMeshProUGUI resultText1;
    [SerializeField] private TextMeshProUGUI resultText2;

    void Update()
    {
        // 데이터 없으면 실행 안함
        if (heroDataSO == null) return;

        // 최종 스탯 가져오기
        var finalStats = CDataManager.Instance.GetHeroFinalStatus(heroDataSO.HeroID, heroDataSO);

        // float → int 변환 (반올림)
        int atk = Mathf.RoundToInt(finalStats.HeroAtk);
        int hp = Mathf.RoundToInt(finalStats.HeroHP);

        // 공격력 * 5 + 체력
        int result = atk * 5 + hp;

        // K, M 포맷 적용
        string formatted = FormatNumber(result);

        // 두 텍스트에 동일 출력
        if (resultText1 != null)
            resultText1.text = formatted;

        if (resultText2 != null)
            resultText2.text = formatted;
    }

    // 숫자를 K, M 단위로 변환
    string FormatNumber(int value)
    {
        if (value >= 1000000)
        {
            float v = value / 1000000f;
            return Format4(v) + "M";
        }
        else if (value >= 1000)
        {
            float v = value / 1000f;
            return Format4(v) + "K";
        }
        else
        {
            return value.ToString();
        }
    }

    // 최대 4자리로 포맷
    string Format4(float v)
    {
        if (v >= 100)
            return v.ToString("F0");   // 123
        else if (v >= 10)
            return v.ToString("F1");   // 12.3
        else
            return v.ToString("F2");   // 1.23
    }
}