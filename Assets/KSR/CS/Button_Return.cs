using System.Collections.Generic;
using UnityEngine;

public class Button_Return : MonoBehaviour
{
    [Header("영웅인포")]
    public List<GameObject> targetPrefabs = new List<GameObject>();

    [Header("센터버튼 a")]
    public GameObject onTarget;

    [Header("센터버튼 b")]
    public GameObject offTarget;

    public void OnClick()
    {
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

        // 2. A 켜기
        if (onTarget != null)
            onTarget.SetActive(true);

        // 3. B 끄기
        if (offTarget != null)
            offTarget.SetActive(false);
    }
}