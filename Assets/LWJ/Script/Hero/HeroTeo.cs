using System.Collections.Generic;
using UnityEngine;

public class HeroTeo : CHero
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected float SectorRadius = 5f;
	[SerializeField] protected float SectorDegree = 30f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected virtual float ScaledSectorRadius => SectorRadius * SpineScale; // 스킬 범위에 스파인 크기 반영
	#endregion

	protected override void ProcessSkillHit(CUnitBase target)
	{
		SectorAreaAttack(target, SectorDegree, ScaledSectorRadius);
	}

	/// <summary>
	/// 부채꼴 영역의 Enemy에게 피해를 줍니다. degree는 부채꼴의 각도, radius는 부채꼴의 반지름입니다.
	/// </summary>
	/// <param name="target">공격 매개 대상입니다. 범위에 상관없이 항상 피해를 입습니다.</param>
	/// <param name="degree">부채꼴 각도</param>
	/// <param name="radius">부채꼴 반지름</param>
	protected virtual void SectorAreaAttack(CUnitBase target, float degree, float radius)
	{
		float sectorHalfDegree = degree * 0.5f; // (정면, 좌측)과 (정면, 우측)의 내적(코사인) 값 같음.
		float cosSectorDegree = Mathf.Cos(sectorHalfDegree * Mathf.Deg2Rad);

		float sqrSectorRadius = radius * radius;

		Vector2 forward = IsFacingRight ? Vector2.right : Vector2.left;
		Vector2 pos = transform.position;

		IReadOnlyList<CUnitBase> enemies = CEnemyManager.Instance.ActiveEnemies;

		for (int i = 0; i < enemies.Count; i++)
		{
			CUnitBase enemy = enemies[i];

			if (enemy == null)
			{
				continue;
			}

			if (enemy == target)
			{
				continue; // target에 대한 피해는 후처리
			}

			if (enemy.IsUnitDead)
			{
				continue;
			}

			Vector2 targetPos = enemy.transform.position;
			Vector2 toTarget = targetPos - pos;

			// 사거리 체크
			if (Vector2.SqrMagnitude(toTarget) > sqrSectorRadius)
			{
				continue;
			}

			// 각도 체크
			toTarget = toTarget.normalized;
			
			float cosAngle = Vector2.Dot(forward, toTarget);

			// (cos범위 > 타겟과의 내적 값) → 부채꼴 바깥
			if (cosSectorDegree > cosAngle)
			{
				continue;
			}
			
			enemy.TakeDamage(FinalSkillDamage, this);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"부채꼴 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}

		// 부채꼴 바깥이어도 타겟은 무조건 피해를 입도록 보장
		if (target != null)
		{
			target.TakeDamage(FinalSkillDamage, this);
		}
	}

	protected void OnDrawGizmosSelected()
	{
		if (SkeletonAni.skeleton == null)
		{
			return;
		}

		float localScale = SkeletonAni.gameObject.transform.localScale.x;

		Gizmos.color = Color.yellow;
		Vector2 forward = IsFacingRight ? Vector2.right : Vector2.left;
		Vector2 left = Quaternion.Euler(0, 0, -SectorDegree * 0.5f) * forward;
		Vector2 right = Quaternion.Euler(0, 0, SectorDegree * 0.5f) * forward;

		Vector2 pos = transform.position;

		Gizmos.DrawLine(pos, pos + left * ScaledSectorRadius);
		Gizmos.DrawLine(pos, pos + right * ScaledSectorRadius);
	}
}