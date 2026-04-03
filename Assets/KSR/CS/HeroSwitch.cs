using UnityEngine;

public class HeroSwitch : MonoBehaviour
{
    [Header("선택 이미지")]
    public RectTransform selectImage;

    public GameObject hero_M;        // 끌 오브젝트
    public GameObject hero_F;        // 켤 오브젝트

    public Transform hero_area1;     // 이동할 대상
    public Transform hero_area2;     // 이동될 부모

    private RectTransform myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    // 버튼 클릭 시 실행
    public void OnClickSwitch()
    {
        // Hero_M 끄기
        if (hero_M != null)
            hero_M.SetActive(false);

        // Hero_F 켜기
        if (hero_F != null)
            hero_F.SetActive(true);

        // hero_area1을 hero_area2 아래로 이동
        if (hero_area1 != null && hero_area2 != null)
        {
            hero_area1.SetParent(hero_area2, false); // 위치 초기화 포함

            AreaState areaState = hero_area1.GetComponentInChildren<AreaState>(); // 영웅 프리펩 상태 변경
            if (areaState != null)
            {
                areaState.SetArea(AreaState.AreaType.Area2);
            }
        }


        // 선택 이미지 이동
        if (selectImage != null && myRect != null)
            selectImage.anchoredPosition = myRect.anchoredPosition;
    }
}