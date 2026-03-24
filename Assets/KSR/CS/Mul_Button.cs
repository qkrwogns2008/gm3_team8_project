using System.Collections.Generic;
using UnityEngine;

public class Mul_Button : MonoBehaviour
{
    public List<LV_UP_Button> targets = new List<LV_UP_Button>(); // 레벨업 버튼들
    public int stepValue = 1;                                     // 배수 (1, 10, 100)

    public RectTransform selectImage; // 선택 표시 이미지
    private RectTransform myRect;

    void Awake() // 자신의 RectTransform 가져오기
    {
        myRect = GetComponent<RectTransform>();
    }

    public void OnClick() // 버튼 클릭 시 실행
    {
        // 모든 레벨업 버튼에 배수 적용 + UI 갱신
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.levelStep = stepValue;
                target.UpdateUI(); // ★ 여기 추가된 핵심
            }
        }

        // 선택 이미지 이동
        if (selectImage != null && myRect != null)
            selectImage.anchoredPosition = myRect.anchoredPosition;
    }
}