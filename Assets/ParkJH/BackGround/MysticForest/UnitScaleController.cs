using UnityEngine;

public class UnitScaleController : MonoBehaviour
{
    [Header("설정값")]
    public float scaleFactor = 0.1f;    // 비례 계수 (0.1)
    public float minScaleLimit = 0.4f; // 최대 30% 감소 (원본의 70% 유지)

    private Transform camTransform;
    private Vector3 initialScale;

    void Start()
    {
        // 메인 카메라 트랜스폼 참조
        if (Camera.main != null) camTransform = Camera.main.transform;

        // 시작할 때의 스케일 저장 (원본 크기 기준)
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (camTransform == null) return;

        // 1. 카메라로부터의 상대적인 Y 거리 계산 (카메라 기준 Y축 증가량)
        float relativeY = transform.position.y - camTransform.position.y-2f;

        // Y값이 음수일 경우(카메라보다 아래)를 대비해 0 이상으로 클램핑
        relativeY = Mathf.Max(0, relativeY);

        // 2. 팀장님의 마법 공식: Y^2 * 0.1
        // 감소량 = (Y * Y) * 0.1
        float reduction = (relativeY * relativeY) * scaleFactor;

        // 3. 감소량이 최대 30%(0.3)를 넘지 않도록 제한
        reduction = Mathf.Min(reduction, 1.0f - minScaleLimit);

        // 4. 최종 스케일 계산 (1.0 - 감소량)
        float finalScaleFactor = 1.0f - reduction;

        // 5. 원본 스케일에 곱해서 적용
        transform.localScale = initialScale * finalScaleFactor;
    }
}