using UnityEngine;

public class Button_AddItem : MonoBehaviour
{
    public Inventory inventory;     // 인벤토리 참조
    public GameObject itemPrefab;   // 추가할 아이템 프리팹

    // 버튼 클릭 시 호출
    public void OnClickAddItem()
    {
        if (inventory == null || itemPrefab == null)
        {
            Debug.LogWarning("Inventory 또는 ItemPrefab이 연결되지 않았습니다.");
            return;
        }

        // 인벤토리에 아이템 추가
        inventory.AddItem(itemPrefab);
    }
}