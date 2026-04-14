using UnityEngine;
using UnityEngine.EventSystems;

public class Hero_Info : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform parentObject;   // 영웅정보 불러올 오브젝트
    [SerializeField] private GameObject prefabToSpawn; // 불러올 영웅정보
    [SerializeField] private AreaState areaState;      // 현재 영역 상태

    [SerializeField] private GameObject targetParent;      // 활성화할 오브젝트
    [SerializeField] private GameObject disableTarget;     // 비활성화할 오브젝트

    public void OnPointerClick(PointerEventData eventData)
    {
        // Area2 상태일 경우 동작 중단
        if (areaState != null && areaState.currentArea == AreaState.AreaType.Area2)
            return;

        // 필수 참조 체크
        if (parentObject == null || prefabToSpawn == null) return;

        // 프리팹 생성
        Instantiate(prefabToSpawn, parentObject);

        // 특정 오브젝트 활성화
        if (targetParent != null)
        {
            targetParent.SetActive(true);
        }

        // 특정 오브젝트 비활성화
        if (disableTarget != null)
        {
            disableTarget.SetActive(false);
        }
    }
}