using System.Collections;
using UnityEngine;

public class RangedNoEffectHeroBase : RangedHeroBase
{
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation, target, EAttackType.Normal));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 일반 공격!");
		}
	}

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

		MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalAnimation, target, EAttackType.Critical));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 치명타 공격!");
		}
	}

	protected virtual IEnumerator Co_PlayMotion(string animationName, CUnitBase target, EAttackType type)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

		yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);

		ProcessHit(target, type);

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
	}
}
