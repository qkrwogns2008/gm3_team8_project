using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null) return;

        HeroDrag hero = dropped.GetComponent<HeroDrag>();
        if (hero == null) return;

        Transform fromSlot = hero.GetOriginalParent();
        Transform toSlot = transform;

        // 같은 슬롯이면 무시
        if (fromSlot == toSlot) return;

        // 현재 슬롯에 다른 영웅이 있는 경우 → 스왑
        if (toSlot.childCount > 0)
        {
            Transform other = toSlot.GetChild(0);
            other.SetParent(fromSlot);
            other.localPosition = Vector3.zero;
        }

        // 현재 드래그 중인 영웅 이동
        dropped.transform.SetParent(toSlot);
        dropped.transform.localPosition = Vector3.zero;

        
    }
}