using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CRewardItemSlot : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    public Image Icon;              // 아이콘 이미지
    public Image Background;        // 배경 이미지
    public Image Outline;           // 테두리 이미지
    public TMP_Text AmountText;     // 개수 텍스트

    public void SetItem(Sprite sprite, int count, Sprite background, Sprite outLine)
    {
        if (Icon)
        {
            Icon.sprite = sprite;
        }

        if (AmountText)
        {
            AmountText.text = count.ToString();
        }

        //if (Background)
        //{
        //    Background.sprite = background;
        //}

        //if(Outline)
        //{
        //    Outline.sprite = outLine;
        //}
    }
}
