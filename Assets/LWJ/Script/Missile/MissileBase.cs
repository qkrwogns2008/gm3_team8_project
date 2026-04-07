using UnityEngine;

public class MissileBase : MonoBehaviour
{
	#region 내부 변수
	protected MissileBase OriginPrefab;
	
	protected CUnitBase Target;
	protected CUnitBase Attacker;

	protected float MoveSpeed;
	protected float Damage;

	protected bool LookAtTarget;
	#endregion

	protected virtual void OnDisable()
	{
		OriginPrefab = null;
		Target = null;
		Attacker = null;
		MoveSpeed = 0f;
		Damage = 0f;
	}

	public virtual void Init(MissileBase origin, MissileDataSO data, float damage, CUnitBase target, CUnitBase attacker)
	{
		OriginPrefab = origin;

		MoveSpeed = data.MoveSpeed;
		LookAtTarget = data.LookAtTarget;

		Damage = damage;

		Target = target;
		Attacker = attacker;
	}

	protected virtual void Update()
	{
		if (Target == null || Target.IsUnitDead)
		{
			ReturnToPool();
			return;
		}

		Vector2 pos = transform.position;
		Vector2 targetPos = Target.CenterPos;

		MoveToTarget(pos, targetPos);

		if (LookAtTarget)
		{
			RotateToTarget(pos, targetPos);
		}
	}

	protected virtual void RotateToTarget(Vector2 pos, Vector2 targetPos)
	{
		if (Target == null)
		{
			return;
		}
		
		Vector2 toTarget = targetPos - pos;

		float rotAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

		transform.rotation = Quaternion.Euler(0, 0, rotAngle);
	}

	protected virtual void MoveToTarget(Vector2 pos, Vector2 targetPos)
	{
		float moveDelta = MoveSpeed * Time.deltaTime;

		transform.position = Vector2.MoveTowards(pos, targetPos, moveDelta);
		
		// 거리 계산
		pos = transform.position;
		Vector2 toTarget = targetPos - pos;
		
		if (Vector2.SqrMagnitude(toTarget) <= 0.0001f)
		{
			OnArrivedTarget();
		}
	}

	protected virtual void OnArrivedTarget()
	{
		if (Target != null)
		{
			Target.TakeDamage(Damage, Attacker);
		}
		ReturnToPool();
	}

	protected virtual void ReturnToPool()
	{
		PoolManager.Instance.Push(OriginPrefab, this);
	}
}