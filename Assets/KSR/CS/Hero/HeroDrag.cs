using UnityEngine;
using UnityEngine.EventSystems;

public class HeroDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;   // 원래 슬롯
    private Canvas rootCanvas;          // 최상위 캔버스
    private CanvasGroup canvasGroup;    // Raycast 제어용

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // 드래그 대상 강제 지정 (핵심)
        eventData.pointerDrag = gameObject;

        // 드롭이 슬롯까지 전달되도록 Raycast 차단 해제
        canvasGroup.blocksRaycasts = false;

        // 최상단으로 이동
        transform.SetParent(rootCanvas.transform);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast 복구
        canvasGroup.blocksRaycasts = true;

        // 드롭 실패 시 원래 자리로 복귀
        if (transform.parent == rootCanvas.transform)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }

    // 원래 부모 반환
    public Transform GetOriginalParent()
    {
        return originalParent;
    }
}