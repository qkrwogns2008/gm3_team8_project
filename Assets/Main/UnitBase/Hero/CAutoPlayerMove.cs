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
            Debug.Log($"{gameObject.name}: 조이스틱 활성 상태 확인됨. ManualGroupMove 호출!");
            // 조이스틱 사용 시 대열 이동
            ManualGroupMove();
        }
        else
        {
            // 그 이외 상황 자동 전투 및 복귀
            HandleAutoCombat();
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

        // 복귀 속도 보정
        float baseSpeed = PlayerHero.FinalMoveSpeed;

        // 현 위치와 대열 목적지 사이의 거리 계산
        float distance = Vector3.Distance(transform.position, _groupTargetPos);

        // 거리가 2.0 이상 멀어지면 기본속도의 1.5배, 가까우면 1.1배
        float returnSpeedMultiplier = (distance > 2.0f) ? 1.5f : 1.1f;
        float finalMoveSpeed = baseSpeed * returnSpeedMultiplier;

        // 지정 좌표 이동
        transform.position = Vector3.MoveTowards(transform.position, _groupTargetPos, finalMoveSpeed * Time.deltaTime);

        // 이동방향 바라보기
        float joystickX = CGroupManager.instance.JoystickX;
        if(_skeletonAnim != null && Mathf.Abs(joystickX) > 0.1f)
        {
            // 조이스틱에 따른 방향전환
            _skeletonAnim.Skeleton.ScaleX = (joystickX > 0) ? -1f : 1f;
        }

        // 수동 이동중에 공격대상 비우기
        _targetEnemy = null;
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
                    PlayerHero.ChangeState(EHeroState.Idle);
                }
            }
        }
        else
        {
            // 적이 없을때: 대열 위치로 복귀
            float distToGroup = Vector3.Distance(transform.position, _groupTargetPos);
            if (distToGroup > 0.5f)
            {
                PlayerHero.ChangeState(EHeroState.Move);
                transform.position = Vector3.MoveTowards(transform.position, _groupTargetPos, PlayerHero.FinalMoveSpeed * Time.deltaTime);

                // 대열 복귀시 바라보는 방향
                if(_skeletonAnim != null)
                {
                    _skeletonAnim.Skeleton.ScaleX = (_groupTargetPos.x > transform.position.x) ? -1f : 1f;
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
    #endregion
    // 버튼 연결
    public void ToggleAutoMode()
    {
        _isAutoMove = !_isAutoMove;
        _targetEnemy = null;
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

    void FindClosestEnemy()
    {
        if(CEnemyManager.Instance == null || CEnemyManager.Instance.ActiveEnemies.Count == 0)
        {
            _targetEnemy = null;
            PlayerHero.SetTarget(null);
            return;
        }

        CUnitBase closest = null;

        float detectionRange = PlayerHero.FinalDetectionRange;
        float minSqrDistance = detectionRange * detectionRange;

        foreach(CUnitBase enemy in CEnemyManager.Instance.ActiveEnemies)
        {
            if(enemy == null || !enemy.gameObject.activeSelf || enemy.IsUnitDead)
            {
                continue;
            }
            float sqrDist = (enemy.transform.position - transform.position).sqrMagnitude;

            if(sqrDist< minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closest = enemy;
            }
            
        }
        if(closest!=null)
        {
            _targetEnemy = closest;
            PlayerHero.SetTarget(closest);
        }
        else
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

    void StopAndAttack()
    {
        LookAtTarget();

		PlayerHero.ChangeState(EHeroState.Combat);
    }
}
