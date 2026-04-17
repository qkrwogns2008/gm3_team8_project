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

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation, target, EAttackType.Normal, AudioSO.Attack, AudioSO.AttackDamaged));
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
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);
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
	}
}
