using UnityEngine;

public class RangedHeroBase : CHero
{
	#region 인스펙터
	[Header("원거리 세팅")]
	[SerializeField] protected MissileBase MissilePrefab;
	[SerializeField] protected MissileDataSO MissileData;

	[Header("투사체 발사 오프셋")]
	[SerializeField] protected Vector2 FirePosOffset = new Vector2(0, 0.5f);
	#endregion

	protected virtual Vector2 CenterPos => (Vector2)transform.position + FirePosOffset * SpineScale;

	protected override void ProcessNormalHit(CUnitBase target)
	{
		if (MissilePrefab == null || MissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		MissileBase missile = PoolManager.Instance.Pop(MissilePrefab, CenterPos, Quaternion.identity);
		missile.Init(MissilePrefab, MissileData, FinalNormalAttackDamage, target, this);
	}
}