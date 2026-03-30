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
	#endregion

	#region 내부 변수
	private CHero _herobase; // 영웅이면 영웅의 이벤트를 담기 위해 형변환
	#endregion

	private void Awake()
	{
		if (_base == null)
		{
			Debug.LogWarning("UnitHUD) base null");
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

		UpdateCooldownBar();
	}

	public void SetHPBar(float currentHP, float maxHP)
	{
		float hpRatio = currentHP / maxHP;

		_hpBar.fillAmount = hpRatio;
	}

	public void StartCooldownUI(float time)
	{
		_coolDownBar.fillAmount = 0;
		// 쿨타임 계산해서 게이지로 표시 로직
	}

	public void UpdateCooldownBar()
	{

	}
}