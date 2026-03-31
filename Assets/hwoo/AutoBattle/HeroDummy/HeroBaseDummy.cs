using Spine.Unity;
using System.Collections;
using UnityEngine;

/*
public enum EHeroState
{
    Idle,
    Move,
	Combat,
    Death
}
*/

public class HeroBaseDummy : CUnitBase
{
    //[Header("일반 공격")]
    //[SpineAnimation(dataField = "SkeletonAni")]
    //[SerializeField] protected string _attackAnimation;
    //[SerializeField] protected EffectDataSO _attackEffect;
    //[SerializeField] protected float _baseAttackDelay = 1.0f;

    [Header("State")]
    [SerializeField] public EHeroState CurrentState = EHeroState.Idle;

    [Header("치명타 공격")]
    [SpineAnimation(dataField = "SkeletonAni")]
    [SerializeField] protected string CriticalAnimation;
    [SerializeField] protected EffectDataSO CriticalEffect;
    //[SerializeField] protected float BaseCriticalActionInterval = 1.5f;

    [Header("치명타 수치")]
    [SerializeField] protected float CriticalChance = 20f;
    [SerializeField] protected float CriticalAttackMultiplier = 2f;

    [Header("스킬")]
    [SpineAnimation(dataField = "SkeletonAni")]
    [SerializeField] protected string SkillAnimation;
    [SerializeField] protected EffectDataSO SkillEffect;

    [Header("스킬 수치")]
    [SerializeField] protected float SkillActionInterval = 2f; // 스킬 액션 딜레이
    [SerializeField] protected float BaseSkillCooldown = 5.0f; // 쿨타임
    [SerializeField] protected float CooldownMultiplier = 1.0f; // 쿨타임 감소 승수

    protected float NextSkillTime;

    protected virtual float CriticalDamage => BaseAtkDamage * CriticalAttackMultiplier;
    protected virtual float FinalSkillCooldown => BaseSkillCooldown * CooldownMultiplier;
    protected virtual float FinalSkillActionInterval => SkillActionInterval / AttackSpeedMultiplier;


