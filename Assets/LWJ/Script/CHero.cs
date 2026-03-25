using System.Collections;
using UnityEngine;

public class CHero : CUnitBase
{
	#region 인스펙터
	[Header("이펙트 데이터")]
	[SerializeField] protected EffectDataSO _attack1Data;
	[SerializeField] protected EffectDataSO _critical1Data;
	[SerializeField] protected EffectDataSO _skill1Data;
	[SerializeField] protected EffectDataSO _skill2Data;

	[Header("치명타")]
	[SerializeField] protected float _criticalChance = 20f;
	[SerializeField] protected float _criticalAttackMultiplier = 2f;
	#endregion

	#region 내부 변수
	protected Coroutine _motionRoutine;
	protected float CriticalDamage => _currentAtk * _criticalAttackMultiplier;
	#endregion

	// for Test
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnAttack(_targetEnemy);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			OnCritical(_targetEnemy);
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
		if (_skeletonAni == null || _attack1Data == null)
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		// 치명타 체크
		bool isCriAttack = (Random.Range(0f, 100f) <= _criticalChance);
		Debug.Log($"크리티컬 : {isCriAttack}");

		if (isCriAttack && _critical1Data != null)
		{
			_motionRoutine = StartCoroutine(Co_PlayMotion(_critical1Data, "Critical", target, CriticalDamage));
			Debug.Log($"{_unitName}의 치명타 공격!");
		}
		else
		{
			_motionRoutine = StartCoroutine(Co_PlayMotion(_attack1Data, "Attack_A", target, _currentAtk));
			Debug.Log($"{_unitName}의 일반 공격!");
		}
	}

	// for test
	private void OnCritical(CUnitBase target)
	{
		if (_skeletonAni == null || _attack1Data == null)
		{
			return;
		}

		if (_motionRoutine != null)
		{
			return;
		}

		_motionRoutine = StartCoroutine(Co_PlayMotion(_critical1Data, "Critical", target, CriticalDamage));
		Debug.Log($"{_unitName}의 치명타 공격!");
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

	/// <summary>
	/// 스파인 애니메이션을 재생하고, effectData의 PreDelay 값에 따라 시간차로 이펙트를 생성합니다. 성공적으로 종료되면 피해를 적용합니다.
	/// </summary>
	protected virtual IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, float damage)
	{
		_skeletonAni.AnimationState.SetAnimation(0, animationName, false);
		_skeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

		yield return new WaitForSeconds(effectData.PreDelay);

		// 이펙트 생성 실패 시 즉시 종료
		if (!TrySummonEffect(effectData))
		{
			_motionRoutine = null;
			yield break;
		}

		if (target != null)
		{
			target.TakeDamage(damage, this);
		}

		_motionRoutine = null;
	}

	// 이펙트 소환 시도
	protected virtual bool TrySummonEffect(EffectDataSO effectData)
	{
		Vector3 pos = transform.position + effectData.Offset;
		Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
		EffectBase fx = Instantiate(effectData.Prefab, pos, rot);

		if (fx == null)
		{
			Debug.LogWarning($"{name} : {effectData.Name} 이펙트 생성 실패");
			return false;
		}
		fx.Init(false);

		return true;
	}
}