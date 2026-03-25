using UnityEngine;
using TMPro;

public class Dummy_Gold : MonoBehaviour
{
    public Dummy_Player DummyData; // 더미 데이터 참조
    private TMP_Text text;

    void Awake() // TMP 컴포넌트 가져오기
    {
        text = GetComponent<TMP_Text>();
    }

    void Update() // 매 프레임 골드 값 반영
    {
        if (DummyData != null && text != null)
        {
            float gold = DummyData.gold;

            if (gold >= 1000f)
            {
                float value = gold / 1000f;

                if (value < 10f)
                    text.text = value.ToString("F2") + "k"; // 1.00k
                else if (value < 100f)
                    text.text = value.ToString("F1") + "k"; // 12.3k
                else
                    text.text = value.ToString("F0") + "k"; // 999k
            }
            else
            {
                text.text = gold.ToString("F0"); // 정수
            }
        }
    }
}