using System.Collections;
using UnityEngine;

/// <summary>
/// 공격 이펙트가 없는 영웅의 베이스 클래스
/// </summary>
public class NoEffectHeroBase : CHero
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation, target, EAttackType.Normal, AudioSO.Attack));
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalAnimation, target, EAttackType.Critical, AudioSO.Critical));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 치명타 공격!");
		}
	}

	protected virtual IEnumerator Co_PlayMotion(string animationName, CUnitBase target, EAttackType type, AudioClip castAudio = null)
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
}
