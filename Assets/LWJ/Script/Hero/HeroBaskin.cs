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

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(MissilePrefab, CenterPos, rot);
		missile.Init(MissilePrefab, MissileData, CriticalDamage, target, this);
	}
}