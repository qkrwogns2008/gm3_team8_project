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
    [SerializeField] private float _walkTimer = 3f;     // 대기시간
    [Header("State")]
    [SerializeField] private EUnitState _currentState = EUnitState.Idle;
    [Header("ForcedAggro")]
    [SerializeField] private bool _isForcedAggro = false;   // 강제 추격 여부
    [SerializeField] private float _forcedAggroTimer = 0f;  // 강제 추격 지속 시간
    [SerializeField] private float AggroDuration = 5f;      // 몇초간 쫒아가는가?

    // 타겟 발견시 속도는 기본과 같음
    #endregion
    #region 내부변수
    private Vector3 _homePosition;                       // 처음 스폰한 위치
    private Vector3 _targetPos;                          // 타겟 위치
    private float _timer = 0f;                           // 대기시간 타이머

    public bool _canAttack = false;         // 공격 스크립트에서 참조

    private SkeletonAnimation _skeletonAnim;

    private CEnemyBase _enemyBase; // 베이스 참조

    #endregion
    #region 프로퍼티
    private float FinalAtkRange => _enemyBase.FinalAtkRange;
    private float FinalDetectionRange => _enemyBase.FinalDetectionRange;
    private float FinalGiveUpRange => _enemyBase.FinalGiveUpRange;
    private float FinalWalkRange => _enemyBase.FinalWalkRange;

    private float MoveSpeed
    {
        get
        {
            if (_enemyBase == null || _enemyBase.EnemyData == null)
            {
                return 2f;
            }
            return _enemyBase.FinalMoveSpeed;
        }
    }
    #endregion


    private void Awake()
    {
        _enemyBase = GetComponent<CEnemyBase>();
        _skeletonAnim = GetComponentInChildren<SkeletonAnimation>();    // 인스펙터에 등록 할 수 있도록

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
        if(_isForcedAggro)
        {
            _forcedAggroTimer += Time.deltaTime;
            if(_forcedAggroTimer <= 0)
            {
                _isForcedAggro = false;
            }
        }
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
                SetAnimation("Idle", true);
                break;
            case EUnitState.Wander:
            case EUnitState.Tracking:
                SetAnimation("Move", true);
                break;
            case EUnitState.Attack:
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

    public void TriggerForcedAggro()
    {
        _isForcedAggro = true;
        _forcedAggroTimer = AggroDuration;
        ChangeState(EUnitState.Tracking);
    }
    void UpdateTracking()
    {
        CUnitBase target = _enemyBase.TargetHero;
        if(_enemyBase.TargetHero == null)
        {
            ChangeState(EUnitState.Idle);
            return;
        }

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;
        float sqrAttackRange = FinalAtkRange * FinalAtkRange;
        float sqrGiveUpRange = FinalGiveUpRange * FinalGiveUpRange;
        // 강제추격중 아닐때만 포기거리 체크
        if (!_isForcedAggro && sqrDist > sqrGiveUpRange)
        {
            ResetTarget();
        }
        else if(sqrDist <= sqrAttackRange)
        {
            ChangeState(EUnitState.Attack);
        }
        else
        {
            Vector3 targetPos = target.transform.position;
            targetPos.z = 0f;
            MoveTo(target.transform.position);
        }

    }
    

    void ResetTarget()
    {
        _enemyBase.SetTarget(null);

        if(_currentState == EUnitState.Tracking || _currentState == EUnitState.Attack)
        {
            ChangeState(EUnitState.Idle);
        }
    }

    void UpdateAttack()
    {
        CUnitBase target = _enemyBase.TargetHero;
        if(_enemyBase.TargetHero == null)
        {
            ChangeState(EUnitState.Idle);
            return;
        }

        _enemyBase.LookAt(target.transform.position);
        _enemyBase.TryAttack(target);

        float sqrDist = (target.transform.position - transform.position).sqrMagnitude;

        if(sqrDist > (FinalAtkRange*FinalAtkRange))
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
        float minsqrDistance = FinalDetectionRange * FinalDetectionRange;


        foreach(var player in HeroManagerDummy.Instance.ActiveHero)
        {
            if(player == null || !player.gameObject.activeSelf)
            {
                continue;
            }
            float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
            if(sqrDist < minsqrDistance)
            {
                minsqrDistance = sqrDist;
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
    void CheckTarget()
    {
        CUnitBase target = _enemyBase.TargetHero;
        if (target == null)
        {
            return;
        }

        if (target.IsUnitDead || !target.gameObject.activeSelf)
        {
            ResetTarget();
        }
    }
    void MoveTo(Vector3 pos)
    {
        if(_enemyBase.IsAttacking)
        {
            return;
        }

        _enemyBase.LookAt(pos);
        Vector3 nextPos = Vector3.MoveTowards(transform.position, pos, MoveSpeed * Time.deltaTime);
        nextPos.z = 0f;
        transform.position = nextPos;

    }

    void SetNewWanderTarget()
    {
        Vector2 rand = Random.insideUnitCircle * FinalWalkRange;
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

    private void OnDrawGizmosSelected()
    {
        // 실시간으로 정보를 가져오기 위해 에디터에서도 베이스를 찾을 수 있도록
        if(_enemyBase == null)
        {
            _enemyBase = GetComponent<CEnemyBase>();
        }
        if(_enemyBase == null || _enemyBase.EnemyData == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FinalDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FinalAtkRange);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, FinalGiveUpRange);

        Vector3 centerPos = Application.isPlaying ? _homePosition : transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(centerPos, FinalWalkRange);

        if(_currentState == EUnitState.Wander || _currentState == EUnitState.Tracking)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPos);
            Gizmos.DrawSphere(_targetPos, 0.3f);
        }

        if(_currentState == EUnitState.Tracking && _enemyBase.TargetHero != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _enemyBase.TargetHero.transform.position);
        }
    }


}
