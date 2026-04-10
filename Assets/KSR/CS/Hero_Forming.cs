using UnityEngine;

public class Hero_Forming : MonoBehaviour
{
    [SerializeField] private Transform parentB;        // 배치칸
    [SerializeField] private GameObject prefabA;       // 배치할 영웅
    [SerializeField] private GameObject targetObject;  // 편성 완료 표시
    [SerializeField] private GameObject warningPrefab; // 경고창

    private GameObject spawnedObject; // 영웅 배치

    // 배치 상태 (내부 상태 관리용)
    private bool isPlaced = false;

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void Spawn()
    {
        if (parentB == null || prefabA == null) return;

        // 이미 생성된 경우 → 제거
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;

            isPlaced = false; // 상태 OFF

            if (targetObject != null)
                targetObject.SetActive(false);

            return;
        }

        // 현재 총 배치 수 체크
        int currentCount = 0;
        for (int i = 0; i < parentB.childCount; i++)
        {
            if (parentB.GetChild(i).childCount > 0)
                currentCount++;
        }

        // 최대 5개 제한
        if (currentCount >= 5)
        {
            if (warningPrefab != null)
                Instantiate(warningPrefab, parentB);
            return;
        }

        // 빈 슬롯 찾기
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            if (slot.childCount == 0)
            {
                spawnedObject = Instantiate(prefabA, slot);

                isPlaced = true; // 상태 ON

                if (targetObject != null)
                    targetObject.SetActive(true);

                break;
            }
        }
    }
}