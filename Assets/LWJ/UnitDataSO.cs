using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO_", menuName = "ScriptableObjects/Unit Data (SO)")]
public class UnitDataSO : ScriptableObject
{
	[SerializeField] private string unitName;
	[SerializeField] private float maxHp;
	[SerializeField] private float attackPower;
	[SerializeField] private float attackRange;
	[SerializeField] private GameObject attack1Prefab;
	[SerializeField] private GameObject skill1Prefab;
	[SerializeField] private GameObject skill2Prefab;
	[SerializeField] private float walkSpeed;

	public string UnitName => unitName;
	public float MaxHp => maxHp;
	public float AttackPower => attackPower;
	public float AttackRange => attackRange;
	public GameObject Attack1Prefab => attack1Prefab;
	public GameObject Skill1Prefab => skill1Prefab;
	public GameObject Skill2Prefab => skill2Prefab;
	public float WalkSpeed => walkSpeed;
}