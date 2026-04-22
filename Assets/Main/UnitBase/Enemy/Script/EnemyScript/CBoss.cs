using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CBoss : CEnemyBase
{
    protected enum EAttackType { Normal, Critical, Skill }

    #region 인스펙터

    [Header("보스 전용")]
    [SerializeField] protected EnemyBaseSO BossData;
    [Header("상태")]
    [SerializeField] public EHeroState CurrentState = EHeroState.Idle;
    [Header("스킬")]
    [SerializeField] private bool _enableUseSkill = true;
    [Header("스킬 쿨타임")]
    [SerializeField] private float _skillCooldown = 7.0f;
    [Header("스킬 데미지 비율")]
    [SerializeField] private float _skillDamageMultiplier = 2.5f;
    [Header("스킬 애니메이션")]
    [SpineAnimation(dataField = "SkeletonAni")]
    [SerializeField] protected string _skillAnimation;
    [Header("보스 UI")]
    [SerializeField] private GameObject _myHealthBar;
    #endregion
    #region 내부 변수
    private float _nextSkillTime;              // 다음 스킬 가능 시간
    private float _scaledMaxHP;
    private float _scaledMaxAtk;
    #endregion

    
    // 공격중인지 확인
    public new bool IsAttacking => MotionRoutine != null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitUnitStats();
    }

    protected override void InitUnitStats()
    {
        base.InitUnitStats();
        

        if (EnemyData != null && CDataManager.Instance != null)
        {
            int stage = CDataManager.Instance.UserData.CurrentStageLevel;

            // 스테이지당 체력 15% 공격력 15%
            float hpGrowth = Mathf.Pow(1.15f, stage - 1);
            float atkGrowth = Mathf.Pow(1.15f, stage - 1);

            unitName = EnemyData.UnitName;

            // 추가 배율
            _scaledMaxHP = Mathf.RoundToInt(EnemyData.BaseMaxHp * hpGrowth);
            _scaledMaxAtk = Mathf.RoundToInt(EnemyData.BaseAttackDamage * atkGrowth);

            currentHp = _scaledMaxHP;
        }
        _nextSkillTime = Time.time + 2f;
    }

    public void ChangeState(EHeroState state)
    {
        if (CurrentState == state && state != EHeroState.Combat)
        {
            return;
        }
        CurrentState = state;

        if (IsAttacking && state != EHeroState.Death)
        {
            return;
        }

        switch (CurrentState)
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

    #region 전투

    public override void TryAttack(CUnitBase target)
    {
        if (!IsAvailable() || target == null)
        {
            return;
        }

        if (_enableUseSkill && Time.time >= _nextSkillTime)
        {
            OnSkill(target);
        }
        else
        {
            OnAttack(target);
        }
    }

    protected virtual void OnSkill(CUnitBase target)
    {
        if (MotionRoutine != null)
        {
            return;
        }

        // 쿨타임 
        _nextSkillTime = Time.time + _skillCooldown;

        // 공격 딜레이
        ApplyAttackCooldown();

        // 데미지
        float finalSkillDamage = _scaledMaxAtk * _skillDamageMultiplier;

        MotionRoutine = StartCoroutine(Co_PlayMotion(_skillAnimation, target, finalSkillDamage));

        if (PrintLog)
        {
            Debug.Log("보스 스킬 발동");
        }

    }

    protected override void OnAttack(CUnitBase target)
    {
        if (MotionRoutine != null)
        {
            return;
        }
        ApplyAttackCooldown();

        MotionRoutine = StartCoroutine(Co_PlayMotion(AttackAnimation, target, _scaledMaxAtk));

    }

    protected override IEnumerator Co_PlayMotion(string animationName, CUnitBase target, float damage)
    {
        var trackEntry = SkeletonAni.AnimationState.SetAnimation(0, animationName, false);

        if (trackEntry != null)
        {
            yield return new WaitForSeconds(trackEntry.Animation.Duration);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (target != null && !target.IsUnitDead)
        {
            target.TakeDamage(damage, this);
        }

        MotionRoutine = null;

        if (CurrentState == EHeroState.Move)
        {
            SetAnimation("Move", true);
        }
        else
        {
            SetAnimation("Idle", true);
        }

    }

    protected override void Die()
    {
        if(IsDead)
        {
            return;
        }
        IsDead = true;

        if (_myHealthBar != null)
        {
            _myHealthBar.SetActive(false);
        }
        if (MainStageController.Instance != null)
        {
            MainStageController.Instance.MainStageUp();
        }
        if (CEnemyManager.Instance != null)
        {
            CEnemyManager.Instance.UnregisterEnemy(this);
        }
        if (CBossSpawner.Instance != null)
        {
            CBossSpawner.Instance.ClearActiveBoss();
        }
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CO_DestroyBoss());
        }

        base.Die();

        
    }

    private IEnumerator CO_DestroyBoss()
    {
        yield return new WaitForSeconds(3.0f);
        // 보스 체력바 비활성화
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        //Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 강제 파괴되었을때도 리스트에서 빼주기
        if(CEnemyManager.Instance != null)
        {
            CEnemyManager.Instance.UnregisterEnemy(this);
        }
    }
    #endregion
}
