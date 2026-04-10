using UnityEngine;
using UnityEngine.EventSystems;

public class Hero_Info : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform parentObject;   // 영웅정보 불러올 오브젝트
    [SerializeField] private GameObject prefabToSpawn; // 불러올 영웅정보
    [SerializeField] private AreaState areaState;      // 현재 영역 상태

    public void OnPointerClick(PointerEventData eventData)
    {
        if (areaState != null && areaState.currentArea == AreaState.AreaType.Area2)
            return;

        if (parentObject == null || prefabToSpawn == null) return;

        Instantiate(prefabToSpawn, parentObject);
    }
}