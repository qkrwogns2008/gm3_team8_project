using UnityEngine;

[CreateAssetMenu(fileName = "HeroAudioData", menuName = "Audio/HeroAudioSO")]
public class HeroAudioSO : ScriptableObject
{
    public AudioClip Attack;
    public AudioClip AttackDamaged;
    public AudioClip Critical;
    public AudioClip CriticalDamaged;
    public AudioClip Skill;
    public AudioClip SkillDamaged;

    public AudioClip hit;

     /*
     AudioSO 활용법
        1. 모객체에 관련 SO 데이터 변수 선언 및 인스펙터에서 드래그 앤 드롭
            [SerializeField] private HeroAudioSO _audioData; 
        2. SoundManager에서 싱글톤 인스턴스를 통해 사운드 재생
            SoundManager.Instance.PlayUnitSFX(_audioData.Attack);
     */
}