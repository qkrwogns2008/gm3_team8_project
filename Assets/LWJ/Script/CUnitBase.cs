using Spine.Unity;
using UnityEngine;

public enum ETeamType { Hero, Enemy } // Hero, Enemy 적 타입 구분
public enum EAttackType { Normal, Skill } // 스킬 공격 여부

// 모든 상호작용 대상의 공통 규칙을 가진 Base
public abstract class CUnitBase : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] protected string _unitName; // 로그용

	[Header("유닛 데이터 SO")]
	[SerializeField] protected UnitDataSO _originData;

	[Header("감지 세팅")]
	[SerializeField] protected ETeamType _teamType; // 여기서 Hero인지 Enemy인지 선택
	[SerializeField] protected LayerMask _enemyLayer;        // 탐지할 레이어
	[SerializeField] protected float _detectionRange; // 탐지 범위

	[Header("Skill Settings")]
	// 힌트 기준점
	[SerializeField] protected Transform _hintAnchor; // _hintAnchor 기준 상호작용 없을시 transform

	[Header("스켈레톤 애니메이션")]
	[SerializeField] protected SkeletonAnimation _skeletonAni;

	[Header("일반 공격")]
	[SpineAnimation(dataField = "_skeletonAni")]
	[SerializeField] protected string _attackAnimation;
	[SerializeField] protected EffectDataSO _attackEffect; // 공격 이펙트 없으면 생략

	[Header("사망")]
	[SpineAnimation(dataField = "_skeletonAni")]
	[SerializeField] protected string _deathAnimation;
	[SerializeField] protected float _deathDisableTime;
	#endregion

	#region 내부 변수
	// 스테이터스
	protected float _baseMaxHp; // 최대 채력
	protected float _currentHp; // 현재 채력
	protected float _baseAtkDamage; // 공격력
	protected float _baseAttackDelay; // 공격 딜레이(초)
	protected float _atkRange; // 공격 범위
	protected float _baseMoveSpeed; // 이동속도

	// 승수
	protected float _maxHPMultiplier = 1.0f;
	protected float _attackDamageMultiplier = 1.0f;
	protected float _attackSpeedMultiplier = 1.0f;
	protected float _moveSpeedMultiplier = 1.0f;

	protected float _nextAttackTime;
	protected CUnitBase _targetEnemy; // 현재 목표 타겟
	protected bool _isMoving = false;
	protected bool _isDead = false; // 사망 여부

	protected float MaxHP => _baseMaxHp * _maxHPMultiplier;
	protected float AttackDamage => _baseAtkDamage * _attackDamageMultiplier;
	protected float AttackDelay => _baseAttackDelay / _attackSpeedMultiplier; // 공격 딜레이 (공격 속도 100% 증가 => 공격 딜레이 1/2)
	protected float MoveSpeed => _baseMoveSpeed * _moveSpeedMultiplier;

	
	//protected Vector3 _targetPos;
	protected float _currentAtk; // conflict 방지를 위한 임시 선언. 추후 삭제 예정
	#endregion

	// 외부에서 이 유닛이 어느 팀인지 확인할 때 사용
	public ETeamType Team => _teamType;

	protected virtual void Awake()
	{
		InitUnitStats();
		
		if (_skeletonAni == null)
		{
			_skeletonAni = GetComponent<SkeletonAnimation>();
		}
		if (_skeletonAni == null)
		{
			Debug.LogWarning($"{_unitName} SkeletonAnimation 부재");
		}
	}

	protected virtual void Update()
	{
	}

	// SO 데이터 주입 함수
	// 유닛 기본값 세팅
	protected virtual void InitUnitStats()
	{
		if (_originData != null)
		{
			_unitName = _originData.UnitName;
			_baseMaxHp = _originData.BaseMaxHp;
			_baseAtkDamage = _originData.BaseAttackDamage;
			_baseAttackDelay = _originData.BaseAttackDelay;
			_atkRange = _originData.AttackRange;
			_baseMoveSpeed = _originData.BaseMoveSpeed;

			_maxHPMultiplier = _originData.MaxHPMultiplier;
			_attackDamageMultiplier = _originData.AttackDamageMultiplier;
			_attackSpeedMultiplier = _originData.AttackSpeedMultiplier;
			_moveSpeedMultiplier = _originData.MoveSpeedMultiplier;

			_currentHp = MaxHP;
		}
	}

	// 데미지 받을 시 호출
	public virtual void TakeDamage(float damage, CUnitBase attacker)
	{
		if (_isDead)
		{
			return;
		}

		_currentHp -= damage;
		if (_currentHp <= 0)
		{
			Die();
		}
	}

	// 사망 시 호출
	protected virtual void Die()
	{
		_isDead = true;
		// 사망 애니메이션 등 추가
	}

	// 공격 가능 여부 확인
	protected virtual bool IsAvailable()
	{
		if (_isDead)
			return false;

		if (Time.time < _nextAttackTime)
			return false;

		return true;
	}

	protected virtual Vector3 GetHitAnchorPosition()
	{
		// _hintAnchor가 없으면 트랜스폼 위치를 기준점으로 사용한다.
		return (_hintAnchor != null) ? _hintAnchor.position : transform.position;
	}

	// 공격 쿨타임 체크 여부
	protected virtual void ApplyAttackCooldown()
	{
		if (AttackDelay > 0f)
		{
			_nextAttackTime = Time.time + AttackDelay;
		}
	}

	// 상호작용의 단일 진입점(제일 중요한 함수)
	// 규칙 검사 + 실제 행동을 담당한다.
	public virtual void TryAttack(CUnitBase target)
	{
		if (IsAvailable() || target == null)
		{
			return;
		}

		ExecuteCombat(EAttackType.Normal, target);

		// 공통 후처리 진행 : 쿨타임
	}

	// 공격 종류가 추가로 필요할 경우 자식에서 재정의
	protected virtual void ExecuteCombat(EAttackType type, CUnitBase target)
	{
		switch (type)
		{
			case EAttackType.Normal:
				ApplyAttackCooldown();
				OnAttack(target);
				break;
		}
	}

	protected virtual void OnAttack(CUnitBase target)
	{
		if (_skeletonAni == null)
		{
			return;
		}

		// 코루틴 → 스켈레톤 재생 + 데미지 처리 로직 (TakeDamage)
	}
}