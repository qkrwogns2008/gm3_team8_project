using UnityEngine;

public class FormingClearButton : MonoBehaviour
{
    [SerializeField] private Transform parentB; // 전체 슬롯 부모 (0~15 슬롯)

    // 버튼 클릭 시 호출
    public void ClearAll()
    {
        if (parentB == null || CDataManager.Instance == null) return;

        // 필드의 모든 슬롯 비우기
        for (int i = 0; i < parentB.childCount; i++)
        {
            Transform slot = parentB.GetChild(i);

            for (int j = slot.childCount - 1; j >= 0; j--)
            {
                Destroy(slot.GetChild(j).gameObject);
            }
        }

        // JSON 전체 초기화
        for (int i = 0; i < 16; i++)
        {
            int x = i % 4;
            int y = i / 4;
            CDataManager.Instance.AddUserHeroArray(x, y, (EHeroID)0);
        }

        // UI 갱신
        if (FormingChecker.Instance != null)
            FormingChecker.Instance.CheckForming();
    }
}