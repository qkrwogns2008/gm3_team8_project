using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public enum ETeamType { Hero, Enemy } // Hero, Enemy 적 타입 구분
public enum EAttackType { Normal, Skill1, Skill2 } // 스킬 공격 여부
public abstract class CUnitBase : MonoBehaviour
{
    // 모든 상호작용 대상의 공통 규칙을 가진 Base
    #region 인스펙터
    
    [SerializeField] protected UnitDataSO _originData;
    
    [SerializeField] protected string _UnitName ; // 텍스트 확장용
    // 규칙
    [Header("Skill Settings")]
    [SerializeField] protected ETeamType _teamType; // 여기서 Hero인지 Enemy인지 선택
    [SerializeField] private LayerMask _enemyLayer;        // 탐지할 레이어
    // 힌트 기준점
    [SerializeField] private Transform _hintAnchor; // _hintAnchor 기준 상호작용 없을시 transform
    #endregion
    #region 내부 변수
    // 스테이터스
    protected float _currentHp; // 현재 채력
    protected float _currentAtk; // 공격력
    protected float _currentAtkRange; // 공격 범위
    protected float _detectionRange; // 탐지 범위
    protected float _currentwalkSpeed; // 이동속도
    protected float _attackCooldown; // 공격속도 쿨타임
    protected float _skill1Cooldown; // 스킬1 재사용 대기시간
    protected float _skill2Cooldown; // 스킬2 재사용 대기시간
    // 오브젝트
    protected CUnitBase _targetEnemy;   // 현재 목표 타겟 CUnitBase 으로 수정
    protected SkeletonAnimation _skeletonAni; //
    protected GameObject _attack1Prefab;// 기본공격 프리팹
    protected GameObject _skill1Prefab; // 스킬 1 프리팹
    protected GameObject _skill2Prefab; // 스킬 2 프리팹
    protected SpriteRenderer _sprite;   // 자식들도 써야 하니 protected
    // 내부 제어용 함수
    private bool _isMoving = false;
    private float _nextAttackTime = 0f; // 내부 쿨타임 적용 함수
    private float _nextSkill1Time = 0f; // 다음 스킬 가능 시간
    private float _nextSkill2Time = 0f; // 다음 스킬 가능 시간
    private bool _isDead = false; // 사망 여부

    // 코드 이식후 추가됨
    protected Vector3 _targetPos;
    #endregion


