using TMPro;
using UnityEngine;

public class SimpleCalcUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textA;
    [SerializeField] private TextMeshProUGUI textB;

    [Header("출력 텍스트")]
    [SerializeField] private TextMeshProUGUI resultText1;
    [SerializeField] private TextMeshProUGUI resultText2;

    void Update()
    {
        if (textA == null || textB == null) return;

        int a = 0;
        int b = 0;

        int.TryParse(textA.text, out a);
        int.TryParse(textB.text, out b);

        int result = a * 5 + b;

        string formatted = FormatNumber(result);

        // 두 개의 출력칸에 동일하게 출력
        if (resultText1 != null)
            resultText1.text = formatted;

        if (resultText2 != null)
            resultText2.text = formatted;
    }

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

    string Format4(float v)
    {
        if (v >= 100)
            return v.ToString("F0");
        else if (v >= 10)
            return v.ToString("F1");
        else
            return v.ToString("F2");
    }
}