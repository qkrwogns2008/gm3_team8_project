using Spine.Unity;
using UnityEngine;

public class CAutoPlayerMove : MonoBehaviour
{
    #region 인스펙터
    [Header("On/Off")]
    [SerializeField] private bool _isAutoMove = true;
    
    #endregion

    #region 내부변수
    private CUnitBase _targetEnemy;                     // 현재 목표 타겟
    private CUnitBase _sharedTarget;                    // 매니저가 공유해준 타겟
    private Vector3 _targetPos;                          // 타겟 위치

    private SkeletonAnimation _skeletonAnim;
    private CHero PlayerHero;   // 상태 제어용 참조 사용시 CHero참조
    private HeroDataSO _heroData;

    private Vector3 _groupTargetPos;                // 내 대열 위치
    private Vector3 _guardPosition;                 // 수동모드일 경우 고정자리

    private float _separationRadius = 6.0f;
    private float _pushForce = 10.0f;
    #endregion


    private void Awake()
    {
        
        PlayerHero = GetComponent<CHero>();
        _skeletonAnim = GetComponentInChildren<SkeletonAnimation>();

        if (_skeletonAnim == null)
        {
            Debug.LogError($"{gameObject.name}: SkeletonAnim null");
        }
    }

    void Start()
    {

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
    void Update()
    {

        if (PlayerHero == null || PlayerHero.IsUnitDead)
        {
            return;
        }
        // 데이터 로드 체크
        if (_heroData == null && PlayerHero.BaseData != null)
        {
            _heroData = PlayerHero.BaseData as HeroDataSO;
        }

        if(_heroData == null)
        {
            return;
        }
		
        // 제어 로직
        if(CGroupManager.instance != null && CGroupManager.instance.IsJoystickActive)
        {
            //Debug.Log($"{gameObject.name}: 조이스틱 활성 상태 확인됨. ManualGroupMove 호출!");
            // 조이스틱 사용 시 대열 이동
            ManualGroupMove();
        }
        else
        {
            // 그 이외 상황 자동 전투 및 복귀
            HandleAutoCombat();

            if(PlayerHero.CurrentState == EHeroState.Combat || PlayerHero.CurrentState == EHeroState.Idle)
            {
                ApplySeparation();
            }
        }

    }

    // 매니저가 타겟을 쏴주고
    public void SetSharedTarget(CUnitBase target)
    {
        _sharedTarget = target;
    }


    // 매니저가 지정해둔 대열 위치로 이동
    // 수동 조작 사용중일때 호출
    void ManualGroupMove()
    {
        // 상태 변경
        PlayerHero.ChangeState(EHeroState.Move);

        Vector3 targetPos = _groupTargetPos;

        float followSpeed = CGroupManager.instance.GroupSpeed * 1.5f;

        // 현 위치에서 대열 위치로 따라잡기
        transform.position = Vector3.MoveTowards(transform.position, _groupTargetPos, followSpeed * Time.deltaTime);

        // 이동방향 바라보기
        float joystickX = CGroupManager.instance.JoystickX;
        if(_skeletonAnim != null && Mathf.Abs(joystickX) > 0.1f)
        {
            // 조이스틱에 따른 방향전환
            _skeletonAnim.Skeleton.ScaleX = (joystickX > 0) ? -1f : 1f;
        }

        // 수동 이동중에 공격대상 비우기
        _targetEnemy = null;
        _sharedTarget = null;

        // 조이스틱 이동중일때 내 위치 갱신
        if(!_isAutoMove)
        {
            _guardPosition = transform.position;
            _guardPosition.z = 0f;
        }
    }

    public void SetGroupTarget(Vector3 pos)
    {
        _groupTargetPos = pos;
    }

    #region 자동전투
    void HandleAutoCombat()
    {
        CheckTarget();

        if(_targetEnemy == null)
        {
            DetermineTarget();
        }
        if(_targetEnemy != null)
        {
            _targetPos = _targetEnemy.transform.position;
            _targetPos.z = 0f;

            float sqrDistance = (transform.position - _targetPos).sqrMagnitude;
            float sqrAttackRange = PlayerHero.FinalAtkRange * PlayerHero.FinalAtkRange;

            if(sqrDistance <= sqrAttackRange)
            {
                // 사거리 내부
                StopAndAttack();
            }
            else
            {
                if(_isAutoMove)
                {
                    MoveToTarget();
                }
                else
                {
                    ReturnToGroupTarget(_groupTargetPos);
                }
            }
        }
        else
        {
            ReturnToGroupTarget(_groupTargetPos);
        }
    }

    // 어떤 적을 타겟할지
    void DetermineTarget()
    {
        // 타겟이 유효한지
        if(_sharedTarget != null && !_sharedTarget.IsUnitDead && _sharedTarget.gameObject.activeSelf)
        {
            _targetEnemy = _sharedTarget;
        }
        // 공유 타겟이 없다면
        else
        {
            // 내 위치에서 가까운 적 찾기
            //FindClosestEnemy();
        }
        if(_targetEnemy != null)
        {
            PlayerHero.SetTarget(_targetEnemy);
        }
    }

    // 자리로 복귀
    void ReturnToGroupTarget(Vector3 destPos)
    {
        float dist = Vector3.Distance(transform.position, destPos);
        if (dist > 0.1f)
        {
            PlayerHero.ChangeState(EHeroState.Move);
            transform.position = Vector3.MoveTowards(transform.position, destPos, PlayerHero.FinalMoveSpeed * Time.deltaTime);

            if (_skeletonAnim != null)
            {
                _skeletonAnim.Skeleton.ScaleX = (destPos.x > transform.position.x) ? -1f : 1f;
            }
        }
        else
        {
            if(PlayerHero.CurrentState != EHeroState.Idle)
            {
                PlayerHero.ChangeState(EHeroState.Idle);
            }
        }
    }
    #endregion

    public void SyncAutoMode(bool isActive)
    {
        _isAutoMove = isActive;

        if(!_isAutoMove)
        {
            _targetEnemy = null;
            PlayerHero.ChangeState(EHeroState.Idle);

            _guardPosition = transform.position;
            _guardPosition.z = 0f;
        }
    }

    public void ToggleAutoMode()
    {
        if(CGroupManager.instance != null)
        {
            CGroupManager.instance.ToggleTeamAutoMode();
        }
    }

    // 대상 체크
    void CheckTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        
        if(_targetEnemy.IsUnitDead || !_targetEnemy.gameObject.activeSelf)
        {
            _targetEnemy = null;
            PlayerHero.SetTarget(null);
        }
    }

