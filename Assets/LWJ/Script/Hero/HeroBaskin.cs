using UnityEngine;

public class HeroBaskin : RangedNoEffectHeroBase
{
	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (MissilePrefab == null || MissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		MissileBase missile = PoolManager.Instance.Pop(MissilePrefab, CenterPos, Quaternion.identity);
		missile.Init(MissilePrefab, MissileData, CriticalDamage, target, this);
	}
}