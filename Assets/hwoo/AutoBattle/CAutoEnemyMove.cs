using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CAutoEnemyMove : MonoBehaviour
{
    #region 인스펙터
    [Header("Green : Detect Range ")]
    [Header("Red : CanAttack Range")]
    [Header("Blue : Walk Range")]
    [Header("Yellow : Move Target")]
    [Header("Move")]
    [SerializeField] private float _walkspeed = 2f;             // 이동속도 (임시) 이후 외부에서 받아올것
    [SerializeField] private float _attackRange = 4f;           // 사거리 (임시)
    [SerializeField] private float _walkRange = 10f;    // 주변 돌아다니는 범위
    [SerializeField] private float _walkTimer = 3f;     // 대기시간
    [Header("Tracking")]
    [SerializeField] private float _detectionRange = 10f;    // 탐지 범위
    [SerializeField] private float _giveUpRange = 12f;      // 추격 포기 범위
    [SerializeField] private LayerMask _playerLayer;        // 탐지할 레이어
    
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

    private CEnemyBase _enemyBase;
    #endregion

    void Start()
    {
        // 현재 위치 세팅
        _homePosition = transform.position;

        _enemyBase = GetComponent<CEnemyBase>();
        _skeletonAnim = GetComponent<SkeletonAnimation>();
        // 이동할 위치 선택
        SetNewTarget();
    }
    void Update()
    {
        // 대상이 없을경우 추적
       if(_targetPlayer == null)
        {
            SerachForTarget();
        }
       if(_targetPlayer != null)
        {
            Tracking();
        }
        else
        {
            Wandering();
        }
       if(!_isMoving)
        {
            SetAnimation("Idle", true);
        }
        
    }
    private void SetAnimation(string animName, bool loop)
    {
        if (_skeletonAnim.AnimationName != animName)
        {
            _skeletonAnim.AnimationState.SetAnimation(0, animName, loop);
        }
    }


    // 플레이어 추적
    void SerachForTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, _detectionRange, _playerLayer);

        if(targets.Length > 0)
        {
            float minDistance = Mathf.Infinity;
            foreach (Collider collider in targets)
            {
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                if(dist < minDistance)
                {
                    minDistance = dist;
                    _targetPlayer = collider.transform;
                }
            }

        }
    }

    // 추적 상태
    void Tracking()
    {
        float distance = Vector3.Distance(transform.position, _targetPlayer.position);

        if(distance > _giveUpRange)
        {
            Debug.Log("추격 대상 범위 이탈");
            _targetPlayer = null;
            _canAttack = false;

            _isMoving = false;
            _timer = 0;
            return;
        }
        _targetPos = _targetPlayer.position;

        // 공격 사거리 내부
        if(distance < _attackRange)
        {
            _isMoving = false;
            _canAttack = true;
            LookAtTarget();
        }
        // giveUp 구간 내부 / 공격 사거리 밖
        else
        {
            _isMoving = true;
            _canAttack = false;
            MoveToTarget(true);
        }
    }

    // 타겟이 없을때
    void Wandering()
    {
        if (_isMoving)
        {
            MoveToTarget(false);
        }
        else
        {
            WaitAtPosition();
        }
    }

    // 이동 위치 탐색
    void MoveToTarget(bool isTracking)
    {
        LookAtTarget();
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkspeed * Time.deltaTime);
        SetAnimation("Move", true);
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
    void WaitAtPosition()
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

    // 기즈모로 범위 확인
    private void OnDrawGizmos()
    {
        // 탐지 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // 사거리
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        // 배회 범위
        Vector3 centerPos = Application.isPlaying ? _homePosition : transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(centerPos, _walkRange);

        // 현재 이동하는 위치
        if(_isMoving)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPos);
            Gizmos.DrawSphere(_targetPos, 0.3f);
        }
    }

}
