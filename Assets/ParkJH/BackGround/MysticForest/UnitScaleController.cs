using UnityEngine;

public class UnitScaleController : MonoBehaviour
{
    [Header("Scale 변화 수치")]
    [SerializeField] private float _minScaleLimit = 0.2f;
    [Header("Shader 설정값")]
    [SerializeField] private float _scaleFactor = 0.05f;
    [SerializeField] private float _horizonStart = 4.5f;//3.83+0.56
    [SerializeField] private float _horizonFinish = 4.7f;

    private Transform _camTransform;
    private Vector3 _initialScale;
    private Vector3 _originalPosition;
    private Renderer _renderer;
    private bool _isPositionAltered = false;
    
    void Start()
    {
        if (Camera.main != null) _camTransform = Camera.main.transform;
        _initialScale = transform.localScale;
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (_camTransform == null || _renderer == null) return;
        float relativeY = transform.position.y - _camTransform.position.y;

        relativeY = Mathf.Max(0, relativeY);

        // 1. 사라짐 처리 (StayPoint 기준)
        if (relativeY >= _horizonStart)
        {
            _renderer.enabled = false;
        }
        // 2. 스케일 계산 (_horizonFinish 이하 기준)
        else if (relativeY < _horizonStart)
        {
            _renderer.enabled = true;
            // 지평선 시작점(_horizonStart)까지만 줄어들도록 제한
            float scaleTargetY = Mathf.Min(relativeY, _horizonStart);
            float reduction = (scaleTargetY * scaleTargetY) * _scaleFactor;
            reduction = Mathf.Min(reduction, 1.0f - _minScaleLimit);

            transform.localScale = _initialScale * (1.0f - reduction);

        }

    }

    /* 지평선 대기 효과 폐기건
    // 3. 눈속임 위치만 렌더링 직전에 교체
    void OnWillRenderObject()
    {
        if (_camTransform == null || !_renderer.enabled) return;

        float currentRelativeY = transform.position.y - _camTransform.position.y;
        // [핵심] Finish 지점을 넘었다면 위치 고정 로직을 타지 않도록 즉시 리턴!
        // 이렇게 해야 지평선에 유닛이 맺힌 채로 카메라에 끌려오는 걸 막습니다.
        if (currentRelativeY >= _horizonFinish)
        {
            return;
        }
        // [중요] 현재 실제 위치를 백업
        _originalPosition = transform.position;

        // Update와 동일한 relativeY 계산 (기준점 통일)
        float relativeY = _originalPosition.y - (_camTransform.position.y);

        

        // 지평선 시작점 ~ 끝점 사이라면 "시각적 위치만" 고정
        if (relativeY >= _horizonStart && relativeY < _horizonFinish)
        {
            // 지평선에 해당하는 실제 월드 Y 좌표 계산
            float visualWorldY = _camTransform.position.y + _horizonStart;

            // 그리기 직전 위치 바꿔치기
            transform.position = new Vector3(_originalPosition.x, visualWorldY, _originalPosition.z);
            _isPositionAltered = true;
        }
    }
    */

}