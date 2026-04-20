using UnityEngine;

[CreateAssetMenu(fileName = "HeroAudioData", menuName = "Audio/HeroAudioSO")]
public class HeroAudioSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private AudioClip _attack;
	[SerializeField] private AudioClip _attackDamaged;
	[SerializeField] private AudioClip _critical;
	[SerializeField] private AudioClip _criticalDamaged;
	[SerializeField] private AudioClip _skill;
	[SerializeField] private AudioClip _skillDamaged;
	#endregion

	#region 프로퍼티
	public AudioClip Attack => _attack;
	public AudioClip AttackDamaged => _attackDamaged;
	public AudioClip Critical => _critical;
	public AudioClip CriticalDamaged => _criticalDamaged;
	public AudioClip Skill => _skill;
	public AudioClip SkillDamaged => _skillDamaged;
	#endregion
}