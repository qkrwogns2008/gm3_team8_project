using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour
{
	#region 인스펙터
	[Header("유닛 베이스")]
	[SerializeField] private CUnitBase _base;

	[Header("UI Gauge Bar")]
	[SerializeField] private Image _hpBar;
	[SerializeField] private Image _coolDownBar;

	[Header("쿨다운 종료 이펙트")]
	[SerializeField] private ParticleSystem _fxCooldownEnd;
	#endregion

	#region 내부 변수
	private CHero _herobase; // 영웅이면 영웅의 이벤트를 담기 위해 형변환

	private bool _isCooling = false;
	private float _cooldown;
	private float _guageEndTime;
	#endregion

	private void Awake()
	{
		if (_base == null)
		{
			Debug.LogWarning("UnitHUD) base null");
			gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		_base.OnHpChanged += SetHPBar;
		
		// 형변환 시도
		_herobase = _base as CHero;
		if (_herobase != null)
		{
			_herobase.OnSkillUsed += StartCooldownUI;
		}
	}

	private void OnDisable()
	{
		_base.OnHpChanged -= SetHPBar;

		if (_herobase != null)
		{
			_herobase.OnSkillUsed -= StartCooldownUI;
		}
	}

	private void Update()
	{
		if (!_isCooling)
		{
			return;
		}

		UpdateCooldownBar();
	}

	public void SetHPBar(float currentHP, float maxHP)
	{
		float hpRatio = currentHP / maxHP;

		_hpBar.fillAmount = hpRatio;
	}

	public void StartCooldownUI(float time)
	{
		// 쿨타임이 0초면 게이지를 갱신할 필요 없음.
		if (time <= 0)
		{
			return;
		}

		_cooldown = time;
		_guageEndTime = Time.time + _cooldown;
		_coolDownBar.fillAmount = 0;

		_isCooling = true;
	}

	public void UpdateCooldownBar()
	{
		float ratio = Mathf.Clamp01(1 - (_guageEndTime - Time.time) / _cooldown);

		_coolDownBar.fillAmount = ratio;

		if (ratio >= 1f)
		{
			_isCooling = false;

			if (_fxCooldownEnd != null)
			{
				_fxCooldownEnd.Play();
			}
		}
	}
}