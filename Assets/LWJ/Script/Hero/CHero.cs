using Spine.Unity;
using System.Collections;
using UnityEngine;

public enum EHeroState
{
	Idle,
	Move,
	Combat,
	Death
}

public class CHero : CUnitBase
{
	protected enum EAttackType
	{
		Normal,
		Critical,
		Skill,
	}

	#region 인스펙터
	[Header("치명타 애니메이션")]
	[SpineAnimation(dataField = "SkeletonAni")]
	[SerializeField] protected string CriticalAnimation;

	[Header("스킬 애니메이션")]
	[SpineAnimation(dataField = "SkeletonAni")]
	[SerializeField] protected string SkillAnimation;

	[Header("유닛 상태")]
	[SerializeField] public EHeroState CurrentState = EHeroState.Idle;

	[Header("공격 기능")]
	[SerializeField] protected bool EnableAttack = true;
	[SerializeField] protected bool EnableCriticalAttack = true;
	[SerializeField] protected bool EnableUseSkill = true;
	#endregion

	#region 내부 변수
	protected HeroDataSO HeroData;

	protected EHeroID HeroID; // ID

	protected float BaseDefense; // 방어력
	protected float DefenseMultiplier = 1.0f; // 방어력 승수

	protected EffectDataSO CriticalEffect; // 치명타 공격 이펙트
	//protected float BaseCriticalActionInterval = 1.5f;
	protected float CriticalChance; // 치명타 확률
	protected float CriticalAttackMultiplier; // 치명타 데미지 승수

	protected EffectDataSO SkillEffect; // 스킬 이펙트
	protected float SkillActionInterval = 1f; // 스킬 액션 딜레이
	protected float BaseSkillDamageRate = 1f; // 스킬 데미지 계수 (1f = 100%)
	protected float BaseSkillCooldown = 5.0f; // 쿨타임
	protected float CooldownMultiplier = 1.0f; // 쿨타임 감소 승수

	protected float NextSkillTime;
	protected bool IsPendingDead = false; // 사망 유예 여부

	protected bool IsFacingRight => (SkeletonAni.skeleton.ScaleX != 1.0f);
	protected virtual float FinalDefense => BaseDefense * DefenseMultiplier;
	protected virtual float CriticalDamage => FinalAttackDamage * CriticalAttackMultiplier;
	protected virtual float FinalSkillActionInterval => SkillActionInterval / AttackSpeedMultiplier;
	protected virtual float FinalSkillDamage => FinalAttackDamage * BaseSkillDamageRate;
	protected virtual float FinalSkillCooldown => BaseSkillCooldown * CooldownMultiplier;

	protected virtual float SpineScale => ScaleMultiplier;
	#endregion

	public event System.Action<float> OnSkillUsed; // 스킬 쿨타임이 인자로 들어감
	public event System.Action OnDead;

	// 영웅 공통 데이터 주입
	protected override void InitUnitStats()
	{
		base.InitUnitStats();

		DeathDisableTime = DeathDisableTime <= 0f ? 3f : DeathDisableTime;

		HeroData = OriginData as HeroDataSO;
		if (HeroData != null)
		{
			HeroID = HeroData.HeroID;
			if (HeroID == EHeroID.None)
			{
				Debug.LogWarning($"{UnitName} ID 설정 필요.");
			}

			BaseDefense = HeroData.BaseDefense;

			AttackEffect = AttackEffect != null ? AttackEffect : HeroData.AttackEffect; // 비었으면 SO에서 할당

			CriticalEffect = HeroData.CriticalEffect;
			CriticalChance = HeroData.CriticalChance;
			CriticalAttackMultiplier = HeroData.CriticalAttackMultiplier;

			SkillEffect = HeroData.SkillEffect;
			SkillActionInterval = HeroData.SkillActionInterval;
			BaseSkillDamageRate = HeroData.BaseSkillDamageRate;
			BaseSkillCooldown = HeroData.BaseSkillCooldown;
			CooldownMultiplier = HeroData.CooldownMultiplier;
		}
	}

	public void ChangeState(EHeroState state)
	{
		if (CurrentState == state && state != EHeroState.Combat)
		{
			return;
		}
		CurrentState = state;

		switch (CurrentState)
		{
			case EHeroState.Idle:
				SetAnimation("Idle", true);
				break;
			case EHeroState.Move:
				SetAnimation("Move", true);
				break;
			case EHeroState.Combat:
				TryAttack(Target);
				break;
		}
	}

