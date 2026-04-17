using System.Collections;
using UnityEngine;

public class HeroRook : CHero
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected float TeleportOffset = 5.0f;
	[SerializeField] protected float TeleportWaitTime = 0.5f;
	[SerializeField] protected float KnockbackPower = 1f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledKnockbackPower => KnockbackPower * SpineScale;
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

				// 치명타 전용 추가 로직
				if (type == EAttackType.Critical &&
					i == 1)
				{
					TeleportToTarget(target);
				}

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
			}
		}
		else
		{
			Debug.LogWarning($"{UnitName}) effectData null");
			yield return new WaitForSeconds(0.3f / AttackSpeedMultiplier);
		}

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

	protected virtual void TeleportToTarget(CUnitBase target)
	{
		float offsetX = IsFacingRight ? -TeleportOffset : TeleportOffset;

		Vector3 pos = target.transform.position + new Vector3(offsetX, 0, 0);
		transform.position = pos;
	}

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			float knockbackValue = IsFacingRight ? -ScaledKnockbackPower : ScaledKnockbackPower;
			target.OnKnockbackX(knockbackValue);

			target.TakeDamage(FinalSkillDamage, this);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		BuffSystem.AddBuff(EBuffFlags.StackGuard, 6f, 10f, this);

		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 받는 피해 무효화 버프 발동.");
		}
	}
}
