using UnityEngine;
using Spine.Unity;
using System.Collections;

public enum EUnitState
{
    Idle,
    Move,
    Wander,
    Tracking,
    Attack,
    Dead
}

public class CEnemyBase : CUnitBase
{
    [SerializeField] protected Transform _effectPos;


    protected EnemyBaseSO _enemySO => OriginData as EnemyBaseSO;
    protected Vector3 _startPosition;

    public override bool IsUnitDead => IsDead;

    public EnemyBaseSO EnemyData => OriginData as EnemyBaseSO;
    public CUnitBase TargetHero => Target;

    protected override void InitUnitStats()
    {
        base.InitUnitStats();

        BaseMaxHp = 100f;
        BaseAtkDamage = 10f;

        CurrentHp = FinalMaxHP;

        if (EnemyData != null)
        {
            

            DetectionRange = EnemyData.DetectionRange;
        }
        
    }

    protected override void Update()
    {
        base.Update();
        // 추격 포기 거리
        
       
        if(SkeletonAni != null && SkeletonAni.AnimationName == "Attack_A")
        {
            return;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // 상태 초기화
        IsDead = false;

        // 능력치 초기화
        InitUnitStats();

        // 애니메이션 초기화
        if(SkeletonAni != null)
        {
            SkeletonAni.AnimationState.SetAnimation(0, "Idle", true);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    // 공격
    protected override void OnAttack(CUnitBase target)
    {
        base.OnAttack(target);
        if(SkeletonAni == null || MotionRoutine != null)
        {
            return;
        }
    }

    protected override IEnumerator Co_PlayMotion(string animationName, CUnitBase target, float damage)
    {
        var trackEntry = SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
        SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);

        if(trackEntry != null)
        {
            // 애니메이션 재생되는동안 기다리기
            // 중복 실행 방지
            yield return new WaitForSeconds(trackEntry.Animation.Duration);
        }
        else
        {
            // 애니메이션 없을경우
            yield return new WaitForSeconds(0.5f);
        }
        
        if(target != null && !target.IsUnitDead)
        {
            target.TakeDamage(damage, this);
        }

        ApplyAttackCooldown();

        MotionRoutine = null;
    }

    protected override void ApplyAttackCooldown()
    {
        float cooldown = (EnemyData != null) ? EnemyData.AttackSpeed : 1.0f;
        NextAttackTime = Time.time + cooldown;
    }

    public void LookAt(Vector3 targetPos)
    {
        if (SkeletonAni == null)
        {
            return;
        }

        // 대상이 자신보다 오른쪽일 경우
        if (targetPos.x > transform.position.x)
        {
            // 뒤집기
            SkeletonAni.Skeleton.ScaleX = -1f;
        }
        // 대상이 자신보다 왼쪽일 경우
        else if (targetPos.x < transform.position.x)
        {
            // 그대로
            SkeletonAni.Skeleton.ScaleX = 1f;
        }
    }

    // 피격
    public override void TakeDamage(float damage, CUnitBase attacker)
    {
        base.TakeDamage(damage, attacker);

        
    }

    protected override void Die()
    {
        if (IsDead)
        {
            return;
        }

        base.Die();
        SetAnimation("Death", false);

        if(_enemySO != null)
        {
            Debug.Log($"{UnitName} 처치");
            Debug.Log($"골드 : {_enemySO.GoldReward}, 경험치: {_enemySO.ExpReward}, 아이템: {_enemySO.ItemReward}");
        }

        StopAllCoroutines();

        StartCoroutine(CoReturnToPool());
    }

    private IEnumerator CoReturnToPool()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    // 편의성
    public void SetAnimation(string animName, bool loop)
    {
        if(SkeletonAni == null || SkeletonAni.AnimationState == null)
        {
            return;
        }

        Spine.TrackEntry currentTrack = SkeletonAni.AnimationState.GetCurrent(0);

        if(currentTrack == null || currentTrack.Animation.Name != animName)
        {
            SkeletonAni.AnimationState.SetAnimation(0, animName, loop);
        }
    }

}
