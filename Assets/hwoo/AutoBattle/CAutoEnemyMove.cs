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
    [Header("State")]
    [SerializeField] private EUnitState _currentState = EUnitState.Idle;

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

    private CEnemyBase _enemyBase; // 베이스 참조
    #endregion



    private void Awake()
    {
        _enemyBase = GetComponent<CEnemyBase>();
        _skeletonAnim = GetComponentInChildren<SkeletonAnimation>();

        if(_skeletonAnim == null)
        {
            Debug.LogError($"{gameObject.name}: SkeletonAnim null");
        }
    }

    void Start()
    {
        // 현재 위치 세팅
        _homePosition = transform.position;
        _homePosition.z = 0f;
        ChangeState(EUnitState.Idle);
        
        // 이동할 위치 선택
    }

    void Update()
    {
        if (_enemyBase == null || _enemyBase.IsUnitDead)
        {
            return;
        }
        CheckTarget();
        switch (_currentState)
        {
            case EUnitState.Idle:
                UpdateIdle();
                break;
            case EUnitState.Wander:
                UpdateWander();
                break;
            case EUnitState.Tracking:
                UpdateTracking();
                break;
            case EUnitState.Attack:
                UpdateAttack();
                break;
        }

    }

    private void OnEnable()
    {
        _homePosition = transform.position;
        _homePosition.z = 0f;
        ChangeState(EUnitState.Idle);
    }

    public void ChangeState(EUnitState newState)
    {
        if (_currentState == newState && newState != EUnitState.Attack)
        {
            return;
        }

        _currentState = newState;

        switch (_currentState)
        {
            case EUnitState.Idle:
                _isMoving = false;
                SetAnimation("Idle", true);
                break;
            case EUnitState.Wander:
            case EUnitState.Tracking:
                _isMoving = true;
                SetAnimation("Move", true);
                break;
            case EUnitState.Attack:
                _isMoving = false;
                //UpdateAttack();
                break;
        }
    }

    // 상태별 함수

    void UpdateIdle()
    {
        if(FindTarget())
        {
            ChangeState(EUnitState.Tracking);
            return;
        }
        _timer += Time.deltaTime;
        if(_timer >= _walkTimer)
        {
            _timer = 0;
            SetNewWanderTarget();
            ChangeState(EUnitState.Wander);
        }
    }

    void UpdateWander()
    {
        if(FindTarget())
        {
            ChangeState(EUnitState.Tracking);
            return;
        }
        MoveTo(_targetPos);
        if(Vector2.Distance(transform.position, _targetPos) < 0.1f)
        {
            ChangeState(EUnitState.Idle);
        }
    }

    void UpdateTracking()
    {
        CUnitBase target = _enemyBase.TargetHero;
        if(_targetPlayer == null)
        {
            ChangeState(EUnitState.Idle);
            return;
        }

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;
        float sqrAttackRange = _attackRange * _attackRange;
        float sqrGiveUpRange = _giveUpRange * _giveUpRange;

        if (sqrDist > _giveUpRange)
        {
            _targetPlayer = null;
            ChangeState(EUnitState.Idle);
        }
        else if(sqrDist <= _attackRange)
        {
            ChangeState(EUnitState.Attack);
        }
        else
        {
            MoveTo(_targetPlayer.position);
        }

    }

    void CheckTarget()
    {
        if(_targetPlayer == null)
        {
            return;
        }

        CUnitBase playerbase = _targetPlayer.GetComponent<CUnitBase>();

        if (playerbase == null || playerbase.IsUnitDead || !_targetPlayer.gameObject.activeSelf)
        {
            ResetTarget();
            return;
        }

        float dist = Vector2.Distance(transform.position, _targetPlayer.position);
        if (dist > _giveUpRange)
        {
            ResetTarget();
        }
    }

    void ResetTarget()
    {
        _targetPlayer = null;

        if(_currentState == EUnitState.Tracking || _currentState == EUnitState.Attack)
        {
            ChangeState(EUnitState.Idle);
        }
    }

    void UpdateAttack()
    {
        if(_targetPlayer == null)
        {
            ChangeState(EUnitState.Idle);
            return;
        }
        _enemyBase.LookAt(_targetPlayer.position);

        _enemyBase.TryAttack(_targetPlayer.GetComponent<CUnitBase>());

        float dist = Vector3.Distance(transform.position, _targetPlayer.position);
        if(dist > _attackRange)
        {
            ChangeState(EUnitState.Tracking);
        }
    }

    // 여기까지 상태

    bool FindTarget()
    {
        if (HeroManagerDummy.Instance == null || HeroManagerDummy.Instance.ActiveHero.Count == 0)
        {
            return false;
        }

        CUnitBase closetPlayer = null;
        float minDistance = _detectionRange * _detectionRange;

        foreach(var player in HeroManagerDummy.Instance.ActiveHero)
        {
            if(player == null || !player.gameObject.activeSelf)
            {
                continue;
            }
            float dist = Vector2.Distance(player.transform.position, transform.position);
            if(dist < minDistance)
            {
                minDistance = dist;
                closetPlayer = player;
            }
        }

        if(closetPlayer != null)
        {
            _enemyBase.SetTarget(closetPlayer);
            return true;
        }
        return false;
    }

    void MoveTo(Vector3 pos)
    {
        _enemyBase.LookAt(pos);
        Vector3 nextPos = Vector3.MoveTowards(transform.position, pos, _walkspeed * Time.deltaTime);
        nextPos.z = 0f;
        transform.position = Vector3.MoveTowards(transform.position, pos, _walkspeed * Time.deltaTime);
    }

    void SetNewWanderTarget()
    {
        Vector2 rand = Random.insideUnitCircle * _walkRange;
        _targetPos = _homePosition + (Vector3)rand;
    }

   

    private void SetAnimation(string animName, bool loop)
    {
        if(_skeletonAnim == null)
        {
            return;
        }
        if(_skeletonAnim.AnimationState == null)
        {
            return;
        }
        if (_skeletonAnim.AnimationName != animName)
        {
            _skeletonAnim.AnimationState.SetAnimation(0, animName, loop);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, _giveUpRange);

        Vector3 centerPos = Application.isPlaying ? _homePosition : transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(centerPos, _walkRange);

        if(_currentState == EUnitState.Wander || _currentState == EUnitState.Tracking)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPos);
            Gizmos.DrawSphere(_targetPos, 0.3f);
        }

        if(_currentState == EUnitState.Tracking && _targetPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetPlayer.position);
        }
    }


}
