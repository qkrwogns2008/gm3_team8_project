using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelectUI : MonoBehaviour, IPointerClickHandler
{
    public GameObject image1;
    public GameObject image2;

    private AreaState areaState;

    void Awake()
    {
        areaState = GetComponentInChildren<AreaState>();
    }

    // 오브젝트 클릭(좌클릭)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (areaState == null) return;

        if (areaState.currentArea == AreaState.AreaType.Area1)
        {
            image1.SetActive(true);
            image2.SetActive(false);
        }
        else
        {
            image1.SetActive(false);
            image2.SetActive(true);
        }
    }

    // 외부 클릭 감지용(static)
    public static void DeselectAll(GameObject img1, GameObject img2)
    {
        if (img1 != null) img1.SetActive(false);
        if (img2 != null) img2.SetActive(false);
    }
}