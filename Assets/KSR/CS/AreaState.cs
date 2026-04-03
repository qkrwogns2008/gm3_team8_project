using UnityEngine;

public class AreaState : MonoBehaviour
{
    public enum AreaType
    {
        Area1,
        Area2
    }

    [Header("현재 위치 상태")]
    public AreaType currentArea;

    // 외부에서 상태 변경할 때 사용
    public void SetArea(AreaType newArea)
    {
        currentArea = newArea;
    }

    // 확인용 (선택)
    public bool IsArea1()
    {
        return currentArea == AreaType.Area1;
    }

    public bool IsArea2()
    {
        return currentArea == AreaType.Area2;
    }
}