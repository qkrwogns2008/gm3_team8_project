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
			target.TakeDamage(CriticalDamage, this);

			BuffSystem.AddBuff
			(
			EBuffFlags.CriticalChanceBoost,
			24f,
			2f,
			this
			);
		}
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		base.ProcessSkillHit(target);
	}

}