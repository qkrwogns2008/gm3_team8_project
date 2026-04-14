using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("이 버튼들을 누르면 해제")]
    public List<Button> closeButtons = new List<Button>();

    private Transform originalParent;

    private static Forming_Button currentSelected;

    private void Start()
    {
        // 리스트에 있는 버튼들에 클릭 이벤트 연결
        foreach (var btn in closeButtons)
        {
            if (btn != null)
                btn.onClick.AddListener(HandleExternalClose);
        }
    }

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

    // 외부 버튼 눌렸을 때 → 원상복구 + 버튼 비활성화
    public void HandleExternalClose()
    {
        if (currentSelected != null)
        {
            currentSelected.ResetVisual();
            currentSelected = null;
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