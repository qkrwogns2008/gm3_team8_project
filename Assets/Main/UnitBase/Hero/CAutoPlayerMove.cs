using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CAutoPlayerMove : MonoBehaviour
{
    #region 인스펙터
    [Header("On/Off")]
    [SerializeField] private bool _isAutoMove = true;
    #endregion

    #region 내부변수
    private CUnitBase _targetEnemy;                     // 현재 목표 타겟
    private Vector3 _targetPos;                          // 타겟 위치

    private SkeletonAnimation _skeletonAnim;
    private CHero PlayerHero;   // 상태 제어용 참조 사용시 CHero참조
    private HeroDataSO _heroData;
    #endregion


    private void Awake()
    {
        
        PlayerHero = GetComponent<CHero>();
        _skeletonAnim = GetComponentInChildren<SkeletonAnimation>();

        if (_skeletonAnim == null)
        {
            Debug.LogError($"{gameObject.name}: SkeletonAnim null");
        }
    }

    void Start()
    {

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
    void Update()
    {

        if (PlayerHero == null || PlayerHero.IsUnitDead)
        {
            return;
        }

        if (_heroData == null && PlayerHero.BaseData != null)
        {
            _heroData = PlayerHero.BaseData as HeroDataSO;
        }

        if(_heroData == null)
        {
            return;
        }
		CheckTarget();

		if (_targetEnemy == null)
		{
			FindClosestEnemy();
		}
        
        if(_targetEnemy != null)
        {
            _targetPos = _targetEnemy.transform.position;
            _targetPos.z = 0f;

            float sqrDistance = (transform.position - _targetPos).sqrMagnitude;

            float finalRange = PlayerHero.FinalAtkRange;
            float sqrAttackRange = finalRange * finalRange;

            if(sqrDistance <= sqrAttackRange)
            {
                StopAndAttack();
            }
            else
            {
                // 자동 켜져있을경우
                if(_isAutoMove)
                {
                    MoveToTarget();
                }
                else
                {
                    if(PlayerHero.CurrentState != EHeroState.Idle)
                    {
                        PlayerHero.ChangeState(EHeroState.Idle);
                    }
                }
            }
        }
        else
        {
            if(PlayerHero.CurrentState != EHeroState.Idle)
            {
                PlayerHero.ChangeState(EHeroState.Idle);
            }
        }

    }

    // 버튼 연결
    public void ToggleAutoMode()
    {
        _isAutoMove = !_isAutoMove;
        _targetEnemy = null;
    }

    // 대상 체크
    void CheckTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        
        if(_targetEnemy.IsUnitDead || !_targetEnemy.gameObject.activeSelf)
        {
            _targetEnemy = null;
            PlayerHero.SetTarget(null);
        }
    }

    void FindClosestEnemy()
    {
        if(CEnemyManager.Instance == null || CEnemyManager.Instance.ActiveEnemies.Count == 0)
        {
            _targetEnemy = null;
            PlayerHero.SetTarget(null);
            return;
        }

        CUnitBase closest = null;

        float detectionRange = PlayerHero.FinalDetectionRange;
        float minSqrDistance = detectionRange * detectionRange;

        foreach(CUnitBase enemy in CEnemyManager.Instance.ActiveEnemies)
        {
            if(enemy == null || !enemy.gameObject.activeSelf || enemy.IsUnitDead)
            {
                continue;
            }
            float sqrDist = (enemy.transform.position - transform.position).sqrMagnitude;

            if(sqrDist< minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closest = enemy;
            }
            
        }
        if(closest!=null)
        {
            _targetEnemy = closest;
            PlayerHero.SetTarget(closest);
        }
        else
        {
            _targetEnemy = null;
            PlayerHero.SetTarget(null);
        }
    }

    // 이동 위치 탐색
    void MoveToTarget()
    {
        PlayerHero.ChangeState(EHeroState.Move);
        LookAtTarget();

        float moveSpeed = PlayerHero.FinalMoveSpeed;

        transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);

        // Z 축 고정
        Vector3 curPos = transform.position;
        curPos.z = 0f;
        transform.position = curPos;

    }

    // 이동 위치 바라보기
    void LookAtTarget()
    {
        // 대상이 오른쪽
        if (_targetPos.x > transform.position.x)
        {
            if (_skeletonAnim != null)
            {
                // 그대로
                _skeletonAnim.Skeleton.ScaleX = -1f;
            }
        }
        // 대상이 왼쪽
        else if (_targetPos.x < transform.position.x)
        {
            if (_skeletonAnim != null)
            {
                // 그대로
                _skeletonAnim.Skeleton.ScaleX = 1f;
            }
        }
    }

    void StopAndAttack()
    {
        LookAtTarget();

		PlayerHero.ChangeState(EHeroState.Combat);
    }
}
