using UnityEngine;

public class RangedHeroBase : CHero
{
	#region 인스펙터
	[Header("원거리 세팅")]
	[SerializeField] protected MissileBase MissilePrefab;
	[SerializeField] protected MissileDataSO MissileData;
	#endregion

	protected override void ProcessNormalHit(CUnitBase target)
	{
		if (MissilePrefab == null || MissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Vector2 pos = transform.position;

		MissileBase missile = PoolManager.Instance.Pop(MissilePrefab, pos, Quaternion.identity);
		missile.Init(MissilePrefab, MissileData, FinalAttackDamage, target, this);
	}
}