    // for Test
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnAttack(Target);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnCritical(Target);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnSkill(Target);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Die();
        }
    }

    public void ChangeState(EHeroState state)
    {
        if(CurrentState == state && state != EHeroState.Combat)
        {
            return;
        }
        CurrentState = state;

        switch(CurrentState)
        {
            case EHeroState.Idle:
                SetAnimation("Idle", true);
                break;
            case EHeroState.Move:
                SetAnimation("Move", true);
                break;
			case EHeroState.Combat:
				TryAttack(Target);
				break;
        }
    }

    protected override void OnAttack(CUnitBase target)
    {
        if (SkeletonAni == null || AttackEffect == null)
        {
            Debug.LogWarning("CHero) 인스펙터 null 감지");
            return;
        }

        if (MotionRoutine != null)
        {
            return;
        }

        // 치명타 체크
        bool isCriAttack = (Random.Range(0f, 100f) <= CriticalChance);

        if (PrintLog)
        {
            Debug.Log($"크리티컬 : {isCriAttack}");
        }

        if (isCriAttack && CriticalEffect != null)
        {
            MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalEffect, CriticalAnimation, target, CriticalDamage));
            if (PrintLog)
            {
                Debug.Log($"{UnitName}의 치명타 공격!");
            }
        }
        else
        {
            MotionRoutine = StartCoroutine(Co_PlayMotion(AttackEffect, AttackAnimation, target, BaseAtkDamage));
            if (PrintLog)
            {
                Debug.Log($"{UnitName}의 일반 공격!");
            }
        }
    }

    // for test
    private void OnCritical(CUnitBase target)
    {
        if (SkeletonAni == null || CriticalEffect == null)
        {
            Debug.LogWarning("CHero) 인스펙터 null 감지");
            return;
        }

        if (MotionRoutine != null)
        {
            return;
        }

        MotionRoutine = StartCoroutine(Co_PlayMotion(CriticalEffect, CriticalAnimation, target, CriticalDamage));
        if (PrintLog)
        {
            Debug.Log($"{UnitName}의 치명타 공격!");
        }
    }

    protected void OnSkill(CUnitBase target)
    {
        if (SkeletonAni == null || SkillEffect == null)
        {
            Debug.LogWarning("CHero) 인스펙터 null 감지");
            return;
        }

        if (MotionRoutine != null)
        {
            return;
        }

        MotionRoutine = StartCoroutine(Co_PlayMotion(SkillEffect, SkillAnimation, target, BaseAtkDamage));
        if (PrintLog)
        {
            Debug.Log($"{UnitName}의 스킬 1 발동!");
        }
    }

    /// <summary>
    /// 스파인 애니메이션을 재생하고, effectData의 PreDelay 값에 따라 시간차로 이펙트를 생성합니다. 성공적으로 종료되면 피해를 적용합니다.
    /// </summary>
    protected virtual IEnumerator Co_PlayMotion(EffectDataSO effectData, string animationName, CUnitBase target, float damage)
    {
        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
            MotionRoutine = null;
            yield break;
        }

        SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
        SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

        // 목록의 이펙트를 순차 출력
        for (int i = 0; i < effectData.Catalog.Count; i++)
        {
            EffectCatalog fxData = effectData.Catalog[i];

            if (fxData == null)
            {
                Debug.LogWarning($"CHero) 이펙트 NONE. {effectData.Name} 이펙트 목록 확인");
                continue;
            }

            yield return new WaitForSeconds(fxData.PreDelay);

            if (fxData.Prefab == null)
            {
                Debug.LogWarning($"CHero) 이펙트 프리팹 NONE. {effectData.Name} 이펙트 목록 확인");
                continue;
            }

            // 이펙트 생성 실패 시 즉시 종료
            if (!TrySummonEffect(fxData))
            {
                Debug.LogWarning($"{name} : {effectData.Name} 이펙트 생성 실패");
                MotionRoutine = null;
                yield break;
            }
        }

        if (target != null)
        {
            target.TakeDamage(damage, this);
        }

        MotionRoutine = null;
    }

    // 오버로딩 : 모션 재생 후 정지.
    protected virtual IEnumerator Co_PlayMotion(string animationName, float time, bool disableAfter)
    {
        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("애니메이션 NONE. 인스펙터 확인");
            MotionRoutine = null;
            yield break;
        }

        SkeletonAni.AnimationState.SetAnimation(0, animationName, false);

        yield return new WaitForSeconds(time);

        if (disableAfter)
        {
            gameObject.SetActive(false);
        }

        MotionRoutine = null;
    }

    // 이펙트 소환 시도
    protected virtual bool TrySummonEffect(EffectCatalog fxData)
    {
        EffectBase prefab = fxData.Prefab;

        Vector3 pos = transform.position + fxData.Offset;
        Quaternion rot = Quaternion.Euler(-45f, 0f, 0f);
        EffectBase fx = PoolManager.Instance.Pop(prefab, pos, rot);

        if (fx == null)
        {
            return false;
        }
        fx.Init(prefab, false);

        return true;
    }

    protected override void Die()
    {
        base.Die();

        if (SkeletonAni == null)
        {
            return;
        }

        if (MotionRoutine != null)
        {
            return;
        }

        MotionRoutine = StartCoroutine(Co_PlayMotion(DeathAnimation, DeathDisableTime, true));
        if (PrintLog)
        {
            Debug.Log($"{UnitName} 사망");
        }
    }

    public override void TryAttack(CUnitBase target)
    {
        if (!IsAvailable() || target == null)
        {
            return;
        }

        if (CanUseSkill())
        {
            ExecuteCombat(EAttackType.Skill, target);
        }
        else
        {
            ExecuteCombat(EAttackType.Normal, target);
        }
    }

    protected override void ExecuteCombat(EAttackType type, CUnitBase target)
    {
        switch (type)
        {
            case EAttackType.Skill:
                //Debug.Log($"{UnitName} 스킬 작동");
                NextSkillTime = Time.time + FinalSkillCooldown;
                ApplyAttackCooldown(false);
                OnSkill(target);
                break;
            case EAttackType.Normal:
                //Debug.Log($"{UnitName} 공격 작동");
                ApplyAttackCooldown(true);
                OnAttack(target);
                break;
        }
    }

    // 스킬 사용 가능 여부 체크
    protected virtual bool CanUseSkill()
    {
        if (BaseSkillCooldown > 0f && Time.time >= NextSkillTime)
        {
            return true;
        }

        return false; // 아직 쿨타임 중이거나 설정 안됨
    }

    /// <summary>
    /// 공격 액션 딜레이를 적용합니다. 기본 공격이면 True, 스킬 사용이면 False입니다.
    /// </summary>
    /// <param name="isNormal">기본 공격 여부</param>
    protected void ApplyAttackCooldown(bool isNormal)
    {
        if (isNormal) // 기본 공격
        {
            if (FinalAttackActionInterval > 0f)
            {
                NextAttackTime = Time.time + FinalAttackActionInterval;
            }
        }
        else // 스킬 사용
        {
            if (FinalSkillActionInterval > 0f)
            {
                NextAttackTime = Time.time + FinalSkillActionInterval;
            }
        }
    }

    // 상태 변경 관리

    // 편의성용 SetAnimation 함수
    public void SetAnimation(string animName, bool loop)
    {
        if (SkeletonAni == null || SkeletonAni.AnimationState == null)
        {
            return;
        }

        Spine.TrackEntry currentTrack = SkeletonAni.AnimationState.GetCurrent(0);

        if (currentTrack == null || currentTrack.Animation.Name != animName)
        {
            SkeletonAni.AnimationState.SetAnimation(0, animName, loop);
        }
    }
}

