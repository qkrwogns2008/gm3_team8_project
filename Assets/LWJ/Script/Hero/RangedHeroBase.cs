using UnityEngine;

/// <summary>
/// 원거리형 영웅의 베이스 클래스
/// </summary>
public class RangedHeroBase : CHero
{
	#region 인스펙터
	[Header("원거리 세팅")]
	[SerializeField] protected MissileBase MissilePrefab;
	[SerializeField] protected MissileDataSO MissileData;
	[SerializeField] protected AudioClip MissileDamaged;
	#endregion

	protected override void ProcessNormalHit(CUnitBase target)
	{
		if (MissilePrefab == null || MissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(MissilePrefab, CenterPos, rot);
		missile.Init(MissilePrefab, MissileData, FinalNormalAttackDamage, target, this, MissileDamaged);
	}
}