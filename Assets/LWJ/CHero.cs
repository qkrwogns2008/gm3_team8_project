using UnityEngine;

public class CHero : CUnitBase
{
	protected override void FindClosesEnemy()
	{
		Collider[] enemies = Physics.OverlapSphere(transform.position, _detectionRange, _enemyLayer);

		if (enemies.Length > 0)
		{
			CUnitBase closest = null;
			float minDistance = Mathf.Infinity;

			foreach (Collider enemy in enemies)
			{
				float distance = Vector3.Distance(transform.position, enemy.transform.position);
				if (distance < minDistance)
				{
					CUnitBase targetUnit = enemy.GetComponent<CUnitBase>();
					if (targetUnit != null)
					{
						minDistance = distance;
						closest = targetUnit;
					}
				}
			}
			_targetEnemy = closest;
		}
		else
		{
			//Debug.Log("타겟 없음");
			_targetEnemy = null;
		}
	}

	protected override void OnAttack(CUnitBase target)
	{
		if (_skeletonAni == null || _attack1Prefab == null)
		{
			return;
		}
		Debug.Log($"{_unitName}의 일반 공격!");

		if (target != null)
		{
			target.TakeDamage(_currentAtk, this);
		}
	}

    protected override void OnSkill1(CUnitBase target)
    {
		if (_skeletonAni == null || _skill1Prefab == null)
		{
			return;
		}
		Debug.Log($"{_unitName}의 스킬 1 발동!");
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