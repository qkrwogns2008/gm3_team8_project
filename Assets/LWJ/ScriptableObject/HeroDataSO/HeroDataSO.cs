using UnityEngine;

/*
노멀 : 0000
레어 : 1000
에픽 : 2000
유니크 : 3000
레전드 : 4000
*/
public enum EHeroID
{
	None = 0,
	Baskin = 1,
	Nami = 2,

	Loto = 1001,
	Jak = 1002,

	Sarah = 2001,
	Elga = 2002,
	Karon = 2003,
	Rook = 2004,

	Snipper = 3001,
	Shane = 3002,
	Evan = 3003,
	Alice = 3004,

	Teo = 4001,
	Yeonhee = 4002,
}

[CreateAssetMenu(fileName = "HeroDataSO_", menuName = "ScriptableObjects/Hero Data (SO)")]
public class HeroDataSO : UnitDataSO
{
	#region 인스펙터
	[Header("ID")]
	[SerializeField] private EHeroID _heroId;

	[Header("능력치")]
	[SerializeField] private float _baseDefense;
	
	[Header("기본 공격 이펙트")]
	[SerializeField] private EffectDataSO _attackEffect; // 기본 공격 이펙트

	[Header("치명타 설정")]
	[SerializeField] private EffectDataSO _criticalEffect; // 치명타 공격 이펙트
	//[SerializeField] private float BaseCriticalActionInterval = 1.5f;
	[SerializeField, Range(0f, 100f)] private float _criticalChance = 5f; // 치명타 확률
	[SerializeField] private float _baseCriticalDamageRatio = 1.5f; // 치명타 데미지 계수 (1f = 100%)
	[SerializeField] private float _criticalAttackMultiplier = 2f; // 치명타 데미지 승수

	[Header("스킬 설정")]
	[SerializeField] private EffectDataSO _skillEffect; // 스킬 이펙트
	[SerializeField] private float _skillActionInterval = 2f; // 스킬 액션 딜레이
	[SerializeField] private float _baseSkillDamageRatio = 2.5f; // 스킬 데미지 계수 (1f = 100%)
	[SerializeField] private float _baseSkillCooldown = 8.0f; // 쿨타임
	[SerializeField] private float _cooldownMultiplier = 1.0f; // 쿨타임 감소 승수
	#endregion

	#region 프로퍼티
	public EHeroID HeroID => _heroId;
	public float BaseDefense => _baseDefense;
	public EffectDataSO AttackEffect => _attackEffect;
	public EffectDataSO CriticalEffect => _criticalEffect;
	public float CriticalChance => _criticalChance;
	public float BaseCriticalDamageRatio => _baseCriticalDamageRatio;
	public float CriticalAttackMultiplier => _criticalAttackMultiplier;
	public EffectDataSO SkillEffect => _skillEffect;
	public float SkillActionInterval => _skillActionInterval;
	public float BaseSkillDamageRatio => _baseSkillDamageRatio;
	public float BaseSkillCooldown => _baseSkillCooldown;
	public float CooldownMultiplier => _cooldownMultiplier;
	#endregion
}