using UnityEngine;

public class CHero : CUnitBase
{
	[SerializeField] protected float _effectDestroyTime = 0.5f;

	protected EffectBase _attack1Effect;

	protected override void Awake()
	{
		base.Awake();
		if (_attack1Prefab != null)
		{
			// 캐싱 시도
			if (!_attack1Prefab.TryGetComponent<EffectBase>(out _attack1Effect))
			{
				Debug.LogWarning($"{name} : 이펙트 캐싱 실패");
				return;
			}
		}
	}

	// for Test
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnAttack(_targetEnemy);
		}
	}

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
		if (_skeletonAni == null || _attack1Effect == null)
		{
			return;
		}
		_skeletonAni.AnimationState.SetAnimation(0, "Attack_A", false);

		// 이펙트 소환
		EffectBase fx = Instantiate(_attack1Effect, transform.position, Quaternion.identity);

		if (fx == null)
		{
			return;
		}
		fx.Init(false, 1f);

		if (target != null)
		{
			target.TakeDamage(_currentAtk, this);
		}
		Debug.Log($"{_unitName}의 일반 공격!");
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