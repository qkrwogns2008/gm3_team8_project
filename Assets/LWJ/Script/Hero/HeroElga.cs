using System.Collections.Generic;
using UnityEngine;

public class HeroElga : RangedHeroBase
{
	#region 인스펙터
	[Header("치명타 원거리 세팅")]
	[SerializeField] protected MissileBase CriticalMissilePrefab;
	[SerializeField] protected MissileDataSO CriticalMissileData;

	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 3f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (CriticalMissilePrefab == null || CriticalMissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(CriticalMissilePrefab, CenterPos, rot);
		missile.Init(CriticalMissilePrefab, CriticalMissileData, CriticalDamage, target, this);
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, SkillHitEffect);

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		CircleAreaAttack(target, ScaledAreaRadius, targetList, FinalSkillDamage);
	}

	protected virtual void CircleAreaAttack(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList, float damage)
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

			target.TakeDamage(damage, this, false);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{damage}]");
		}
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