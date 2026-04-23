using UnityEngine;

public class MissileBase : MonoBehaviour
{
	#region 내부 변수
	protected MissileBase OriginPrefab;
	
	protected CUnitBase Target;
	protected CUnitBase Attacker;

	protected float MoveSpeed;
	protected float Damage;
	protected AudioClip HitAudio;

	protected bool LookAtTarget;

	protected Vector3 Rot;
	protected Vector2 TargetPos;
	#endregion

	protected virtual void OnEnable()
	{
		Rot = transform.rotation.eulerAngles;
	}

	protected virtual void OnDisable()
	{
		OriginPrefab = null;
		Target = null;
		Attacker = null;
		MoveSpeed = 0f;
		Damage = 0f;
		HitAudio = null;
		Rot = Vector3.zero;
		TargetPos = Vector2.zero;
	}

	public virtual void Init(MissileBase origin, MissileDataSO data, float damage, CUnitBase target, CUnitBase attacker, AudioClip hitAudio = null)
	{
		OriginPrefab = origin;

		MoveSpeed = data.MoveSpeed;
		LookAtTarget = data.LookAtTarget;

		Damage = damage;
		HitAudio = hitAudio;

		Target = target;
		TargetPos = target != null ? target.CenterPos : transform.position;

		Attacker = attacker;
	}

	protected virtual void Update()
	{
		/*
		if (Target == null || Target.IsUnitDead
			//|| !Target.gameObject.activeSelf
			)
		{
			ReturnToPool();
			return;
		}
		*/

		Vector2 pos = transform.position;
		if (Target != null)
		{
			TargetPos = Target.CenterPos;
		}

		MoveToTarget(pos, TargetPos);

		if (LookAtTarget)
		{
			RotateToTarget(pos, TargetPos);
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

		Rot.z = rotAngle;
		transform.rotation = Quaternion.Euler(Rot);
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
			if (HitAudio != null)
			{
				SoundManager.Instance.PlayUnitSFX(HitAudio); // Hit 오디오 재생
			}
			Target.TakeDamage(Damage, Attacker);
		}
		ReturnToPool();
	}

	protected virtual void ReturnToPool()
	{
		PoolManager.Instance.Push(OriginPrefab, this);
	}
}