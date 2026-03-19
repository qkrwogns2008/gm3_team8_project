using UnityEngine;

public class CHero : CUnitBase
{
	// 이동은 부모가 Update에서 알아서 하니, 여기선 공격만 정의!
	
	protected override void OnAttack(CUnitBase target)
    {
		if (_skeletonAni == null || _attack1Prefab == null)
		{
			return;
		}
		Debug.Log($"{_unitName}의 일반 공격!");
        target.TakeDamage(_currentAtk, this);
    }

    protected override void OnSkill1(CUnitBase target)
    {
		if (_skeletonAni == null || _skill1Prefab == null)
		{
			return;
		}
		Debug.Log($"{_unitName}의 스킬 1 발동!");
        // 여기에 이펙트 생성이나 특수 로직 추가
    }

    protected override void OnSkill2(CUnitBase target)
    {
		if (_skeletonAni == null || _skill2Prefab == null)
		{
			return;
		}
		Debug.Log($"{_unitName}의 스킬 2 발동!");
    }
}