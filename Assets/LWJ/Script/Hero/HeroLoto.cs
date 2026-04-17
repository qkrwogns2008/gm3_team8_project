using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLoto : NoEffectHeroBase
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int AttackCount = 2;
	[SerializeField] protected List<float> MultiHitPredelay = new List<float>() { 0.3f, 0.3f };

	[SerializeField] protected int CriticalAttackCount = 2;
	[SerializeField] protected List<float> MultiHitPredelayCritical = new List<float>() { 0.3f, 0.4f };

	[Header("스킬 속성값")]
	[SerializeField] protected float TeleportOffset = 5.0f;
	[SerializeField] protected float TeleportWaitTime = 0.5f;
	#endregion

	protected override void OnSkill(CUnitBase target)
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(SkillAnimation, target, EAttackType.Skill, AudioSO.Skill, AudioSO.SkillDamaged));
		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 스킬 발동!");
		}
	}

	// multi attack
	protected override IEnumerator Co_PlayMotion(string animationName, CUnitBase target, EAttackType type, AudioClip castAudio = null, AudioClip hitAudio = null)
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

		int count = (type == EAttackType.Normal) ? AttackCount : CriticalAttackCount;
		List<float> predelay = (type == EAttackType.Normal) ? MultiHitPredelay : MultiHitPredelayCritical;

		if (count != predelay.Count)
		{
			Debug.LogWarning($"{unitName}) 공격 횟수와 딜레이 수 불일치.");
			MotionRoutine = null;
			yield break;
		}

		for (int i = 0; i < count; i++)
		{
			yield return new WaitForSeconds(predelay[i] / AttackSpeedMultiplier);

			if (target.IsUnitDead)
			{
				continue;
			}

			if (target != null && hitAudio != null)
			{
				SoundManager.Instance.PlayUnitSFX(hitAudio); // Hit 오디오 재생
			}
			ProcessHit(target, type);
		}

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
	}

	protected override void ProcessNormalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(FinalNormalAttackDamage / AttackCount, this);
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

		MotionRoutine = StartCoroutine(Co_TeleportToTarget(target));
	}

	protected virtual IEnumerator Co_TeleportToTarget(CUnitBase target)
	{
		yield return new WaitForSeconds(TeleportWaitTime);
		float offsetX = IsFacingRight ? -TeleportOffset : TeleportOffset;
		Vector3 pos = target.transform.position + new Vector3(offsetX, 0, 0);

		transform.position = pos;
		
		ProcessTeleportHit(target);

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
	}

	protected virtual void ProcessTeleportHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(FinalSkillDamage, this);
		}
	}
}
