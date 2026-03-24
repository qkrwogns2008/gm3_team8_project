using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CAutoEnemyMove : MonoBehaviour
{
    #region 인스펙터
    [Header("Move")]
    [SerializeField] private float _walkspeed = 2f;             // 이동속도 (임시) 이후 외부에서 받아올것
    [SerializeField] private float _attackRange = 4f;           // 사거리 (임시)
    [SerializeField] private float _walkRange = 10f;    // 주변 돌아다니는 범위
    [SerializeField] private float _walkTimer = 3f;     // 대기시간
    [Header("Tracking")]
    [SerializeField] private float _detectionRange = 10f;    // 탐지 범위
    [SerializeField] private LayerMask _playerLayer;        // 탐지할 레이어
    [Header("SpineAnim")]
    [SerializeField] private string _idleAnim = "Idle";
    [SerializeField] private string _moveAnim = "Move";
    // 타겟 발견시 속도는 기본과 같음
    #endregion
    #region 내부변수
    private Vector3 _homePosition;                       // 처음 스폰한 위치
    private Vector3 _targetPos;                          // 타겟 위치
    private bool _isMoving = false;                      // 이동 상태 확인
    private float _timer = 0f;                           // 대기시간 타이머

    public bool _canAttack = false;         // 공격 스크립트에서 참조
    private Transform _targetPlayer;        // 발견 플레이어

    private SkeletonAnimation _skeletonAnim;
    #endregion

    void Start()
    {
        // 현재 위치 세팅
        _homePosition = transform.position;

        _skeletonAnim = GetComponent<SkeletonAnimation>();
        // 이동할 위치 선택
        SetNewTarget();
    }
    void Update()
    {
        // 플레이어 레이어 탐색
        Collider[] targets = Physics.OverlapSphere(transform.position, _detectionRange, _playerLayer);

        //탐지된 플레이어가 있을경우
        if(targets.Length > 0)
        {
            // 가장 가까운 플레이어 레이어 탐지
            Transform closestTarget = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target.transform;
                }
               
            }
            // 가장가까운 플레이어 탐지후
            if (closestTarget != null)
            {
                _targetPos = closestTarget.position;
                // 사거리 안
                if (minDistance < _attackRange)
                {
                    // 이동 정지
                    _isMoving = false;
                    // 공격 가능 상태
                    _canAttack = true;
                    // 멈춘상태여도 플레이어 바라보도록
                    LookAtTarget();
                }
                // 사거리 밖
                else
                {
                    // 이동
                    _isMoving = true;
                    // 공격 불가능
                    _canAttack = false;
                    MoveToTarget(true);
                }
            }
        }
       
        // 플레이어 감지 못함
        else
        {
            if (_isMoving)
            {
                MoveToTarget(false);
            }
            else
            {
                WaitAtPosiiton();
            }
        }
    }
    
    // 이동 위치 탐색
    void MoveToTarget(bool isTracking)
    {
        LookAtTarget();
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkspeed * Time.deltaTime);
        
        // 목표지점.x - 현재 위치.x(절대값) < 0.005f 경우 정지
        if (!isTracking && Mathf.Abs(transform.position.x - _targetPos.x) < 0.05f)
        {
            _isMoving = false;
            _timer = 0f;
        }
    }

    // 이동 위치 바라보기
    void LookAtTarget()
    {
        // 대상이 오른쪽
        if (_targetPos.x > transform.position.x)
        {
            if (_skeletonAnim != null)
            {
                // 그대로
                _skeletonAnim.Skeleton.ScaleX = -1f;
            }
        }
        // 대상이 왼쪽
        else if (_targetPos.x < transform.position.x)
        {
            if (_skeletonAnim != null)
            {
                // 그대로
                _skeletonAnim.Skeleton.ScaleX = 1f;
            }
        }
    }

    // 이동 후 대기
    void WaitAtPosiiton()
    {
        _timer += Time.deltaTime;
        if (_timer >= _walkTimer)
        {
            _timer = 0f;
            SetNewTarget();
        }
    }

    // 새로운 위치 탐색
    void SetNewTarget()
    {
        Vector2 randPoint = Random.insideUnitCircle * _walkRange;
        _targetPos = new Vector3(_homePosition.x + randPoint.x, _homePosition.y, _homePosition.z + randPoint.y);
        _isMoving = true;
    }

}
