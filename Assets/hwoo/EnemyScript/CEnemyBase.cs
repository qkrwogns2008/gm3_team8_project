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

    public EnemyBaseSO EnemyData => _originData as EnemyBaseSO;
    public bool IsDead => _isDead;

    protected override void InitUnitStats()
    {
        base.InitUnitStats();

        _enemySO = _originData as EnemyBaseSO;

        if(_enemySO != null)
        {
            _detectionRange = _enemySO.DetectionRange;
        }
        _startPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        // 추격 포기 거리
        if (_targetEnemy != null && _enemySO != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, _targetEnemy.transform.position);

            if(distanceToEnemy > _enemySO.GiveUpRange)
            {
                // 타겟 초기화
                _targetEnemy = null;
            }
        }
        if (_skeletonAni != null)
        {
            if (_skeletonAni.AnimationName == "Attack_A")
            {
                return;
            }
           
        }
    }

    

    // 공격
    protected override void OnAttack(CUnitBase target)
    {
        Debug.Log("공격 실행");
        // 공격 애니메이션 1회 재생
        SetAnimation("Attack_A", false);
        // 이후 Idle
        _skeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0f);

        // 이펙트 생성
        //if(_enemySO != null && _enemySO.AttackEffectPrefab != null)
        //{
        //    Vector3 spawnPoint = (_effectPos != null ? _effectPos.position : transform.position);
        //    Instantiate(_enemySO.AttackEffectPrefab, spawnPoint, Quaternion.identity);
        //}

        if(target != null)
        {
            target.TakeDamage(_currentAtk, this);
            Debug.Log($"{_unitName}의 공격, 피해량: {_currentAtk}");
        }
    }

    public void TryAttack(CUnitBase target)
    {
        if(_isDead || target == null || _isAttackCooldown)
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
        if (_skeletonAni == null)
        {
            return;
        }

        // 대상이 자신보다 오른쪽일 경우
        if (targetPos.x > transform.position.x)
        {
            // 뒤집기
            _skeletonAni.Skeleton.ScaleX = -1f;
        }
        // 대상이 자신보다 왼쪽일 경우
        else if (targetPos.x < transform.position.x)
        {
            // 그대로
            _skeletonAni.Skeleton.ScaleX = 1f;
        }
    }

    // 피격
    public override void TakeDamage(float damage, CUnitBase attacker)
    {
        base.TakeDamage(damage, attacker);

        // 피격 애니메이션 (살아있을때만)
        if(!_isDead && _skeletonAni != null)
        {
            SetAnimation("hit", false);
            _skeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0f);
        }
    }

    protected override void Die()
    {
        base.Die();

        SetAnimation("Death", false);

        if(_enemySO != null)
        {
            Debug.Log($"{_unitName} 처치");
            Debug.Log($"골드 : {_enemySO.GoldReward}, 경험치: {_enemySO.ExpReward}, 아이템: {_enemySO.ItemReward}");
        }

        Destroy(gameObject, 3f);
    }
    
    // 편의성
    public void SetAnimation(string animName, bool loop)
    {
        if(_skeletonAni == null || _skeletonAni.AnimationState == null)
        {
            return;
        }

        Spine.TrackEntry currentTrack = _skeletonAni.AnimationState.GetCurrent(0);

        if(currentTrack == null || currentTrack.Animation.Name != animName)
        {
            _skeletonAni.AnimationState.SetAnimation(0, animName, loop);
        }
    }

}
