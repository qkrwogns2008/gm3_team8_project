using UnityEngine;

public class HeroElga : RangedHeroBase
{
	#region 인스펙터
	[Header("치명타 원거리 세팅")]
	[SerializeField] protected MissileBase CriticalMissilePrefab;
	[SerializeField] protected MissileDataSO CriticalMissileData;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (CriticalMissilePrefab == null || CriticalMissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(CriticalMissilePrefab, CenterPos, rot);
		missile.Init(CriticalMissilePrefab, CriticalMissileData, CriticalDamage, target, this);
	}
}