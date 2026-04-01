using UnityEngine;

[CreateAssetMenu(fileName = "HeroDataSO_", menuName = "ScriptableObjects/Hero Data (SO)")]
public class HeroDataSO : UnitDataSO
{
	#region 인스펙터
	[Header("기본 공격 이펙트")]
	[SerializeField] private EffectDataSO _attackEffect; // 기본 공격 이펙트

	[Header("치명타 설정")]
	[SerializeField] private EffectDataSO _criticalEffect; // 치명타 공격 이펙트
	//[SerializeField] private float BaseCriticalActionInterval = 1.5f;
	[SerializeField] private float _criticalChance = 5f; // 치명타 확률
	[SerializeField] private float _criticalAttackMultiplier = 2f; // 치명타 데미지 승수

	[Header("스킬 설정")]
	[SerializeField] private EffectDataSO _skillEffect; // 스킬 이펙트
	[SerializeField] private float _skillActionInterval = 2f; // 스킬 액션 딜레이
	[SerializeField] private float _baseSkillCooldown = 8.0f; // 쿨타임
	[SerializeField] private float _cooldownMultiplier = 1.0f; // 쿨타임 감소 승수
	#endregion

	#region 프로퍼티
	public EffectDataSO AttackEffect => _attackEffect;
	public EffectDataSO CriticalEffect => _criticalEffect;
	public float CriticalChance => _criticalChance;
	public float CriticalAttackMultiplier => _criticalAttackMultiplier;
	public EffectDataSO SkillEffect => _skillEffect;
	public float SkillActionInterval => _skillActionInterval;
	public float BaseSkillCooldown => _baseSkillCooldown;
	public float CooldownMultiplier => _cooldownMultiplier;
	#endregion
}