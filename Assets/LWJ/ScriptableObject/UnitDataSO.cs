using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO_", menuName = "ScriptableObjects/Unit Data (SO)")]
public class UnitDataSO : ScriptableObject
{
	#region 인스펙터
	[Header("능력치")]
	[SerializeField] private string _unitName;
	[SerializeField] private float _baseMaxHp;
	[SerializeField] private float _baseAttackDamage;
	[SerializeField] private float _baseAttackDelay;
	[SerializeField] private float _attackRange;
	[SerializeField] private float _baseMoveSpeed;

	[Header("승수")]
	[SerializeField] private float _maxHPMultiplier = 1.0f;
	[SerializeField] private float _attackDamageMultiplier = 1.0f;
	[SerializeField] private float _attackSpeedMultiplier = 1.0f;
	[SerializeField] private float _moveSpeedMultiplier = 1.0f;
	#endregion
	
	#region 프로퍼티
	public string UnitName => _unitName;
	public float BaseMaxHp => _baseMaxHp;
	public float BaseAttackDamage => _baseAttackDamage;
	public float BaseAttackDelay => _baseAttackDelay;
	public float AttackRange => _attackRange;
	public float BaseMoveSpeed => _baseMoveSpeed;
	public float MaxHPMultiplier => _maxHPMultiplier;
	public float AttackDamageMultiplier => _attackDamageMultiplier;
	public float AttackSpeedMultiplier => _attackSpeedMultiplier;
	public float MoveSpeedMultiplier => _moveSpeedMultiplier;
	#endregion
}