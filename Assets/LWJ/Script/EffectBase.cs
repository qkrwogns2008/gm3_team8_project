using System.Collections;
using UnityEngine;

public enum EEffectDirection
{
	None, // 무방향
	Left,
	Right,
}

/// <summary>
/// 유닛이 공격할 때 출력되는 이펙트 처리 로직을 담당합니다.
/// </summary>
public class EffectBase : MonoBehaviour
{
	// 외부에서 이펙트 프리팹 생성 → 초기화 값 주입(방향, 비활성 예약)
	#region 인스펙터
	[Header("Effect 방향")]
	[SerializeField] protected GameObject NoDirectionEffect;
	[SerializeField] protected GameObject LeftEffect;
	[SerializeField] protected GameObject RightEffect;

	[Header("Effect Life")]
	[SerializeField] protected float EffectLifeTime = 2f;

	[Header("Praticle Scale Multiplier")]
	[SerializeField] protected bool EnableParticleBaseScaleMultiplier = true;
	[SerializeField] protected float BaseScaleMultiplier = 1.0f; // 오직 초기화에서만 사용
	#endregion

	#region 내부 변수
	protected EffectBase OriginPrefab;
	protected Vector3 BaseScale;
	protected Coroutine Routine;
	#endregion

	protected virtual void Awake()
	{
		if (NoDirectionEffect == null &&
			(LeftEffect == null || RightEffect == null))
		{
			Debug.LogWarning($"{name} : 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
		if (EffectLifeTime <= 0)
		{
			Debug.LogWarning($"{name} : EffectLifeTime이 0보다 커야 합니다.");
			gameObject.SetActive(false);
			return;
		}

		DisableAllChildEffect();
		BaseScale = transform.localScale;

		if (EnableParticleBaseScaleMultiplier)
		{
			SetEffectScale(BaseScaleMultiplier);
		}
	}

	// 모든 자식 비활성화
	protected virtual void DisableAllChildEffect()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Effect의 방향, 크기를 설정합니다.
	/// </summary>
	/// <param name="isFacingRight">방향</param>
	/// <param name="scale">크기 (기본 1.0f)</param>
	public virtual void Init(EffectBase prefab , EEffectDirection direction, float scale = 1.0f)
	{
		OriginPrefab = prefab;

		DisableAfterTime(EffectLifeTime);
		if (!EnableParticleBaseScaleMultiplier)
		{
			SetEffectScale(scale);
        }
        SetEffectDirection(direction);
    }

	protected virtual void SetEffectDirection(EEffectDirection direction)
	{
		if (direction == EEffectDirection.None)
		{
			NoDirectionEffect.SetActive(true);
			return;
		}

		if (LeftEffect == null || RightEffect == null)
		{
			Debug.LogWarning($"{name} : 인스펙터 null. EffectDataSO 방향 유무 확인");
			return;
		}

		// 우측 방향 이펙트 출력
		if (direction == EEffectDirection.Right)
		{
			RightEffect.SetActive(true);
		}
		// 좌측 방향 이펙트 출력
		else
		{
			LeftEffect.SetActive(true);
		}
	}

	/// <summary>
	/// Effect 크기를 설정합니다.
	/// </summary>
	/// <param name="scale"></param>
	public virtual void SetEffectScale(float scale)
	{
		Vector3 nextScale = transform.localScale;
		nextScale = BaseScale * scale;
		transform.localScale = nextScale;
	}

	/// <summary>
	/// time 뒤에 해당 오브젝트를 비활성화하고 EffectPool로 복귀시킵니다.
	/// </summary>
	/// <param name="time"></param>
	protected virtual void DisableAfterTime(float time)
	{
		if (Routine != null)
		{
			Routine = null;
		}
		Routine = StartCoroutine(Co_ReturnToPoolAfterTime(time));
	}

	protected virtual IEnumerator Co_ReturnToPoolAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		PoolManager.Instance.Push(OriginPrefab, this);
		Routine = null;
	}

	protected virtual void OnDisable()
	{
		DisableAllChildEffect();
	}
}