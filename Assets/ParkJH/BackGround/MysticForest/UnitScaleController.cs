using UnityEngine;

public class UnitScaleController : MonoBehaviour
{
    [Header("Scale 변화 수치")]
    [SerializeField] private float _minScaleLimit = 0.2f;
    [Header("Shader 설정값")]
    [SerializeField] private float _scaleFactor = 0.01f;
    [SerializeField] private float _horizonStart = 10;//3.83+0.56
    [SerializeField] private float _horizonFinish = 11f;

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

        // 사라짐 처리 (StayPoint 기준)
        if (relativeY >= _horizonFinish && _renderer.enabled == true)
        {
            _renderer.enabled = false;
        }
        else if (relativeY < _horizonFinish && relativeY >= _horizonStart && _renderer.enabled == false)
        {
            _renderer.enabled = true;
        }
        // 스케일 계산 (_horizonFinish 이하 기준)
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

}