using System.Collections.Generic;
using UnityEngine;

public class HeroRadgrid : CHero
{
	#region 인스펙터
	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 4f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();

	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, CriticalHitEffect);
		target.TakeDamage(CriticalDamage, this);

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

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		CircleAreaAttack(target, ScaledAreaRadius, targetList);
	}

	protected virtual void CircleAreaAttack(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList)
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

			target.TakeDamage(FinalSkillDamage, this);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}
	}

	protected virtual void SummonHitEffectOnTarget(CUnitBase target, EffectDataSO fxData)
	{
		if (fxData == null)
		{
			return;
		}
		if (fxData.Catalog == null ||
			fxData.Catalog.Count == 0)
		{
			return;
		}
		if (fxData.Catalog[0] == null)
		{
			return;
		}

		TrySummonEffect(fxData.Catalog[0], target.transform.position);
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