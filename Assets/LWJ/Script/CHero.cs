using Spine.Unity;
using System.Collections;
using UnityEngine;

public class CHero : CUnitBase
{
	//[Header("일반 공격")]
	//[SpineAnimation(dataField = "_skeletonAni")]
	//[SerializeField] protected string _attackAnimation;
	//[SerializeField] protected EffectDataSO _attackEffect;
	//[SerializeField] protected float _baseAttackDelay = 1.0f;

	[Header("치명타 공격")]
	[SpineAnimation(dataField = "_skeletonAni")]
	[SerializeField] protected string _criticalAnimation;
	[SerializeField] protected EffectDataSO _criticalEffect;
	[SerializeField] protected float _baseCriticalDelay = 1.0f;

	[Header("치명타 수치")]
	[SerializeField] protected float _criticalChance = 20f;
	[SerializeField] protected float _criticalAttackMultiplier = 2f;

	[Header("스킬")]
	[SpineAnimation(dataField = "_skeletonAni")]
	[SerializeField] protected string _skillAnimation;
	[SerializeField] protected EffectDataSO _skillEffect;
	[SerializeField] protected float _baseSkillDelay = 1.0f;
	[SerializeField] protected float _baseSkillCooldown = 1.0f;

	Coroutine _motionRoutine;

	protected float CriticalDamage => _baseAtkDamage * _criticalAttackMultiplier;

	// for Test
	protected override void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnAttack(_targetEnemy);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			OnCritical(_targetEnemy);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			OnSkill(_targetEnemy);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Die();
		}
	}

	protected override void OnAttack(CUnitBase target)
	{
		if (_skeletonAni == null || _attackEffect == null)
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		// 치명타 체크
		bool isCriAttack = (Random.Range(0f, 100f) <= _criticalChance);
		Debug.Log($"크리티컬 : {isCriAttack}");
		if (isCriAttack && _criticalEffect != null)
		{
			_motionRoutine = StartCoroutine(Co_PlayMotion(_criticalEffect, _criticalAnimation, target, CriticalDamage));
			Debug.Log($"{_unitName}의 치명타 공격!");
		}
		else
		{
			_motionRoutine = StartCoroutine(Co_PlayMotion(_attackEffect, _attackAnimation, target, _baseAtkDamage));
			Debug.Log($"{_unitName}의 일반 공격!");
		}
	}

	// for test
	private void OnCritical(CUnitBase target)
	{
		if (_skeletonAni == null || _criticalEffect == null)
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		_motionRoutine = StartCoroutine(Co_PlayMotion(_criticalEffect, _criticalAnimation, target, CriticalDamage));
		Debug.Log($"{_unitName}의 치명타 공격!");
	}

	protected void OnSkill(CUnitBase target)
	{
		if (_skeletonAni == null || _skillEffect == null)
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		_motionRoutine = StartCoroutine(Co_PlayMotion(_skillEffect, _skillAnimation, target, _baseAtkDamage));
		Debug.Log($"{_unitName}의 스킬 1 발동!");
	}

	/// <summary>
	/// 스파인 애니메이션을 재생하고, effectData의 PreDelay 값에 따라 시간차로 이펙트를 생성합니다. 성공적으로 종료되면 피해를 적용합니다.
	/// </summary>
	protected virtual IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, float damage)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			_motionRoutine = null;
			yield break;
		}

		_skeletonAni.AnimationState.SetAnimation(0, animationName, false);
		_skeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

		// 목록의 이펙트를 순차 출력
		for (int i = 0; i < effectData.Catalog.Count; i++)
		{
			EffectCatalog fxData = effectData.Catalog[i];

			yield return new WaitForSeconds(fxData.PreDelay);

			// 이펙트 생성 실패 시 즉시 종료
			if (!TrySummonEffect(fxData))
			{
				Debug.LogWarning($"{name} : {effectData.Name} 이펙트 생성 실패");
				_motionRoutine = null;
				yield break;
			}
		}

		if (target != null)
		{
			target.TakeDamage(damage, this);
		}

		_motionRoutine = null;
	}

	// 오버로딩 : 모션 재생 후 정지.
	protected virtual IEnumerator Co_PlayMotion(string animationName, float time, bool disableAfter)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			_motionRoutine = null;
			yield break;
		}

		_skeletonAni.AnimationState.SetAnimation(0, animationName, false);

		yield return new WaitForSeconds(time);

		if (disableAfter)
		{
			gameObject.SetActive(false);
		}

		_motionRoutine = null;
	}

	// 이펙트 소환 시도
	protected virtual bool TrySummonEffect(EffectCatalog fxData)
	{
		Vector3 pos = transform.position + fxData.Offset;
		Quaternion rot = Quaternion.Euler(-45f, 0f, 0f);
		EffectBase fx = Instantiate(fxData.Prefab, pos, rot);

		if (fx == null)
		{
			return false;
		}
		fx.Init(false);

		return true;
	}

	protected override void Die()
	{
		base.Die();

		if (_skeletonAni == null )
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		_motionRoutine = StartCoroutine(Co_PlayMotion(_deathAnimation, _deathDisableTime, true));
		Debug.Log($"{_unitName} 사망");
	}
}

/*
public virtual void TryAttack(CUnitBase target)
	{
		if (IsAvailable() || target == null) return;
		// Base -> 공통 규칙만 먼저 검사한다.
		// 자식 -> 자기만의 조건과 행동을 제공한다.

		// 실제 행동 수행 -> 자식이 구현하는 부분(abtract)
		// 우선순위 2: 스킬 1
		if (CanUseSkill1())
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
*/
/*
protected virtual void ExecuteCombat(EAttackType type, CUnitBase target)
	{
		switch (type)
		{
			case EAttackType.Skill1:
				Debug.Log($"{_unitName}스킬1 작동");
				_nextSkill1Time = Time.time + _skill1Cooldown;
				ApplyAttackCooldown();
				OnSkill1(target);
				break;
			case EAttackType.Normal:
				Debug.Log($"{_unitName}공격 작동");
				ApplyAttackCooldown();
				OnAttack(target);
				break;
		}
	}
*/
/*
// 1번 스킬 사용 가능 여부 체크 함수
	protected virtual bool CanUseSkill1()
	{
		// 조건 1: 쿨타임 수치가 설정되어 있는가? (0보다 큰가)
		// 조건 2: 현재 시간이 다음 가능 시간보다 지났는가?
		if (_skill1Cooldown > 0f && Time.time >= _nextSkill1Time)
		{
			return true; // 사용 가능!
		}

		return false; // 아직 쿨타임 중이거나 설정 안됨
	}
*/