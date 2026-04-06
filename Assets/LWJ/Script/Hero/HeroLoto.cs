using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLoto : NoEffectHero
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int AttackCount = 2;
	[SerializeField] protected List<float> MultiHitPredelay = new List<float>() { 0.3f, 0.3f };

	[SerializeField] protected int CriticalAttackCount = 2;
	[SerializeField] protected List<float> MultiHitPredelayCritical = new List<float>() { 0.3f, 0.4f };
	#endregion

	// multi attack
	protected override IEnumerator Co_PlayMotion(string animationName, CUnitBase target, EAttackType type)
	{
		if (string.IsNullOrEmpty(animationName))
		{
			Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
			MotionRoutine = null;
			yield break;
		}

		SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

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
				break;
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
}
