using UnityEngine;

[CreateAssetMenu(fileName = "EffectDataSO_", menuName = "ScriptableObjects/Effect Data (SO)")]
public class EffectDataSO : ScriptableObject
{
	[Header("ÀÌÆåÆ® ÀÌ¸§")]
	[SerializeField] private string _name;

	[Header("ÀÌÆåÆ® ÇÁ¸®ÆÕ")]
	[SerializeField] private EffectBase _prefab;

	[Header("¼ÒÈ¯ ¿ÀÇÁ¼Â")]
	[SerializeField] private Vector3 _offset;

	[Header("ÀÌÆåÆ® ¼±µô·¹ÀÌ")]
	[SerializeField] private float _preDelay;

	public string Name => _name;
	public EffectBase Prefab => _prefab;
	public Vector3 Offset => _offset;
	public float PreDelay => _preDelay;
}