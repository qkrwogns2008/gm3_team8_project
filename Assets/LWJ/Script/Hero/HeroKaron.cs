using System.Collections.Generic;
using UnityEngine;

public class HeroKaron : RangedHeroBase
{
	#region 인스펙터
	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, CriticalHitEffect);
		target.TakeDamage(CriticalDamage, this, false);
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

		hero.AddHPByRatio(0.3f);
		hero.BuffSystem.AddBuff(EBuffFlags.DefenseBoost, 0.15f, 10f, this);
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