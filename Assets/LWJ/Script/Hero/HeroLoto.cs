using Spine.Unity;
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
	[SerializeField] protected bool PrintSkillLog = false;
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
				continue;
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

	protected virtual IEnumerator Co_TeleportToTarget(CUnitBase target, EffectDataSO effectData)
	{
		yield return new WaitForSeconds(TeleportWaitTime);
		float offsetX = target.IsFacingRight ? -TeleportOffset : TeleportOffset;
		Vector3 pos = target.transform.position + new Vector3(offsetX, 0, 0);

		transform.position = pos;
		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 무적 {isInvincible}");
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

				ProcessTeleportHit(target);
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) 이펙트 null");
		}

		MotionRoutine = null;

		if (IsPendingDead)
		{
			DeathSequence();
		}
	}

	protected virtual void ProcessTeleportHit(CUnitBase target)
	{

	}
}
