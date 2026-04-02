using Spine.Unity;
using System.Collections;
using UnityEngine;

public enum ETeamType { Hero, Enemy } // Hero, Enemy 적 타입 구분

// 모든 상호작용 대상의 공통 규칙을 가진 Base
public abstract class CUnitBase : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] protected string UnitName; // 로그용
	[SerializeField] protected float CurrentHp; // 현재 체력

	[Header("유닛 데이터 SO")]
	[SerializeField] protected UnitDataSO OriginData;

	[Header("감지 세팅")]
	[SerializeField] protected ETeamType TeamType; // 여기서 Hero인지 Enemy인지 선택 → 추후 미사용 시 제거

	[Header("스켈레톤 애니메이션")]
	[SerializeField] protected SkeletonAnimation SkeletonAni;

	[Header("일반 공격")]
	[SpineAnimation(dataField = "SkeletonAni")]
	[SerializeField] protected string AttackAnimation;
	[SerializeField] protected EffectDataSO AttackEffect; // 공격 이펙트 없으면 생략

	[Header("사망")]
	[SpineAnimation(dataField = "SkeletonAni")]
	[SerializeField] protected string DeathAnimation;
	[SerializeField] protected float DeathDisableTime;

	[Header("log")]
	[SerializeField] protected bool PrintLog = true;

	/*
	[Header("이동 스크립트")]
	[SerializeField] protected CAutoEnemyMove _moveEnemy;
	[SerializeField] protected CAutoPlayerMove _movePlayer;
	*/
	#endregion

	#region 내부 변수
	// 스테이터스


	protected float BaseMaxHp; // 최대 체력
	protected float BaseAtkDamage; // 공격력
	protected float BaseAttackActionInterval; // 공격 주기(초)
	protected float AtkRange; // 공격 범위
	protected float BaseMoveSpeed; // 이동속도

	// 승수
	protected float MaxHPMultiplier = 1.0f;
	protected float AttackDamageMultiplier = 1.0f;
	protected float AttackSpeedMultiplier = 1.0f;
	protected float MoveSpeedMultiplier = 1.0f;

	protected float DetectionRange;

	protected float NextAttackTime;
	protected CUnitBase Target; // 현재 목표 타겟
	protected bool IsDead = false; // 사망 여부
	protected Coroutine MotionRoutine;

	protected virtual float FinalMaxHP => BaseMaxHp * MaxHPMultiplier; // 1000 * 1.1 (최대 체력 10%증가) = 1100
	protected virtual float FinalAttackDamage => BaseAtkDamage * AttackDamageMultiplier;
	protected virtual float FinalAttackActionInterval => BaseAttackActionInterval / AttackSpeedMultiplier; // 공격 딜레이 (공격 속도 100% 증가 => 공격 딜레이 1/2)
	protected virtual float FinalMoveSpeed => BaseMoveSpeed * MoveSpeedMultiplier;

	private bool _isRegisterd = false; // 중복 등록 방지
	#endregion

	public virtual event System.Action<float, float> OnHpChanged;

	// 외부에서 이 유닛이 어느 팀인지 확인할 때 사용
	public ETeamType Team => TeamType;
	public virtual bool IsUnitDead => IsDead;

	protected virtual void Awake()
	{

        InitUnitStats();
		
		if (SkeletonAni == null)
		{
			SkeletonAni = GetComponent<SkeletonAnimation>();
		}
		if (SkeletonAni == null)
		{
			Debug.LogWarning($"CUnitBase) {UnitName} SkeletonAnimation 부재");
		}
	}
	protected virtual void Start()
	{
		

    }

	protected virtual void Update()
	{
	}

	protected virtual void OnEnable()
	{
		StartCoroutine(CoRegisterWithWaiting());
    }
	

	protected virtual void OnDisable()
	{
		UnRegisterFromManager();
	}

    #region ListManager
    private IEnumerator CoRegisterWithWaiting()
    {
        if (_isRegisterd)
        {
            yield break;
        }

        if (TeamType == ETeamType.Hero)
        {
            yield return new WaitUntil(() => HeroManagerDummy.Instance != null);
            HeroManagerDummy.Instance.RegisterHero(this);

        }
        else if (TeamType == ETeamType.Enemy)
        {
            yield return new WaitUntil(() => CEnemyManager.Instance != null);
            CEnemyManager.Instance.RegisterEnemy(this);
        }

        _isRegisterd = true;

    }
    private void UnRegisterFromManager()
    {
        if (!_isRegisterd)
        {
            return;
        }

        if (TeamType == ETeamType.Hero)
        {
            if (HeroManagerDummy.Instance != null)
            {
                HeroManagerDummy.Instance.UnregisterHero(this);
            }
        }
        else if (TeamType == ETeamType.Enemy)
        {
            if (CEnemyManager.Instance != null)
            {
                CEnemyManager.Instance.UnregisterEnemy(this);
            }
        }

        _isRegisterd = false;
    }
	#endregion

	#region PoolManager

	public GameObject OriginPrefab { get; private set; }

	public void SetOriginPrefab(GameObject prefab)
	{
		if(OriginPrefab != null)
		{
			return;
		}
		OriginPrefab = prefab;
	}

	#endregion

	// SO 데이터 주입 함수
	// 유닛 기본값 세팅
	protected virtual void InitUnitStats()
	{
		if (OriginData != null)
		{
			UnitName = OriginData.UnitName;
			BaseMaxHp = OriginData.BaseMaxHp;
			BaseAtkDamage = OriginData.BaseAttackDamage;
			BaseAttackActionInterval = OriginData.BaseAttackInterval;
			AtkRange = OriginData.AttackRange;
			BaseMoveSpeed = OriginData.BaseMoveSpeed;

			MaxHPMultiplier = OriginData.MaxHPMultiplier;
			AttackDamageMultiplier = OriginData.AttackDamageMultiplier;
			AttackSpeedMultiplier = OriginData.AttackSpeedMultiplier;
			MoveSpeedMultiplier = OriginData.MoveSpeedMultiplier;

			DetectionRange = OriginData.DetectionRange;
			/*
			if (_moveEnemy != null)
			{

			}
			if (_movePlayer != null)
			{

			}
			*/

			CurrentHp = FinalMaxHP;
		}
	}

	// 데미지 받을 시 호출
	public virtual void TakeDamage(float damage, CUnitBase attacker)
	{
		if (IsDead)
		{
			return;
		}

		CurrentHp -= damage;

		if (PrintLog)
		{
			Debug.Log($"CUnitBase) [{UnitName}] {damage} 피해 입음. [HP:{CurrentHp}]");
		}

		OnHpChanged?.Invoke(CurrentHp, FinalMaxHP);

		if (CurrentHp <= 0)
		{
			Die();
		}
	}

	// 사망 시 호출
	protected virtual void Die()
	{
		IsDead = true;
		// 사망 애니메이션 등 추가
	}

	// 공격 가능 여부 확인
	protected virtual bool IsAvailable()
	{
		if (IsDead)
			return false;

		if (Time.time < NextAttackTime)
			return false;

		return true;
	}

	// 공격 후딜레이 적용
	protected virtual void ApplyAttackCooldown()
	{
		if (FinalAttackActionInterval > 0f)
		{
			NextAttackTime = Time.time + FinalAttackActionInterval;
		}
	}

	public virtual void SetTarget(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		Target = target;
	}

	// 상호작용의 단일 진입점(제일 중요한 함수)
	// 규칙 검사 + 실제 행동을 담당한다.
	public virtual void TryAttack(CUnitBase target)
	{
		if (!IsAvailable() || target == null)
		{
			return;
		}

		ExecuteCombat(target);

		// 공통 후처리 진행 : 쿨타임
	}

	// 공격 종류가 추가로 필요할 경우 자식에서 재정의
	protected virtual void ExecuteCombat(CUnitBase target)
	{
		OnAttack(target);
	}

	protected virtual void OnAttack(CUnitBase target)
	{
		ApplyAttackCooldown();

		if (SkeletonAni == null)
		{
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation, target, BaseAtkDamage));

		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 일반 공격!");
			Debug.Log("CUnitBase) OnAttack 호출");
		}
	}

	protected virtual IEnumerator Co_PlayMotion(string animationName, CUnitBase target, float damage)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

		if (target != null)
		{
			target.TakeDamage(damage, this);
		}

		MotionRoutine = null;
	}
}