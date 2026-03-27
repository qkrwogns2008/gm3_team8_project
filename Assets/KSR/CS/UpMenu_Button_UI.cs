using System.Collections.Generic;
using UnityEngine;

public class UpMenu_Button_UI : MonoBehaviour
{
    [Header("선택 이미지")]
    public RectTransform selectImage;

    [Header("프리팹on")]
    public GameObject myPrefab;

    [Header("프리팹off")]
    public List<GameObject> offPrefabs = new List<GameObject>();

    private RectTransform myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        // 1. 다른 프리팹 전부 끄기
        foreach (var obj in offPrefabs)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // 2. 프리팹 켜기
        if (myPrefab != null)
            myPrefab.SetActive(true);

        // 3. 선택 이미지 이동
        if (selectImage != null && myRect != null)
            selectImage.anchoredPosition = myRect.anchoredPosition;
    }
}