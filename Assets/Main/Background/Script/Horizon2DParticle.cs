using System.Collections.Generic;
using UnityEngine;

public class Horizon2DParticle : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _cameraTr;
    [SerializeField] private bool _useX = true;
    [SerializeField] private bool _useY = true;
    #endregion

    #region 내부변수
    private ParallaxLayerElement[] _layers;
    private Vector3 _startCamPos;
    Vector3 camPos;
    Vector3 ParentPos;
    // private Vector3[] _initialLocalPositions; // 자식들의 초기 위치 저장용
    #endregion

    private void Awake()
    {
        if (_cameraTr == null)
            _cameraTr = Camera.main.transform;

        // 자식들 가져오기
        _layers = GetComponentsInChildren<ParallaxLayerElement>(true);

        // 초기 위치 캐싱 (누적 오차 방지)
        // _initialLocalPositions = new Vector3[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
        {
            //    _initialLocalPositions[i] = _layers[i].transform.localPosition;
        }
    }

    private void OnEnable()
    {
        //_startCamPos = _cameraTr.position;
        camPos = _cameraTr.position;
        ParentPos = transform.position;
    }

    private void Update()
    {
        GameState gameState = CGameManager.Instance.CurrentState;
        if (gameState != GameState.MainStage)
        {
            return;
        }
        camPos = _cameraTr.position;
        ParentPos = transform.position;

        for (int i = 0; i < _layers.Length; i++)
        {
            // 카메라와 start X, Y값 차이
            ParallaxLayerElement layer = _layers[i];
            Vector3 totalDelta = camPos - ParentPos;

            if (layer == null) continue;

            // float factorX = layer.factorX;
            //float factorY = layer.factorY;
            Vector3 ChildrenPos = Vector3.zero;

            float dist = totalDelta.y - 100; // 카메라, Layer 간 거리
            if (dist > 0)
            {
                dist = 0; // 카메라가 레이어보다 아래로 내려가는 경우 보정
            }
            // X축 LayerElement 비례 parallax
            if (_useX)
            {
                //ChildrenPos.x = -layer.factorX * dist * totalDelta.x;
            }

            // Y축: 지평선 효과 (카메라와의 거리에 반비례하여 속도 감소)
            if (_useY)
            {
                if (dist < -220) // 일정 거리 이상 멀어지면 지평선 효과 적용
                {
                    dist = -555;
                }
                ChildrenPos.z = layer.factorY * dist * dist - 1f;
            }
            // 초기 위치에서 ChildrenPos 더해줌
            layer.transform.position = ParentPos + ChildrenPos;
        }
    }
}