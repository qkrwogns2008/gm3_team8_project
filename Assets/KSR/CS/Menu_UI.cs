using System.Collections.Generic;
using UnityEngine;

public class Menu_UI : MonoBehaviour
{
    [Header("내 메뉴 (온오프)")]
    public GameObject myPrefab;

    [Header("다른 메뉴 (끄기용)")]
    public List<GameObject> otherPrefabs = new List<GameObject>();

    [Header("센터 버튼 활성화")]
    public GameObject offTarget; // 꺼질 대상 (A)
    public GameObject onTarget;  // 켜질 대상 (B)

    [Header("영웅인포")]
    public List<GameObject> targetPrefabs = new List<GameObject>();

    public void OnClick()
    {
        if (myPrefab == null) return;

        // 🔹 현재 상태 확인
        bool isActive = myPrefab.activeSelf;

        if (!isActive)
        {
            // 1. 다른 메뉴 전부 끄기
            foreach (var obj in otherPrefabs)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            // 2. 내 메뉴 켜기
            myPrefab.SetActive(true);

            // 3. 센터 버튼 전환
            if (offTarget != null) offTarget.SetActive(false);
            if (onTarget != null) onTarget.SetActive(true);
        }
        else
        {
            // 1. 내 매뉴 끄기
            myPrefab.SetActive(false);

            // 2. 센터 버튼 전환 (반대로)
            if (offTarget != null) offTarget.SetActive(true);
            if (onTarget != null) onTarget.SetActive(false);
        }
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