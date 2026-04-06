using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroNami : NoEffectHeroBase
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int CriticalAttackCount = 5;
	[SerializeField] protected List<float> MultiHitPredelayCritical = new List<float>() { 0.3f, 0.2f, 0.2f, 0.2f, 0.2f };
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

		if (type == EAttackType.Critical)
		{
			int count = CriticalAttackCount;
			List<float> predelay = MultiHitPredelayCritical;

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

				ProcessHit(target, type);
			}
		}
		else
		{
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);

			ProcessHit(target, type);
		}

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
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
