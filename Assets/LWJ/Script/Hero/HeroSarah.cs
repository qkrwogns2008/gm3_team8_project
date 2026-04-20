using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSarah : CHero
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int CriticalAttackCount = 2;

	[Header("스킬 속성값")]
	[SpineAnimation(dataField = "SkeletonAni")]
	[SerializeField] protected string SkillAnimation2;
	[SerializeField] protected EffectDataSO SkillEffect2;
	[SerializeField] protected float TeleportOffset = 5.0f;
	[SerializeField] protected float TeleportWaitTime = 0.5f;

	[SerializeField] protected AudioClip Skill2;
	[SerializeField] protected AudioClip SkillDamaged2;

	[SerializeField] protected float AreaRadius = 3f;

	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected bool isSkillUsing = false;
	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override void OnDisable()
	{
		base.OnDisable();
		isSkillUsing = false;
	}

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
			if ((type == EAttackType.Critical) &&
				(effectData.Catalog.Count != CriticalAttackCount))
			{
				Debug.LogWarning($"[{unitName}] 공격 횟수, 이펙트 불일치");
			}

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

				if (isSkillUsing)
				{
					ProcessTeleportHit(target);
					yield break;
				}
				else
				{
					ProcessHit(target, type);
				}
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) 이펙트 null");
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);

			if (type != EAttackType.Skill)
			{
				ProcessHit(target, type);
			}
		}

		if (type != EAttackType.Skill)
		{
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
	}

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(CriticalDamage / CriticalAttackCount, this);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		isSkillUsing = true;
		MotionRoutine = StartCoroutine(Co_TeleportToTargetBehind(target));
	}

	protected virtual IEnumerator Co_TeleportToTargetBehind(CUnitBase target)
	{
		isInvincible = true; // 무적 활성화
		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 무적 {isInvincible}");
		}

		yield return new WaitForSeconds(TeleportWaitTime);

		float offsetX = target.IsFacingRight ? -TeleportOffset : TeleportOffset;
		Vector3 pos = target.transform.position + new Vector3(offsetX, 0, 0);

		transform.position = pos;
		isInvincible = false; // 무적 비활성화
		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 무적 {isInvincible}");
		}

		yield return StartCoroutine(Co_PlayMotion(SkillEffect2, SkillAnimation2, target, EAttackType.Skill, Skill2));

		isSkillUsing = false;
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

	protected virtual void ProcessTeleportHit(CUnitBase target)
	{
		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		CircleAreaAttack(target, ScaledAreaRadius, targetList, FinalSkillDamage);
		if (target != null && !target.IsUnitDead && SkillDamaged2)
		{
			SoundManager.Instance.PlayUnitSFX(SkillDamaged2);
		}
	}

	protected virtual void CircleAreaAttack(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList, float damage)
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

			target.TakeDamage(damage, this, false);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{damage}]");
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
		Gizmos.DrawWireSphere(Target.transform.position, ScaledAreaRadius);
	}
}