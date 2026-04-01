using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroYeonhee : RangedHeroBase
{
	protected override void ProcessSkillHit(CUnitBase target, CUnitBase attacker)
	{
		SectorAttack(target, attacker);
	}

	protected virtual void SectorAttack(CUnitBase target, CUnitBase attacker)
	{

	}

	protected void OnDrawGizmosSelected()
	{

	}
}
