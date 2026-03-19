using UnityEngine;

public class HeroTeo : CHero
{
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnAttack();
		}
	}

	protected override void OnAttack(CUnitBase target)
	{
		base.OnAttack(target);
		_skeletonAni.AnimationState.SetAnimation(0, "Attack_A", false);
		GameObject fx = Instantiate(_attack1Prefab, transform.position, Quaternion.identity);
	}

	protected override void OnSkill1(CUnitBase target)
	{
		base.OnSkill1(target);

	}

	protected override void OnSkill2(CUnitBase target)
	{
		base.OnSkill2(target);

	}
}