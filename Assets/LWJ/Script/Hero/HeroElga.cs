using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroElga : RangedHeroBase
{
	#region 인스펙터
	[Header("치명타 원거리 세팅")]
	[SerializeField] protected MissileBase CriticalMissilePrefab;
	[SerializeField] protected MissileDataSO CriticalMissileData;
	[SerializeField] protected AudioClip CriticalMissileDamaged;

	[Header("타격 이펙트(적)")]
	[SerializeField] protected EffectDataSO SkillHitEffect;

	[Header("스킬 속성값")]
	[SerializeField] protected float SkillDropPreDelay = 0.5f;
	[SerializeField] protected float AreaRadius = 3f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected Coroutine SkillRoutine;
	protected readonly List<CUnitBase> FindedTargets = new List<CUnitBase>();
	// 스킬 범위에 스파인 크기 반영
	protected virtual float ScaledAreaRadius => AreaRadius * SpineScale;
	#endregion

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (CriticalMissilePrefab == null || CriticalMissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(CriticalMissilePrefab, CenterPos, rot);
		missile.Init(CriticalMissilePrefab, CriticalMissileData, CriticalDamage, target, this, CriticalMissileDamaged);
	}

	protected override void ProcessSkillHit(CUnitBase target)
	{
		if (target == null)
		{
			return;
		}

		SummonHitEffectOnTarget(target, SkillHitEffect);

		FindedTargets.Clear();
		IReadOnlyList<CUnitBase> targetList = CEnemyManager.Instance.ActiveEnemies;
		FindTargetsOnCircleArea(target, ScaledAreaRadius, targetList);

		SkillRoutine = StartCoroutine(Co_SelectedTargetsAttack(FindedTargets, FinalSkillDamage));

		if (PrintSkillLog)
		{
			Debug.Log($"원형 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}
	}

	// 주변 타겟 탐색
	protected virtual void FindTargetsOnCircleArea(CUnitBase originTarget, float radius, IReadOnlyList<CUnitBase> targetList)
	{
		Vector2 areaCenterPos = originTarget.transform.position;
		float sqrRadius = radius * radius;

		for (int i = 0; i < targetList.Count; i++)
		{
			CUnitBase target = targetList[i];

			if (target == null)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			Vector2 targetPos = target.transform.position;
			Vector2 toTarget = targetPos - areaCenterPos;

			if (toTarget.sqrMagnitude > sqrRadius)
			{
				continue;
			}
			FindedTargets.Add(target);
		}
	}

	// 목록의 모든 타겟 공격
	protected virtual IEnumerator Co_SelectedTargetsAttack(IReadOnlyList<CUnitBase> targets, float damage)
	{
		yield return new WaitForSeconds(SkillDropPreDelay / AttackSpeedMultiplier);

		// 선택된 타겟 목록 순회
		for (int j = 0; j < targets.Count; j++)
		{
			CUnitBase target = targets[j];

			if (target == null)
			{
				continue;
			}
			if (target.IsUnitDead)
			{
				continue;
			}

			target.TakeDamage(damage, this, false);
		}

		if (AudioSO != null && AudioSO.SkillDamaged)
		{
			SoundManager.Instance.PlayUnitSFX(AudioSO.SkillDamaged);
		}

		SkillRoutine = null;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if (Target == null)
		{
			return;
		}
		if (Target.IsUnitDead)
		{
			return;
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Target.transform.position, ScaledAreaRadius);
	}
}