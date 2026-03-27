using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectCatalog
{
	[Header("¿Ã∆Â∆Æ «¡∏Æ∆’")]
	[SerializeField] private EffectBase _prefab;

	[Header("¿Ã∆Â∆Æ º±µÙ∑π¿Ã")]
	[SerializeField] private float _preDelay;

	[Header("º“»Ø ø¿«¡º¬")]
	[SerializeField] private Vector3 _offset;

	public EffectBase Prefab => _prefab;
	public Vector3 Offset => _offset;
	public float PreDelay => _preDelay;
}

[CreateAssetMenu(fileName = "EffectDataSO_", menuName = "ScriptableObjects/Effect Data (SO)")]
public class EffectDataSO : ScriptableObject
{
	[Header("¿Ã∆Â∆Æ ¿Ã∏ß")]
	[SerializeField] private string _name;

	[Header("¿Ã∆Â∆Æ ∏Ò∑œ")]
	[SerializeField] private List<EffectCatalog> _effectCatalog;

	public string Name => _name;
	public IReadOnlyList<EffectCatalog> Catalog => _effectCatalog;
}