	// for Test
	protected override void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnAttack(Target);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			OnCritical(Target);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			OnSkill(Target);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Die();
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			TakeDamage(Random.Range(5f, 20f), this);
		}
	}

	protected override void OnAttack(CUnitBase target)
	{
		ApplyAttackCooldown(true);

		if (SkeletonAni == null)
		{
			Debug.LogWarning("CHero) 인스펙터 null 감지");
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackEffect, AttackAnimation, target, EAttackType.Normal));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 일반 공격!");
		}
	}

	protected virtual void OnCritical(CUnitBase target)
	{
		ApplyAttackCooldown(true);

		if (SkeletonAni == null)
		{
			Debug.LogWarning("CHero) 인스펙터 null 감지");
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalEffect, CriticalAnimation, target, EAttackType.Critical));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 치명타 공격!");
		}
	}

	protected virtual void OnSkill(CUnitBase target)
	{
		ApplyAttackCooldown(false);
		NotifySkillUse();

		if (SkeletonAni == null)
		{
			Debug.LogWarning("CHero) 인스펙터 null 감지");
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		MotionRoutine = StartCoroutine(Co_PlayMotion(SkillEffect, SkillAnimation, target, EAttackType.Skill));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 스킬 발동!");
		}
	}

	// 자식에서 Invoke를 사용하기 위함.
	protected virtual void NotifySkillUse()
	{
		OnSkillUsed?.Invoke(FinalSkillCooldown);
	}

	// 공격 종류에 따라 데미지 처리 로직 분기
	protected virtual void ProcessHit(CUnitBase target, EAttackType type)
	{
		switch (type)
		{
			case EAttackType.Normal:
				ProcessNormalHit(target);
				break;
			case EAttackType.Critical:
				ProcessCriticalHit(target);
				break;
			case EAttackType.Skill:
				ProcessSkillHit(target);
				break;
		}
	}

	protected virtual void ProcessNormalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(FinalAttackDamage, this);
		}
	}

	protected virtual void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(CriticalDamage, this);
		}
	}

	// 영웅 고유 스킬 처리 로직. 자식에서 재정의함.
	protected virtual void ProcessSkillHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(FinalSkillDamage, this);
		}
	}

	/// <summary>
	/// 스파인 애니메이션을 재생하고, effectData의 PreDelay 값에 따라 시간차로 이펙트를 생성합니다. 성공적으로 종료되면 피해를 적용합니다.
	/// </summary>
	protected virtual IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, EAttackType type)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

		if (effectData != null)
		{
			// 목록의 이펙트를 순차 출력
			for (int i = 0; i < effectData.Catalog.Count; i++)
			{
				EffectCatalog fxData = effectData.Catalog[i];

				if (fxData == null)
				{
					Debug.LogWarning($"CHero) 이펙트 NONE. {effectData.Name} 이펙트 목록 확인");
					continue;
				}

				yield return new WaitForSeconds(fxData.PreDelay / AttackSpeedMultiplier);

				if (fxData.Prefab == null)
				{
					Debug.LogWarning($"CHero) 이펙트 프리팹 NONE. {effectData.Name} 이펙트 목록 확인");
					continue;
				}

				// 이펙트 생성 실패 시 즉시 종료
				if (!TrySummonEffect(fxData, transform.position))
				{
					Debug.LogWarning($"{name} : {effectData.Name} 이펙트 생성 실패");
					MotionRoutine = null;
					yield break;
				}
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) 이펙트 null");
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);
		}

		ProcessHit(target, type);

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
	}

	// 오버로딩 : 모션 재생 후 정지.
	protected virtual IEnumerator Co_PlayMotion(string animationName, float time, bool disableAfter)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);

		yield return new WaitForSeconds(time);

		if (disableAfter)
		{
			gameObject.SetActive(false);
		}

		MotionRoutine = null;
	}

	protected virtual bool TrySummonEffect(EffectCatalog fxData, Vector3 position)
	{
		EffectBase prefab = fxData.Prefab;
		if (prefab == null)
		{
			return false;
		}

		Vector3 pos = position + fxData.Offset;
		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);
		EffectBase fx = PoolManager.Instance.Pop(prefab, pos, rot);

		if (fx == null)
		{
			return false;
		}

		EEffectDirection dir;

		if (fxData.IsNoDirection) // 무방향 이펙트인지 체크
		{
			dir = EEffectDirection.None;
		}
		else
		{
			dir = IsFacingRight ? EEffectDirection.Right : EEffectDirection.Left;
		}

		fx.Init(prefab, dir);

		return true;
	}

	public override void TakeDamage(float damage, CUnitBase attacker)
	{
		if (IsDead)
		{
			return;
		}

		// 방어력 연산. 최소 1f의 피해 보장.
		float finalDamage = Mathf.Max(1f, damage - FinalDefense);
		CurrentHp = Mathf.Max(CurrentHp - finalDamage, 0);

		if (PrintLog)
		{
			if (finalDamage > 1)
			{
				Debug.Log($"CUnitBase) [{UnitName}] {finalDamage} 피해 입음. [HP:{CurrentHp}]");
			}
			else
			{
				Debug.Log($"CUnitBase) [{UnitName}] 방어력에 의해 피해 상쇄. [피해:{damage} / 방어력:{FinalDefense}]");
			}
		}

		NotifyHpChange();

		if (CurrentHp <= 0)
		{
			Die();
		}
	}

	protected override void Die()
	{
		base.Die();

		if (SkeletonAni == null)
		{
			return;
		}

		if (MotionRoutine != null)
		{
			IsPendingDead = true;
			return;
		}

		DeathSequence();
	}

	protected virtual void DeathSequence()
	{
		MotionRoutine = StartCoroutine(Co_PlayMotion(DeathAnimation, DeathDisableTime, true));

		if (PrintLog)
		{
			Debug.Log($"{UnitName} 사망");
		}

		OnDead?.Invoke();
	}

	public override void TryAttack(CUnitBase target)
	{
		if (!IsAvailable() || target == null)
		{
			return;
		}

		if (EnableUseSkill && CanUseSkill())
		{
			ExecuteCombat(EAttackType.Skill, target);
		}
		else
		{
			// 치명타 체크
			bool isCriAttack = (Random.Range(0f, 100f) <= CriticalChance);

			if (EnableCriticalAttack && isCriAttack)
			{
				ExecuteCombat(EAttackType.Critical, target);
			}
			else if(EnableAttack)
			{
				ExecuteCombat(EAttackType.Normal, target);
			}
		}
	}

	protected virtual void ExecuteCombat(EAttackType type, CUnitBase target)
	{
		switch (type)
		{
			case EAttackType.Skill:
				OnSkill(target);
				break;
			case EAttackType.Critical:
				OnCritical(target);
				break;
			case EAttackType.Normal:
				OnAttack(target);
				break;
		}
	}

	// 스킬 사용 가능 여부 체크
	protected virtual bool CanUseSkill()
	{
		if (BaseSkillCooldown > 0f && Time.time >= NextSkillTime)
		{
			return true;
		}

		return false; // 아직 쿨타임 중이거나 설정 안됨
	}

	/// <summary>
	/// 공격 액션 딜레이를 적용합니다. 기본 공격이면 True, 스킬 사용이면 False입니다.
	/// </summary>
	/// <param name="isNormal">기본 공격 여부</param>
	protected void ApplyAttackCooldown(bool isNormal)
	{
		if (isNormal) // 기본 공격
		{
			if (FinalAttackActionInterval > 0f)
			{
				NextAttackTime = Time.time + FinalAttackActionInterval;
			}
		}
		else // 스킬 사용
		{
			NextSkillTime = Time.time + FinalSkillCooldown;

			if (FinalSkillActionInterval > 0f)
			{
				NextAttackTime = Time.time + FinalSkillActionInterval;
			}
		}
	}

	public void SetAnimation(string animName, bool loop)
	{
		if (SkeletonAni == null || SkeletonAni.AnimationState == null)
		{
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		Spine.TrackEntry currentTrack = SkeletonAni.AnimationState.GetCurrent(0);

		if (currentTrack == null || currentTrack.Animation.Name != animName)
		{
			SkeletonAni.AnimationState.SetAnimation(0, animName, loop);
		}
	}
}
