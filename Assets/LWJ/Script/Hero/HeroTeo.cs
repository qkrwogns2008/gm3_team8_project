using System.Collections.Generic;
using UnityEngine;

public class HeroTeo : CHero
{
	#region 인스펙터
	[Header("스킬 속성값")]
	[SerializeField] protected float SectorRadius = 5f;
	[SerializeField] protected float SectorWidth = 30f;
	[SerializeField] protected bool PrintSkillLog = false;
	#endregion

	#region 내부 변수
	protected float SpineScale => SkeletonAni.gameObject.transform.localScale.x;
	protected float ScaledSectorRadius => SectorRadius * SpineScale; // 스킬 범위에 스파인 크기 반영
	#endregion

	protected override void ProcessSkillHit(CUnitBase target, CUnitBase attacker)
	{
		SectorAttack(target, attacker);
	}

	protected virtual void SectorAttack(CUnitBase target, CUnitBase attacker)
	{
		float SectorHalfWidth = SectorWidth * 0.5f; // (정면, 좌측)과 (정면, 우측)의 내적(코사인) 값 같음.
		float cosSectorWidth = Mathf.Cos(SectorHalfWidth * Mathf.Deg2Rad);

		float sqrSectorRadius = ScaledSectorRadius * ScaledSectorRadius;

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

			if (target == enemy)
			{
				continue; // target에 대한 피해는 후처리
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
			if (cosSectorWidth > cosAngle)
			{
				continue;
			}

			
			enemy.TakeDamage(FinalSkillDamage, attacker);
		}

		if (PrintSkillLog)
		{
			Debug.Log($"부채꼴 범위 피해 발생. 피해량 : [{FinalSkillDamage}]");
		}

		// 부채꼴 바깥이어도 타겟은 무조건 피해를 입도록 보장
		if (target != null)
		{
			target.TakeDamage(FinalSkillDamage, attacker);
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
		Vector2 left = Quaternion.Euler(0, 0, -SectorWidth * 0.5f) * forward;
		Vector2 right = Quaternion.Euler(0, 0, SectorWidth * 0.5f) * forward;

		Vector2 pos = transform.position;

		Gizmos.DrawLine(pos, pos + left * ScaledSectorRadius);
		Gizmos.DrawLine(pos, pos + right * ScaledSectorRadius);
	}
}