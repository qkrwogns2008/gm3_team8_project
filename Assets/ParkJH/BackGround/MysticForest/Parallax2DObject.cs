using UnityEngine;

public class Parallax2DObject : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _cameraTr;
    [SerializeField] private bool _useX = true;
    [SerializeField] private bool _useY = true;
    #endregion

    #region 내부변수
    private ParallaxLayerElement[] _layers;
    private Vector3 _startCamPos;
    private Vector3[] _initialLocalPositions; // 자식들의 초기 위치 저장용
    #endregion

    private void Awake()
    {
        if (_cameraTr == null)
            _cameraTr = Camera.main.transform;

        // 자식들 가져오기
        _layers = GetComponentsInChildren<ParallaxLayerElement>();

        // 초기 위치 캐싱 (누적 오차 방지)
        _initialLocalPositions = new Vector3[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
        {
            _initialLocalPositions[i] = _layers[i].transform.localPosition;
        }
    }

    private void Start()
    {
        _startCamPos = _cameraTr.position;
    }

    private void LateUpdate()
    {
        Vector3 camPos = _cameraTr.position;

        for (int i = 0; i < _layers.Length; i++)
        {
            // 카메라와 start X, Y값 차이
            Vector3 totalDelta = camPos - _initialLocalPositions[i];
            ParallaxLayerElement layer = _layers[i];
            if (layer == null) continue;

            float f = layer.factor;
            Vector3 targetOffset = Vector3.zero;

            // X축 LayerElement 비례 parallax
            if (_useX && totalDelta.x > 0)
            {
                targetOffset.x = totalDelta.x * f;
            }

            // Y축: 지평선 효과 (카메라와의 거리에 반비례하여 속도 감소)
            if (_useY)
            {
                float dist = Mathf.Abs(camPos.y - _initialLocalPositions[i].y); // 카메라, Layer 간 거리
                float perspectiveFactor = - f * Mathf.Max(1f, dist * dist ); // 최소값 1로 고정하여 급발진 방지

                targetOffset.y =  perspectiveFactor;
            }

            // [핵심] += 가 아니라 초기 위치에서 Offset을 더하는 방식 (오차 없음)
            layer.transform.localPosition = _initialLocalPositions[i] + targetOffset;
        }
    }
}