using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleSpawnOnImage : MonoBehaviour, IPointerClickHandler
{
    [Header("프리팹")]
    public GameObject spawnPrefab; // 생성할 프리팹

    [Header("부모 (빈 오브젝트)")]
    public Transform parent; // 생성 위치

    [Header("활성/비활성 대상")]
    public GameObject targetObject; // 같이 켜질 오브젝트

    private GameObject spawnedObject; // 생성된 프리팹 저장
    private bool isActive = false; // 현재 상태

    // =============================
    // 이미지 클릭 감지

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive)
        {
            Activate(); // 생성 + 활성화
        }
        else
        {
            Deactivate(); // 제거 + 비활성화
        }
    }

    // =============================
    // 활성화 (프리팹 생성 + 오브젝트 ON)

    void Activate()
    {
        if (spawnPrefab != null && parent != null)
        {
            spawnedObject = Instantiate(spawnPrefab, parent); // 프리팹 생성
        }

        if (targetObject != null)
        {
            targetObject.SetActive(true); // 오브젝트 활성화
        }

        isActive = true; // 상태 변경
    }

    // =============================
    // 비활성화 (프리팹 제거 + 오브젝트 OFF)

    void Deactivate()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); // 프리팹 제거
        }

        if (targetObject != null)
        {
            targetObject.SetActive(false); // 오브젝트 비활성화
        }

        isActive = false; // 상태 변경
    }
}