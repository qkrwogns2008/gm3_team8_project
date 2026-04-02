using UnityEngine;

public class Parallax2DSky : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _cameraTr;       // 따라갈 카메라
    [SerializeField] private bool _useX = true;         // X축 parallax 여부
    [SerializeField] private bool _useY = false;        // Y축 parallax 여부
    #endregion

    #region 내부변수
    private ParallaxLayerElement[] _layers;
    // 이전 프레임 카메라 위치를 저장해 두었다가 -> 현재 프레임 위치

    private Vector3 _prevCampos;
    #endregion


    private void Awake()
    {
        if (_cameraTr == null)
        {
            if (Camera.main != null) _cameraTr = Camera.main.transform;
            else { enabled = false; return; }
        }

        _layers = GetComponentsInChildren<ParallaxLayerElement>();

        if (_layers.Length == 0)
        {
            Debug.LogWarning($"{name}: 자식 중에 ParallaxLayerElement가 없습니다!");
        }
    }

    private void Start()
    {
        // 첫 프레임에서 델타 값이 튀지 않도록 초기화
        _prevCampos = _cameraTr.position;

        foreach (var layer in _layers)
        {
            Debug.Log($"레이어 등록 완료: {layer.name} / 팩터: {layer.factor}");
        }
    }

    private void LateUpdate()
    {
        // 카메라 이동량
        Vector3 camPos = _cameraTr.position;
        Vector3 delta = camPos - _prevCampos;

        if (delta == Vector3.zero) return; // 카메라 미 이동시 계산 스킵

        if (_layers != null)
        {
            int layerCount = _layers.Length;
            for (int i = 0; i < layerCount; i++)
            {
                ParallaxLayerElement layer = _layers[i];
                if (layer == null)
                {
                    continue;
                }

                float f = layer.factor;
                Vector3 move = Vector3.zero;
                // 설정된 축에 따라 이동량 계산
                if (_useX)
                {
                    move.x = delta.x * f;
                }
                if (_useY)
                {
                    move.y = delta.y * f;
                }
                layer.transform.position += move;
            }

            _prevCampos = camPos;

        }

    }
}
