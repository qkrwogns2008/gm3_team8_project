using System.Collections.Generic;
using UnityEngine;

public class HeroYeonhee : RangedHeroBase
{
	#region 인스펙터
	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("치명타 공격 속성값")]
	[SerializeField] protected float AdditionalTargetRadius = 4f; // 추가 타겟 탐색 범위
	[SerializeField] protected float AdditionalTargetDamageMultiplier = 0.5f; // 추가 타겟 피해 비율

	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 4f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	protected virtual float ScaledAdditionalTargetRadius => AdditionalTargetRadius * SpineScale;
	// 추가 타겟 최종 피해량
	protected virtual float FinalAdditionalTargetDamage => CriticalDamage * AdditionalTargetDamageMultiplier;
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

		// 추가 타겟 탐색
		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;

		CUnitBase additionalTarget = FindNearAdditionalTarget(target, ScaledAdditionalTargetRadius, targetList);

		if (additionalTarget != null)
		{
			SummonHitEffectOnTarget(additionalTarget, CriticalHitEffect);
			additionalTarget.TakeDamage(FinalAdditionalTargetDamage, this);

			if (PrintLog)
			{
				Debug.Log($"{UnitName}) 추가 타겟 공격. 대상 : [{additionalTarget.UnitName}]");
			}
		}
	}

	/// <summary>
	/// 현재 타겟 위치로부터 범위 내 가장 가까운 추가 타겟 1개체를 targetList 목록에서 탐색합니다. target은 탐색할 타겟, radius는 탐지 범위(반지름), targetList는 탐지할 타겟 목록입니다. CUnitBase를 반환합니다.
	/// </summary>
	/// <param name="originTarget">공격 매개 대상입니다. 해당 개체 위치를 기준으로 주변을 탐색합니다.</param>
	/// <param name="radius">탐지 범위(반지름)</param>
	/// <param name="targetList">타겟 목록</param>
	/// <returns>탐색된 객체</returns>
	protected virtual CUnitBase FindNearAdditionalTarget(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList)
	{
		Vector2 findCenterPos = originTarget.transform.position;
		float sqrRadius = radius * radius;

		CUnitBase nearest = null;
		float maxSqrDistance = Mathf.NegativeInfinity;

		for (int i = 0; i < targetList.Count; i++)
		{
			CUnitBase target = targetList[i];

			if (target == null)
			{
				continue;
			}
			if (target == originTarget)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			// 사거리 체크
			Vector2 targetPos = target.transform.position;
			Vector2 toTarget = targetPos - findCenterPos;
			float sqrDistance = toTarget.sqrMagnitude;

			if (sqrDistance > sqrRadius)
			{
				continue;
			}

			if (sqrDistance > maxSqrDistance)
			{
				nearest = target;
				maxSqrDistance = sqrDistance;
			}
		}

		if (nearest != null)
		{
			return nearest;
		}

		return null;
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, SkillHitEffect);

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		CircleAreaAttack(target, ScaledAreaRadius, targetList);
	}

	/// <summary>
	/// originTarget 주변 원 영역에 있는 모든 targetList 목록 대상에게 피해를 줍니다. radius는 반지름, targetList는 탐지할 타겟 목록입니다.
	/// </summary>
	/// <param name="originTarget">공격 매개 대상입니다. 해당 대상을 중심으로 범위 피해가 발생합니다.</param>
	/// <param name="radius">원 반지름</param>
	/// <param name="targetList">타겟 목록</param>
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