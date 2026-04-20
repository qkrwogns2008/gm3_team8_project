using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Center_Button : MonoBehaviour
{
    [Header("프리펩 off")]
    public List<GameObject> targetPrefabs = new List<GameObject>();

    [Header("센터버튼 a")]
    public GameObject onTarget;

    [Header("센터버튼 b")]
    public GameObject offTarget;

    public void OnClick()
    {
        // 1. 프리팹 전부 끄기
        foreach (var obj in targetPrefabs)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // 2. A 켜기
        if (onTarget != null)
            onTarget.SetActive(true);

        // 3. B 끄기
        if (offTarget != null)
            offTarget.SetActive(false);
    }
}