    // 이동 위치 탐색
    void MoveToTarget()
    {
        PlayerHero.ChangeState(EHeroState.Move);
        LookAtTarget();

        float moveSpeed = PlayerHero.FinalMoveSpeed;

        transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);

        // Z 축 고정
        Vector3 curPos = transform.position;
        curPos.z = 0f;
        transform.position = curPos;

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

    // 겹치는 영웅 밀어내기
    void ApplySeparation()
    {
        if(CGroupManager.instance == null)
        {
            return;
        }

        Vector3 pushVector = Vector3.zero;

        foreach(var pair in CGroupManager.instance.ActiveHeroes)
        {
            CAutoPlayerMove otherHero = pair.Value;

            if(otherHero == null || otherHero == this)
            {
                continue;
            }
            if(!otherHero.gameObject.activeInHierarchy || otherHero.PlayerHero.IsUnitDead)
            {
                continue;
            }

            Vector3 diff = transform.position - otherHero.transform.position;
            float dist = diff.magnitude;

            if (dist < _separationRadius && dist > 0.001f)
            {
                float pushWeight = 1f - (dist / _separationRadius);

                pushVector += diff.normalized * pushWeight;
            }
        }
        
        if(pushVector != Vector3.zero)
        {
            transform.position += pushVector * _pushForce * Time.deltaTime;

            Vector3 curPos = transform.position;
            curPos.z = 0f;
            transform.position = curPos;
        }
    }

    void StopAndAttack()
    {
        LookAtTarget();

		PlayerHero.ChangeState(EHeroState.Combat);
    }
}
