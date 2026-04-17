using System.Collections;
using UnityEngine;

public class HeroShane : CHero
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int CriticalAttackCount = 2;
	[SerializeField] protected int SkillAttackCount = 3;
	#endregion

	protected override IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, EAttackType type, AudioClip castAudio = null, AudioClip hitAudio = null)
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

				if (target != null && hitAudio != null)
				{
					SoundManager.Instance.PlayUnitSFX(hitAudio); // Hit 오디오 재생
				}
				ProcessHit(target, type);
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) 이펙트 null");
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
		if (target != null)
		{
			target.TakeDamage(CriticalDamage / CriticalAttackCount, this);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(FinalSkillDamage / SkillAttackCount, this);
		}
	}
}