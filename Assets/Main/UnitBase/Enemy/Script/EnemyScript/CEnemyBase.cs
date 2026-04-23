using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float _giveupRange = 15f;
    [SerializeField] private float _walkRange = 5f;    // 주변 돌아다니는 범위
    [Header("어그로 설정")]
    [SerializeField] private float _switchThreshold = 1.2f;
    [SerializeField] private float _minTargetStayTime = 1.0f;
    [SerializeField] private float _finalMaxHP;
    [SerializeField] private float _finalMaxAtk;
    protected EnemyBaseSO _enemySO => OriginData as EnemyBaseSO;
    protected Vector3 _startPosition;
    protected Dictionary<CUnitBase, float> _threatTable = new Dictionary<CUnitBase, float>();
    private float _lastTargetSwitchTime = -900f;    // 마지막으로 타겟이 바뀐 시간. (첫 타겟을 바로 잡기 위해 작은값지정)
    private int _mySpawnIndex;
	protected EnemyAudioSO AudioSO;
    

    public override bool IsUnitDead => IsDead;

    public EnemyBaseSO EnemyData => OriginData as EnemyBaseSO;
    public CUnitBase TargetHero => Target;
    private CAutoEnemyMove _moveScript;
    private CSpawnArea _mySpawner;
    public virtual float FinalGiveUpRange => _giveupRange * ScaleMultiplier;
    public virtual float FinalWalkRange => _walkRange * ScaleMultiplier;

    public bool IsAttacking { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        _moveScript = GetComponent<CAutoEnemyMove>();
    }
    protected override void InitUnitStats()
    {
        base.InitUnitStats();
        

        if (EnemyData != null && CDataManager.Instance != null)
        {
			AudioSO = EnemyData.AudioSO;
            int stage = CDataManager.Instance.UserData.CurrentStageLevel;

            #region 성장공식 이후 수정 필요

            // 스테이지당 체력 15%, 공격력 10%
            float hpGrowth = Mathf.Pow(1.15f, stage - 1);
            float atkGrowth = Mathf.Pow(1.1f, stage - 1);
            #endregion

            unitName = EnemyData.UnitName;

            _finalMaxHP = Mathf.RoundToInt(EnemyData.BaseMaxHp * hpGrowth);
            currentHp = _finalMaxHP;

            _finalMaxAtk = Mathf.RoundToInt(EnemyData.BaseAttackDamage * atkGrowth);
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
        NextAttackTime = 0f;
        Target = null;

        // 유령 코루틴 변수 삭제
        MotionRoutine = null;

        // 애니메이션 초기화
        if(SkeletonAni != null)
        {
            SkeletonAni.AnimationState.ClearTracks();
            SkeletonAni.AnimationName = null;

            SkeletonAni.Skeleton.SetToSetupPose();
            SkeletonAni.AnimationState.SetAnimation(0, "Idle", true);
        }
        // 능력치 초기화
        InitUnitStats();

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }



    // 공격
    protected override void OnAttack(CUnitBase target)
    {
		ApplyAttackCooldown();

		if (SkeletonAni == null)
		{
			return;
		}

		if (MotionRoutine != null)
		{
			return;
		}

		AudioClip castAudio = (AudioSO != null) ? AudioSO.Attack : null;

		MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation,
			target, 
			BaseAtkDamage,
			castAudio));

		if (PrintLog)
		{
			Debug.Log($"{UnitName}의 일반 공격!");
		}
	}

    protected virtual IEnumerator Co_PlayMotion(string animationName, CUnitBase target, float damage, AudioClip castAudio = null)
    {
        //공격시 이동 차단
        IsAttacking = true;

        var trackEntry = SkeletonAni.AnimationState.SetAnimation(0, animationName, false);
        //SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);
		if (castAudio != null)
		{
			SoundManager.Instance.PlayUnitSFX(castAudio); // 공격 오디오 재생
		}

		if (trackEntry != null)
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
            target.TakeDamage(_finalMaxAtk, this);
        }

        ApplyAttackCooldown();

        MotionRoutine = null;

        IsAttacking = false;
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
    public override void TakeDamage(float damage, CUnitBase attacker, bool summonNormalHitEffect = true, AudioClip HitAudio = null)
    {
        base.TakeDamage(damage, attacker, summonNormalHitEffect, HitAudio);
        if(IsDead || attacker == null)
        {
            return;
        }

        
        AddThreat(attacker, damage);
        UpdateBestTarget();
        
    }
    #region 어그로
    private void AddThreat(CUnitBase attacker, float damage)
    {
        if(!_threatTable.ContainsKey(attacker))
        {
            _threatTable.Add(attacker, 0f);
        }
        _threatTable[attacker] += damage;
    }
    private void UpdateBestTarget()
    {
        if(_threatTable.Count == 0)
        {
            return;
        }
        if(Time.time - _lastTargetSwitchTime < _minTargetStayTime && Target != null && !Target.IsUnitDead)
        {
            return;
        }
        CUnitBase bestAttacker = null;
        float maxThreat = -1f;

        // 순회중 삭제를 위한 임시 리스트
        List<CUnitBase> toRemove = new List<CUnitBase>();

        // 가장 높은 어그로대상 탐색 및 검사
        foreach (var pair in _threatTable)
        {
            CUnitBase unit = pair.Key;

            // 적이 사라졌거나 죽으면 삭제 목록에 추가
            if (unit == null || unit.IsUnitDead || !unit.gameObject.activeSelf)
            {
                toRemove.Add(unit);
                continue;
            }
            
            // 가장 높은 데미지를 준 대상 찾기
            if (pair.Value > maxThreat)
            {
                maxThreat = pair.Value;
                bestAttacker = unit;
            }
        }
        
        // 유효하지 않은 데이터 청소
        foreach(var unit in toRemove)
        {
            _threatTable.Remove(unit);
        }
        // 더이상 공격 대상이 없을경우 종료
        if(bestAttacker == null)
        {
            return;
        }
        // 타겟 전환 결정
        // 현 타겟이 있고 살아있다면 새로운 적이 현재보다 20% 더 강하게 때렷을때 대상 변경
        if(Target != null && !Target.IsUnitDead)
        {
            float currentTargetThreat = _threatTable.ContainsKey(Target) ? _threatTable[Target] : 0f;

            // 새로운 대상의 어그로가 기존보다 확실히 높지 않으면 무시
            if(maxThreat < currentTargetThreat * _switchThreshold)
            {
                return;
            }
        }
        // 최종 타겟 설정 및 행동 변경
        if(Target != bestAttacker)
        {
            SetTarget(bestAttacker);

            // 타겟이 바뀌엇으므로 추격상태로 강제 전환
            _lastTargetSwitchTime = Time.time;

            if(_moveScript != null)
            {
                _moveScript.TriggerForcedAggro();
            }
        }
    }
    #endregion

    public void InitSpawn(CSpawnArea spawner, int index)
    {
        _mySpawner = spawner;
        _mySpawnIndex = index;

        IsDead = false;
        IsAttacking = false;
        Target = null;
        NextAttackTime = 0f;

        // 끼임 방지
        if(_moveScript != null)
        {
            _moveScript.enabled = false;
            _moveScript.enabled = true;
        }
    }
    protected override void Die(AudioClip deathAudio = null)
    {
        
        if(_mySpawner != null)
        {
            _mySpawner.OnMonsterDeath(_mySpawnIndex);
        }

        if(ItemManager.Instance != null && EnemyData != null)
        {
            ItemManager.Instance.ProcessDrop(EnemyData._dropTable, transform.position);
        }

        base.Die();
        SetAnimation("Death", false);
		
		StopAllCoroutines();

        StartCoroutine(CoReturnToPool());
    }


    private IEnumerator CoReturnToPool()
    {
        yield return new WaitForSeconds(3f);
        if(OriginPrefab != null)
        {
            PoolManager.Instance.Push(OriginPrefab, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    #region 편의성
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
    #endregion
}
