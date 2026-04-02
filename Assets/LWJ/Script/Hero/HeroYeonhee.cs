using UnityEngine;

public class HeroYeonhee : RangedHeroBase
{
	#region 인스펙터
	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectBase CriticalHitEffect;
	[SerializeField] protected EffectBase SkillHitEffect;
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

	protected virtual void SummonHitEffectOnTarget(CUnitBase target, EffectBase fx)
	{
		if (fx == null)
		{
			return;
		}

		TrySummonEffect(fx, EEffectDirection.None, target.transform.position);
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
		target.TakeDamage(FinalSkillDamage, this);
	}

	protected void OnDrawGizmosSelected()
	{

	}
}