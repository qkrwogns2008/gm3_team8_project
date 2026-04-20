using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSnipper : RangedHeroBase
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int SkillAttackCount = 3;

	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 4f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected Coroutine SkillRoutine;
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();

	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	// 치명타 재정의
	protected override void OnCritical(CUnitBase target)
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalAnimation, target, EAttackType.Critical, AudioSO.Critical, AudioSO.CriticalDamaged));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 치명타 공격!");
		}
	}

	protected virtual IEnumerator Co_PlayMotion(string animationName, CUnitBase target, EAttackType type, AudioClip castAudio = null, AudioClip hitAudio = null)
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

		yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);

		if (target != null && hitAudio != null)
		{
			SoundManager.Instance.PlayUnitSFX(hitAudio); // Hit 오디오 재생
		}
		ProcessHit(target, type);

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
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, CriticalHitEffect);
		target.TakeDamage(CriticalDamage, this, false);
	}

	// 스킬 재정의
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

				if (type == EAttackType.Skill)
				{
					ProcessSkillHit(target);
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

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}
		if (SkillRoutine != null)
		{
			return;
		}

		FindedTargets.Clear();

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		FindTargetsOnCircleArea(target, ScaledAreaRadius, targetList);

		SkillRoutine = StartCoroutine(Co_SelectedTargetsAttack(SkillHitEffect, FindedTargets, FinalSkillDamage / SkillAttackCount));

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}
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

	// 목록의 모든 타겟 공격
	protected virtual IEnumerator Co_SelectedTargetsAttack(EffectDataSO effectData, IReadOnlyList<CUnitBase> targets, float damage)
	{
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

				// 선택된 타겟 목록 순회
				for (int j = 0; j < targets.Count; j++)
				{
					CUnitBase target = targets[j];

					if (target == null)
					{
						continue;
					}
					if (target.IsUnitDead)
					{
						continue;
					}

					// 이펙트 생성 실패 시 즉시 종료
					if (!TrySummonEffect(fxData, target.transform.position))
					{
						Debug.LogWarning($"{name} : {effectData.Name} 이펙트 생성 실패");
						SkillRoutine = null;
						yield break;
					}

					if (i != 0) // 조준 이펙트가 아니면
					{
						AudioClip clip = AudioSO.SkillDamaged;
						if (target != null && clip != null)
						{
							SoundManager.Instance.PlayUnitSFX(clip); // 공격 오디오 재생
						}
						target.TakeDamage(damage, this, false);
					}
				}
			}
		}
		else
		{
			Debug.LogWarning($"[{UnitName}] effectData null");
			SkillRoutine = null;
			yield break;
		}

		SkillRoutine = null;
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
		Gizmos.DrawWireSphere(Target.transform.position, ScaledAreaRadius);
	}
}