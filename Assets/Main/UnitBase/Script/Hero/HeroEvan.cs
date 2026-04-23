using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEvan : CHero
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 4f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();

	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, EAttackType type, AudioClip castAudio = null)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
		if (castAudio != null)
		{
			SoundManager.Instance.PlayUnitSFX(castAudio); // 공격 오디오 재생
		}

		if (effectData != null)
		{
			// 목록의 이펙트를 순차 출력
			for (int i = 0; i < effectData.Catalog.Count; i++)
			{
				EffectInfo fxData = effectData.Catalog[i];

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

				if (type == EAttackType.Skill)
				{
					ProcessHit(target, type);
					yield return new WaitForSeconds(0.6f / AttackSpeedMultiplier);
				}
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) effectData null");
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);
		}

		if (type != EAttackType.Skill)
		{
			ProcessHit(target, type);
		}

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
		else
		{
			// 조이스틱 작동 중인지 체크
			if (CGroupManager.instance != null && CGroupManager.instance.IsJoystickActive)
			{
				ChangeState(EHeroState.Move);
			}
			else
			{
				ChangeState(EHeroState.Idle);
			}
		}
	}

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(CriticalDamage, this);
			AddHPByRatio(0.08f);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		FindedTargets.Clear();

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		FindTargetsOnCircleArea(this, ScaledAreaRadius, targetList);

		SelectedTargetsAttack(FindedTargets, FinalSkillDamage);

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}

		BuffSystem.AddBuff(EBuffFlags.DefenseBoost, 0.2f, 10f, this);
	}

	// 주변 타겟 탐색
	protected virtual void FindTargetsOnCircleArea(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList)
	{
		Vector2 areaCenterPos = originTarget.transform.position;
		float sqrRadius = radius * radius;

		for (int i = 0; i < targetList.Count; i++)
		{
			CUnitBase target = targetList[i];

			if (target == null)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			Vector2 targetPos = target.transform.position;
			Vector2 toTarget = targetPos - areaCenterPos;

			if (toTarget.sqrMagnitude > sqrRadius)
			{
				continue;
			}
			FindedTargets.Add(target);
		}
	}

	protected virtual void SelectedTargetsAttack(IReadOnlyList<CUnitBase> targets, float damage)
	{
		// 선택된 타겟 목록 순회
		for (int i = 0; i < targets.Count; i++)
		{
			CUnitBase target = targets[i];

			if (target == null)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			target.TakeDamage(damage, this);
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if (Target == null)
		{
			return;
		}
		if (Target.IsUnitDead)
		{
			return;
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, ScaledAreaRadius);
	}
}