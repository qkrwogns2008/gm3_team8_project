using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRadgrid : CHero
{
	#region 인스펙터
	[Header("Multi Attack 설정")]
	[SerializeField] protected int SkillAttackCount = 5;
	[SerializeField] protected List<float> MultiHitPredelay = new List<float>() { 0.6f, 0.2f, 0.1f, 0.1f, 0.1f };

	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 6f;

	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected Coroutine SkillRoutine;
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();

	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, CriticalHitEffect);
		target.TakeDamage(CriticalDamage, this, false);

		BuffSystem.AddBuff
			(
			EBuffFlags.CriticalChanceBoost,
			24f,
			2f,
			this
			);
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}


		BuffSystem.AddBuff
			(
			EBuffFlags.StackGuard,
			10f,
			10f,
			this
			);

		SummonHitEffectOnTarget(target, SkillHitEffect);

		FindedTargets.Clear();
		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		FindTargetsOnCircleArea(target, ScaledAreaRadius, targetList);

		SkillRoutine = StartCoroutine(Co_SelectedTargetsAttack(FindedTargets, FinalSkillDamage / SkillAttackCount));

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}
	}

	// 주변 타겟 탐색
	protected virtual void FindTargetsOnCircleArea(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList)
	{
		Vector2 areaCenterPos = originTarget.transform.position;
		float sqrRadius = radius * radius;

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

			Vector2 targetPos = target.transform.position;
			Vector2 toTarget = targetPos - areaCenterPos;

			if (toTarget.sqrMagnitude > sqrRadius)
			{
				continue;
			}
			FindedTargets.Add(target);
		}
	}

	// 목록의 모든 타겟 공격
	protected virtual IEnumerator Co_SelectedTargetsAttack(IReadOnlyList<CUnitBase> targets, float damage)
	{
		if (SkillAttackCount != MultiHitPredelay.Count)
		{
			Debug.LogWarning($"{unitName}) 공격 횟수와 딜레이 수 불일치.");
			SkillRoutine = null;
			yield break;
		}

		// Multi Attack
		for (int i = 0; i < MultiHitPredelay.Count; i++)
		{
			bool IsHit = false;
			float preDelay = MultiHitPredelay[i];

			yield return new WaitForSeconds(preDelay / AttackSpeedMultiplier);

			// 선택된 타겟 목록 순회
			for (int j = 0; j < targets.Count; j++)
			{
				CUnitBase target = targets[j];

				if (target == null)
				{
					continue;
				}
				if (target.IsUnitDead)
				{
					continue;
				}

				target.TakeDamage(damage, this, false);
				IsHit = true;
			}

			if (IsHit) // 타격에 성공했으면
			{
				if (AudioSO.SkillDamaged != null)
				{
					SoundManager.Instance.PlayUnitSFX(AudioSO.SkillDamaged); // 공격 오디오 재생
				}
			}
		}

		SkillRoutine = null;
	}
	
	protected virtual void OnDrawGizmosSelected()
	{
		if (Target == null)
		{
			return;
		}
		if (Target.IsUnitDead)
		{
			return;
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Target.transform.position, ScaledAreaRadius);
	}
}