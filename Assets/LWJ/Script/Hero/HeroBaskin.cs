using UnityEngine;

public class HeroBaskin : RangedNoEffectHeroBase
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

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

	protected override void ProcessSkillHit(CUnitBase target)
	{
		BuffSystem.AddBuff(EBuffFlags.AttackDamageBoost, 0.05f, 10f, this);
		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 공격력 증가 버프 부여.");
		}
	}
}