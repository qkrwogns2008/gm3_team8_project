using System.Collections.Generic;
using UnityEngine;

public class HeroYeonhee : RangedHeroBase
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
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale; // 스킬 범위에 스파인 크기 반영
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (CriticalHitEffect == null ||
			SkillHitEffect == null)
		{
			Debug.LogWarning($"{name}) 인스펙터 null 감지.");
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

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, CriticalHitEffect);
		target.TakeDamage(CriticalDamage, this);
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, SkillHitEffect);
		CircleAreaAttack(target, ScaledAreaRadius);
	}

	/// <summary>
	/// 원 영역의 Enemy에게 피해를 줍니다. radius는 부채꼴의 반지름입니다.
	/// </summary>
	/// <param name="target">공격 매개 대상입니다. 해당 대상을 중심으로 범위 피해가 발생합니다.</param>
	/// <param name="radius">원 반지름</param>
	protected virtual void CircleAreaAttack(CUnitBase target, float radius)
	{
		Vector2 areaCenterPos = target.transform.position;
		float sqrRadius = radius * radius;

		IReadOnlyList<CUnitBase> enemies = CEnemyManager.Instance.ActiveEnemies;

		for (int i = 0; i < enemies.Count; i++)
		{
			CUnitBase enemy = enemies[i];
			
			if (enemy == null)
			{
				continue;
			}

			if (enemy.IsUnitDead)
			{
				continue;
			}

			Vector2 targetPos = enemy.transform.position;
			Vector2 toTarget = targetPos - areaCenterPos;

			if (toTarget.sqrMagnitude > sqrRadius)
			{
				continue;
			}

			enemy.TakeDamage(FinalSkillDamage, this);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}
	}

	protected void OnDrawGizmosSelected()
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