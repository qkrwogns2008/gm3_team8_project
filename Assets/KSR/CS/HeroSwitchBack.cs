using UnityEngine;

public class HeroSwitchBack : MonoBehaviour
{
    [Header("선택 이미지")]
    public RectTransform selectImage;

    public GameObject hero_M;        // 다시 켤 오브젝트
    public GameObject hero_F;        // 끌 오브젝트

    public Transform hero_area1;     // 이동할 대상
    public Transform originalParent; // 원래 부모

    private RectTransform myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    // 버튼 클릭 시 실행
    public void OnClickSwitchBack()
    {
        // Hero_M 켜기
        if (hero_M != null)
            hero_M.SetActive(true);

        // Hero_F 끄기
        if (hero_F != null)
            hero_F.SetActive(false);

        // hero_area1을 원래 부모로 복구
        if (hero_area1 != null && originalParent != null)
        {
            hero_area1.SetParent(originalParent, false);

            AreaState areaState = hero_area1.GetComponentInChildren<AreaState>(); // 영웅 프리펩 상태 변경
            if (areaState != null)
            {
                areaState.SetArea(AreaState.AreaType.Area1);
            }
        }

        // 선택 이미지 이동
        if (selectImage != null && myRect != null)
            selectImage.anchoredPosition = myRect.anchoredPosition;
    }
}