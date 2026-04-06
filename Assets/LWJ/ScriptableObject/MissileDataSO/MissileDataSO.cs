using UnityEngine;

[CreateAssetMenu(fileName = "MissileDataSO_", menuName = "ScriptableObjects/Missile Data (SO)")]
public class MissileDataSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private string _missileName;
	[SerializeField] private float _moveSpeed;
	[SerializeField] private bool _lookAtTarget;
	#endregion

	#region 프로퍼티
	public string MissileName => _missileName;
	public float MoveSpeed => _moveSpeed;
	public bool LookAtTarget => _lookAtTarget;
	#endregion
}