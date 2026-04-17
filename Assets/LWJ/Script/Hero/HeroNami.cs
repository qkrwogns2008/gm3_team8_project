using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroNami : NoEffectHeroBase
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int CriticalAttackCount = 5;
	[SerializeField] protected List<float> MultiHitPredelayCritical = new List<float>() { 0.3f, 0.2f, 0.2f, 0.2f, 0.2f };

	[Header("스킬 속성값")]
	[SerializeField, Range(0f, 2f)] protected float SkillPreDelay = 0.3f;
	[SerializeField] protected bool PrintSkillLog = false;
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

		MotionRoutine = StartCoroutine(Co_PlayMotion(SkillAnimation, target, EAttackType.Skill));
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

				if (target != null && hitAudio != null)
				{
					SoundManager.Instance.PlayUnitSFX(hitAudio); // Hit 오디오 재생
				}
				ProcessHit(target, type);
			}
		}
		else if (type == EAttackType.Skill)
		{
			yield return new WaitForSeconds(SkillPreDelay / AttackSpeedMultiplier);

			if (target != null && hitAudio != null)
			{
				SoundManager.Instance.PlayUnitSFX(hitAudio); // Hit 오디오 재생
			}
			ProcessHit(target, type);
		}
		else
		{
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);

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

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(CriticalDamage / CriticalAttackCount, this);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		IReadOnlyList<CUnitBase> targetList = CHeroManager.Instance.ActiveHero;

		CUnitBase lowestUnit = FindLowestHPTarget(targetList);
		if (lowestUnit == null)
		{
			return;
		}
		CHero hero = lowestUnit as CHero;
		if (hero == null)
		{
			return;
		}

		hero.AddHPByRatio(0.1f);
		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 스킬 사용 대상 : [{hero.UnitName}]");
		}
	}

	/// <summary>
	/// targetList에서 가장 체력이 적은 Unit을 반환합니다. 반환 타입 CUnitBase.
	/// </summary>
	/// <returns>가장 체력이 적은 유닛</returns>
	protected virtual CUnitBase FindLowestHPTarget(IReadOnlyList<CUnitBase> targetList)
	{
		CUnitBase lowest = null;
		float minHPValue = Mathf.Infinity;

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

			float curHP = target.CurrnetHP;

			if (curHP < minHPValue)
			{
				lowest = target;
				minHPValue = curHP;
			}
		}

		if (lowest != null)
		{
			return lowest;
		}

		return null;
	}
}
