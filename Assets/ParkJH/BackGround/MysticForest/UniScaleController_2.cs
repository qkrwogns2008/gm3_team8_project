using UnityEngine;

public class UniScaleController_2 : MonoBehaviour
{
    [SerializeField] private float _horizonStart = 4.5f;
    [SerializeField] private float _horizonFinish = 4.7f;
    [SerializeField] private float _scaleFactor = 0.05f;
    [SerializeField] private float _minScaleLimit = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _propBlock;
    private Transform _camTransform;
    private Vector3 _initialScale;
    private static readonly int _YOffsetID = Shader.PropertyToID("_VisualYOffset");

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camTransform = Camera.main.transform;
        _initialScale = transform.localScale;
        _propBlock = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (!_spriteRenderer || !_camTransform) return;

        // 1. 순수 거리 계산 (transform.position 절대 수정 금지)
        float relativeY = transform.position.y - _camTransform.position.y;

        // 2. 소멸 처리
        if (relativeY >= _horizonFinish)
        {
            _spriteRenderer.enabled = false;
            return;
        }
        _spriteRenderer.enabled = true;

        // 3. 스케일 처리
        float scaleTargetY = Mathf.Min(relativeY, _horizonStart);
        float reduction = Mathf.Min((scaleTargetY * scaleTargetY) * _scaleFactor, 1.0f - _minScaleLimit);
        transform.localScale = _initialScale * (1.0f - reduction);

        // 4. [핵심] 시각적 오프셋만 계산해서 셰이더로 전송
        float offset = 0f;
        if (relativeY > _horizonStart)
        {
            // 지평선을 넘어간 만큼 마이너스 값을 줘서 셰이더가 아래로 깎게 만듦
            offset = -(relativeY - _horizonStart);
        }

        _spriteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(_YOffsetID, offset);
        _spriteRenderer.SetPropertyBlock(_propBlock);
    }
}