    protected virtual void Awake()
    {
        //SO 인스팩터 연결
        InitUnitStats();
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite == null)
        { 
            Debug.Log($"{_UnitName} SpriteRenderer 부재");
        }
    }

    //SO 인스팩터 연결 함수
    protected virtual void InitUnitStats()
    {
        if (_originData != null)
        {
            _UnitName = _originData.UnitName;
            _currentHp = _originData.MaxHp;
            _currentAtk = _originData.AttackPower;
            _currentAtkRange = _originData.AttackRange;
            _skeletonAni = _originData.SkeletonAni;
            _attack1Prefab = _originData.Attack1Prefab;
            _skill1Prefab = _originData.Skill1Prefab;
            _skill2Prefab = _originData.Skill2Prefab;
            _currentwalkSpeed = _originData.WalkSpeed;
        }
        
    }
    // SO 오리진 데이터 가져오기
    protected virtual void Update()
    {
        FindClosesEnemy();

        if (_targetEnemy != null)
        {
            
            _targetPos = _targetEnemy.transform.position; // transform.position 수정

            // 거리 계산
            float distanceToEnemy = Vector3.Distance(transform.position, _targetPos);

            // 가까우면 공격
            if (distanceToEnemy <= _currentAtkRange)
            {
                // 공격 쿨타임 여부 판단
                if (IsAvailable())
                {
                    // 공격 쿨타임 됬을시 공격
                    StopAttack();
                }
                else
                {
                    // 제자리에서 처다보기
                    _isMoving = false;
                    LookAtTarget();
                }
            }
            // 멀면 이동
            else
            {
                _isMoving = true;
                MoveToTarget();
            }
        }
        else
        {
            //대기
            _isMoving = false;
        }
    }




    // 데미지 받을시 호출
    public virtual void TakeDamage(float damage, CUnitBase attacker)
    {
        if (_isDead) return;
        _currentHp -= damage;
        if (_currentHp <= 0) Die();
    }

    // 사망, 공격 시작시 여부 확인
    protected bool IsAvailable()
    {
        if (_isDead)
            return false;
        if (Time.time < _nextAttackTime)
            return false;
        return true;
    }

    protected Vector3 GetHitAnchorPosition()
    {
        // _hintAnchor가 없으면 트랜스폼 위치를 기준점으로 사용한다.
        return (_hintAnchor != null) ? _hintAnchor.position : transform.position;
    }


    // 공격 쿨타임 체크 여부
    protected void ApplyAttackCooldown()
    {
        if (_attackCooldown > 0f)
        {
            _nextAttackTime = Time.time + _attackCooldown;
        }
    }
    // 1번 스킬 사용 가능 여부 체크 함수
    protected bool CanUseSkill1()
    {
        // 조건 1: 쿨타임 수치가 설정되어 있는가? (0보다 큰가)
        // 조건 2: 현재 시간이 다음 가능 시간보다 지났는가?
        if (_skill1Cooldown > 0f && Time.time >= _nextSkill1Time)
        {
            return true; // 사용 가능!
        }

        return false; // 아직 쿨타임 중이거나 설정 안됨
    }
    // 2번 스킬 사용 가능 여부 체크 함수
    protected bool CanUseSkill2()
    {
        if (_skill2Cooldown > 0f && Time.time >= _nextSkill2Time)
        {
            return true;
        }

        return false;
    }
    // 상호작용의 단일 진입점(제일 중요한 함수)
    // 규칙 검사 + 실제 행동을 담당한다.
    public void TryAttack(CUnitBase target)
    {
        if (_isDead || target == null) return;
        // Base -> 공통 규칙만 먼저 검사한다.
        // 자식 -> 자기만의 조건과 행동을 제공한다.
        


        // 실제 행동 수행 -> 자식이 구현하는 부분(abtract)
        if (CanUseSkill2())
        {
            ExecuteCombat(EAttackType.Skill2, target);
        }
        // 우선순위 2: 스킬 1
        else if (CanUseSkill1())
        {
            ExecuteCombat(EAttackType.Skill1, target);
        }
        // 우선순위 3: 일반 공격
        else
        {
            ExecuteCombat(EAttackType.Normal, target);
        }

        // 공통 후처리 진행 : 쿨타임


    }






    protected void ExecuteCombat(EAttackType type, CUnitBase target)
    {
        switch (type)
        {
            case EAttackType.Skill1:
                Debug.Log($"{_UnitName}스킬1 작동");
                _nextSkill1Time = Time.time + _skill1Cooldown;
                ApplyAttackCooldown();
                OnSkill1(target);
                break;

            case EAttackType.Skill2:
                Debug.Log($"{_UnitName}스킬2 작동");
                _nextSkill2Time = Time.time + _skill2Cooldown;
                ApplyAttackCooldown();
                OnSkill2(target);
                break;

            case EAttackType.Normal:
                Debug.Log($"{_UnitName}공격 작동");
                ApplyAttackCooldown();
                OnAttack(target);
                break;
        }
    }
    // 사망시 호출
    protected virtual void Die()
    {
        _isDead = true;
        // 사망 애니메이션 등 추가
    }

    // 외부에서 이 유닛이 어느 팀인지 확인할 때 사용
    public ETeamType Team => _teamType;


    //============================== CAutoPlayerMoving 함수 가져옴==========================

    protected void FindClosesEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _detectionRange, _enemyLayer);

        if (enemies.Length > 0)
        {
            CUnitBase closest = null; // transform -> CUnitBase 수정
            float minDistance = Mathf.Infinity;


            foreach (Collider enemy in enemies)
            {
                // 가까운 대상 거리 계산
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    CUnitBase targetUnit = enemy.GetComponent<CUnitBase>();
                    if (targetUnit != null)
                    {
                        minDistance = distance;
                        closest = targetUnit;
                    }
                }
            }
            // 타겟 설정
            _targetEnemy = closest;
        }
        else
        {
            Debug.Log("타겟 없음");
            _targetEnemy = null;
        }
    }

    protected void MoveToTarget()
    {
        LookAtTarget();
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _currentwalkSpeed * Time.deltaTime);

    }

    // 이동 위치 바라보기
    /// <summary>
    /// 기본 캐릭터 방향이 오른쪽일 경우 그대로
    /// 왼쪽을 바라볼 경우 반대로 뒤집기
    protected void LookAtTarget()
    {
        // 오른쪽
        if (_targetPos.x > transform.position.x)
        {
            if (_sprite != null)
            {
                // 뒤집기
                _sprite.flipX = true;
            }
        }
        // 왼쪽
        else if (_targetPos.x < transform.position.x)
        {
            if (_sprite != null)
            {
                // 그대로
                _sprite.flipX = false;
            }
        }
    }
    /// </summary>

    protected void StopAttack() // <- 이거 함수이름 StopAttack??
    {
        _isMoving = false;
        LookAtTarget();

        // 공격 로직 실행
        TryAttack(_targetEnemy);
    }

    protected virtual void OnAttack(CUnitBase target)
    { }
    protected virtual void OnSkill1(CUnitBase target)
    { }
    protected virtual void OnSkill2(CUnitBase target)
    { }




}
