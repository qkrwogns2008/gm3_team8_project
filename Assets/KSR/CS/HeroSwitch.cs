using UnityEngine;
using System.Collections.Generic;

public class HeroSwitch : MonoBehaviour
{
    [Header("선택 이미지")]
    public RectTransform selectImage;

    public GameObject hero_M;        // 끌 오브젝트
    public GameObject hero_F;        // 켤 오브젝트

    public Transform hero_area1;     // 이동할 대상
    public Transform hero_area2;     // 이동될 부모

    [Header("카드 매니저")]
    public CardManager cardManager;  // 소유 카드 표시용

    [Header("영웅인포")]
    public List<GameObject> targetPrefabs = new List<GameObject>();

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
            hero_area1.SetParent(hero_area2, false);

            AreaState areaState = hero_area1.GetComponentInChildren<AreaState>();
            if (areaState != null)
            {
                areaState.SetArea(AreaState.AreaType.Area2);
            }
        }

        // 선택 이미지 이동
        if (selectImage != null && myRect != null)
            selectImage.anchoredPosition = myRect.anchoredPosition;

        // 소유 카드만 표시
        if (cardManager != null)
            cardManager.ShowOwnedOnly();

        // 인포 파일 제거
        foreach (var obj in targetPrefabs)
        {
            if (obj == null) continue;

            // 안전 처리
            List<Transform> children = new List<Transform>();
            foreach (Transform child in obj.transform)
            {
                children.Add(child);
            }

            // 자식 제거
            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }
        }
    }
}