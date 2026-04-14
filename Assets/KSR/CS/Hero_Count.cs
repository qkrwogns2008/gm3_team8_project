using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hero_Count : MonoBehaviour
{
    public Image fillImage;          // Fill Amount 쓸 이미지
    public TMP_Text expText;         // "현재 / 최대" 텍스트

    void Update()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        if (expText == null || fillImage == null) return;

        string text = expText.text; // 예: "10 / 100"
        string[] parts = text.Split('/');

        if (parts.Length != 2) return;

        float current = float.Parse(parts[0]);
        float max = float.Parse(parts[1]);

        if (max <= 0) return;

        fillImage.fillAmount = current / max;
    }
}