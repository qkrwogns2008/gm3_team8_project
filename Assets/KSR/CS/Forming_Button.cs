using UnityEngine;
using UnityEngine.EventSystems;

public class Forming_Button : MonoBehaviour, IPointerClickHandler
{
    [Header("편성 버튼")]
    public GameObject buttonB;

    [Header("에리어 상태")]
    public AreaState areaState;

    [Header("이동시킬 대상 (카드 내부)")]
    public Transform topVisual;

    [Header("위치 기준 (선택)")]
    public Transform targetPoint;

    private Transform originalParent;

    private static Forming_Button currentSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 에리어2가 아니면 동작 안함
        if (areaState == null || !areaState.IsArea2())
            return;

        // 같은 카드 다시 클릭 시 해제 (토글)
        if (currentSelected == this)
        {
            ResetVisual();
            currentSelected = null;
            return;
        }

        // 이전 선택 복구
        if (currentSelected != null)
        {
            currentSelected.ResetVisual();
        }

        currentSelected = this;

        // 버튼 활성화
        if (buttonB != null)
            buttonB.SetActive(true);

        // topVisual 이동
        if (topVisual != null)
        {
            originalParent = topVisual.parent;
            topVisual.SetParent(buttonB.transform, false);
        }
    }

    // 원상 복구 + 버튼 비활성화
    public void ResetVisual()
    {
        if (topVisual != null && originalParent != null)
        {
            topVisual.SetParent(originalParent, false);
        }

        if (buttonB != null)
        {
            buttonB.SetActive(false);
        }
    }

    // 외부 클릭 시 전체 해제
    public static void ResetCurrent()
    {
        if (currentSelected != null)
        {
            currentSelected.ResetVisual();
            currentSelected = null;
        }
    }
}