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

    protected EnemyBaseSO _enemySO;
    protected Vector3 _startPosition;

    private bool _isAttackCooldown = false;

    public bool IsUnitDead => IsDead;

    public EnemyBaseSO EnemyData => OriginData as EnemyBaseSO;

    protected override void InitUnitStats()
    {
        base.InitUnitStats();

        
    }

    protected override void Update()
    {
        base.Update();
        // 추격 포기 거리
        if (TargetEnemy != null && _enemySO != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, TargetEnemy.transform.position);

            if(distanceToEnemy > _enemySO.GiveUpRange)
            {
                // 타겟 초기화
                TargetEnemy = null;
            }
        }
        if (SkeletonAni != null)
        {
            if (SkeletonAni.AnimationName == "Attack_A")
            {
                return;
            }
        }
    }

    private void OnEnable()
    {
        // 상태 초기화
        _isAttackCooldown = false;
        IsDead = false;

        // 능력치 초기화
        InitUnitStats();

        // 애니메이션 초기화
        if(SkeletonAni != null)
        {
            SkeletonAni.AnimationState.SetAnimation(0, "Idle", true);
        }
    }

    // 공격
    protected override void OnAttack(CUnitBase target)
    {
        Debug.Log("공격 실행");
        // 공격 애니메이션 1회 재생
        SetAnimation("Attack_A", false);
        // 이후 Idle
        SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0f);

        // 이펙트 생성
        //if(_enemySO != null && _enemySO.AttackEffectPrefab != null)
        //{
        //    Vector3 spawnPoint = (_effectPos != null ? _effectPos.position : transform.position);
        //    Instantiate(_enemySO.AttackEffectPrefab, spawnPoint, Quaternion.identity);
        //}

        if(target != null)
        {
            target.TakeDamage(FinalAttackDamage, this);
            Debug.Log($"{UnitName}의 공격, 피해량: {FinalAttackDamage}");
        }
    }

    public void TryAttack(CUnitBase target)
    {
        if(IsDead || target == null || _isAttackCooldown)
        {
            Debug.Log("타겟Null");
            return;
        }
        StartCoroutine(CoAttackRoutine(target));
    }

    

    private IEnumerator CoAttackRoutine(CUnitBase target)
    {
        _isAttackCooldown = true;

        OnAttack(target);

        float cooldown = (_enemySO != null) ? _enemySO.AttackSpeed : 1.0f;
        yield return new WaitForSeconds(cooldown);

        _isAttackCooldown = false;
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
