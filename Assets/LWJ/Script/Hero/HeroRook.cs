using UnityEngine;

public class HeroRook : CHero
{
	#region
	[Header("스킬 속성값")]
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	protected override void ProcessSkillHit(CUnitBase target)
	{
		BuffSystem.AddBuff(EBuffFlags.StackGuard, 6f, 10f, this);

		if (PrintSkillLog)
		{
			Debug.Log($"[{UnitName}] 받는 피해 무효화 버프 발동.");
		}
	}
}
