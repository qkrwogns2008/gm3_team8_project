using System.Collections.Generic;
using UnityEngine;

public class HeroEvan : CHero
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected float AreaRadius = 4f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();

	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	/// <summary>
	/// ratio 비율 만큼 체력을 회복합니다. (1 = 100%)
	/// </summary>
	protected virtual void AddHPByRatio(float ratio)
	{
		float amount = FinalMaxHP * ratio;
		CurrentHp = Mathf.Min(CurrentHp + amount, FinalMaxHP);

		NotifyHpChange();

		if (PrintLog)
		{
			Debug.Log($"[{UnitName}] 체력 회복 : {amount}. 현재 체력 : {CurrentHp}");
		}
	}

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target != null)
		{
			target.TakeDamage(CriticalDamage, this);
			AddHPByRatio(0.08f);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		FindedTargets.Clear();

		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		FindTargetsOnCircleArea(this, ScaledAreaRadius, targetList);

		SelectedTargetsAttack(FindedTargets, FinalSkillDamage);

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

	protected virtual void SelectedTargetsAttack(IReadOnlyList<CUnitBase> targets, float damage)
	{
		// 선택된 타겟 목록 순회
		for (int i = 0; i < targets.Count; i++)
		{
			CUnitBase target = targets[i];

			if (target == null)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			target.TakeDamage(damage, this);
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
		Gizmos.DrawWireSphere(transform.position, ScaledAreaRadius);
	}
}