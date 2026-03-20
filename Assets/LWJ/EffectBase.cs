using UnityEngine;

/// <summary>
/// 유닛이 공격할 때 출력되는 이펙트 처리 로직을 담당합니다.
/// </summary>
public class EffectBase : MonoBehaviour
{
	// 외부에서 이펙트 프리팹 생성 → 초기화 값 주입(방향, 비활성 예약)
	#region 인스펙터
	[Header("Effect")]
	[SerializeField] protected GameObject _leftEffect;
	[SerializeField] protected GameObject _rightEffect;

	[Header("Praticle Scale Multiplier")]
	[SerializeField] protected bool _enableParticleBaseScaleMultiplier = true;
	[SerializeField] protected float _baseScaleMultiplier = 1.0f; // 오직 초기화에서만 사용
	#endregion

	#region 내부 변수
	protected Vector3 _baseScale;
	#endregion

	protected virtual void Awake()
	{
		if (_leftEffect == null || _rightEffect == null)
		{
			Debug.LogWarning($"{name} : 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		_leftEffect.SetActive(false);
		_rightEffect.SetActive(false);
		_baseScale = transform.localScale;

		if (_enableParticleBaseScaleMultiplier)
		{
			SetEffectScale(_baseScaleMultiplier);
		}
	}

	/// <summary>
	/// Effect의 방향, 지속 시간, 크기를 설정합니다.
	/// </summary>
	/// <param name="isFacingRight">방향</param>
	/// <param name="lifeTime">지속시간</param>
	/// <param name="scale">크기 (기본 1.0f)</param>
	public virtual void Init(bool isFacingRight, float lifeTime, float scale = 1.0f)
	{
		SetEffectDirection(isFacingRight);
		DisableAfterTime(lifeTime);
		if (!_enableParticleBaseScaleMultiplier)
		{
			SetEffectScale(scale);
		}
	}

	protected virtual void SetEffectDirection(bool isFacingRight)
	{
		if (_leftEffect == null || _rightEffect == null)
		{
			Debug.LogWarning($"{name} : 인스펙터 null");
			return;
		}

		// 우측 방향 이펙트 출력
		if (isFacingRight)
		{
			_leftEffect.SetActive(false);
			_rightEffect.SetActive(true);
		}
		// 좌측 방향 이펙트 출력
		else
		{
			_leftEffect.SetActive(true);
			_rightEffect.SetActive(false);
		}
	}

	/// <summary>
	/// Effect 크기를 설정합니다.
	/// </summary>
	/// <param name="scale"></param>
	public virtual void SetEffectScale(float scale)
	{
		Vector3 nextScale = transform.localScale;
		nextScale = _baseScale * scale;
		transform.localScale = nextScale;
	}

	protected virtual void DisableAfterTime(float time)
	{
		// 풀링 사용 시 파괴 대신 비활성화로 변경
		Destroy(gameObject, time);
	}
}