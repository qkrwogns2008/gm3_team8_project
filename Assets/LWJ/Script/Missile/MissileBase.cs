using UnityEngine;

public class MissileBase : MonoBehaviour
{
	#region 내부 변수
	protected MissileBase OriginPrefab;
	
	protected CUnitBase Target;
	protected CUnitBase Attacker;

	protected float MoveSpeed;
	protected float Damage;
	#endregion

	private void OnDisable()
	{
		OriginPrefab = null;
		Target = null;
		Attacker = null;
		MoveSpeed = 0f;
		Damage = 0f;
	}

	public void Init(MissileBase origin, MissileDataSO data, float damage, CUnitBase target, CUnitBase attacker)
	{
		OriginPrefab = origin;

		MoveSpeed = data.MoveSpeed;
		Damage = damage;

		Target = target;
		Attacker = attacker;
	}

	private void Update()
	{
		MoveToTarget();
	}

	protected void MoveToTarget()
	{
		Vector2 targetPos = Target.transform.position;
		Vector2 pos = transform.position;
		float moveDelta = MoveSpeed * Time.deltaTime;

		transform.position = Vector2.MoveTowards(pos, targetPos, moveDelta);
		
		pos = transform.position;
		Vector2 toTarget = targetPos - pos;
		
		if (Vector2.SqrMagnitude(toTarget) <= 0.0001f)
		{
			OnArrivedTarget();
		}
	}

	protected void OnArrivedTarget()
	{
		if (Target != null)
		{
			Target.TakeDamage(Damage, Attacker);
		}
		ReturnToPool();
	}

	protected void ReturnToPool()
	{
		PoolManager.Instance.Push(OriginPrefab, this);
	}
}