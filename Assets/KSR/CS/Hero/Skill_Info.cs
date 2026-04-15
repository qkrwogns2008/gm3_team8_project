using UnityEngine;

public class Skill_Info : MonoBehaviour
{
    [Header("켜질 오브젝트 1")]
    [SerializeField] private GameObject targetA;

    [Header("켜질 오브젝트 2")]
    [SerializeField] private GameObject targetB;

    private bool isOpen = false;

    // 버튼 OnClick에 연결
    public void OnClick()
    {
        // 버튼으로 열기
        if (targetA != null) targetA.SetActive(true);
        if (targetB != null) targetB.SetActive(true);

        isOpen = true;
    }

    void Update()
    {
        if (!isOpen) return;

        // 마우스 클릭 시 닫기
        if (Input.GetMouseButtonDown(0))
        {
            CloseTargets();
        }
    }

    void CloseTargets()
    {
        if (targetA != null) targetA.SetActive(false);
        if (targetB != null) targetB.SetActive(false);

        isOpen = false;
    }
}