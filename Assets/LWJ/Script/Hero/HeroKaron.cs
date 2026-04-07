using System.Collections.Generic;
using UnityEngine;

public class HeroKaron : RangedHeroBase
{
	#region ¿ŒΩ∫∆Â≈Õ
	[Header("≈∏∞› ¿Ã∆Â∆Æ(¿˚)")]
	[SerializeField] protected EffectDataSO CriticalHitEffect;
	#endregion

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
}