using UnityEngine;
using UnityEngine.EventSystems;

public class HeroDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 드래그 시작 전 부모 슬롯 저장
    private Transform originalParent;

    // 최상위 캔버스
    private Canvas rootCanvas;

    // Raycast 제어용
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // 최상위 Canvas 찾기
        rootCanvas = GetComponentInParent<Canvas>();

        // CanvasGroup 가져오기 (없으면 추가)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 현재 부모 슬롯 저장
        originalParent = transform.parent;

        // 드래그 대상 지정
        eventData.pointerDrag = gameObject;

        // 드롭 이벤트가 슬롯까지 전달되도록 Raycast 비활성화
        canvasGroup.blocksRaycasts = false;

        // 최상위로 이동
        transform.SetParent(rootCanvas.transform);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 위치 따라 이동
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

        // 현재 필드 상태를 기준으로 JSON 갱신
        UpdateJSON();
    }

    // 현재 필드 상태를 기준으로 JSON 데이터 초기화 후 재기록
    private void UpdateJSON()
    {
        if (CDataManager.Instance == null) return;

        Transform fieldParent = originalParent.parent;

        // 전체 초기화
        for (int i = 0; i < 16; i++)
        {
            int x = i % 4;
            int y = i / 4;
            CDataManager.Instance.AddUserHeroArray(x, y, (EHeroID)0);
        }

        // 슬롯 기준으로 재기록
        for (int i = 0; i < fieldParent.childCount; i++)
        {
            Transform slot = fieldParent.GetChild(i);

            if (slot.childCount == 0) continue;

            Transform child = slot.GetChild(0);

            Hero_Connect1 connect = child.GetComponent<Hero_Connect1>();
            if (connect == null || connect.heroDataSO == null) continue;

            int x = i % 4;
            int y = i / 4;

            CDataManager.Instance.AddUserHeroArray(x, y, connect.heroDataSO.HeroID);
        }
    }

    // 원래 부모 반환
    public Transform GetOriginalParent()
    {
        return originalParent;
    }
}