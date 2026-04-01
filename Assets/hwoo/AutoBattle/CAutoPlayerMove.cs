using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAutoPlayerMove : MonoBehaviour
{
    #region 인스펙터
    [Header("Move")]
    [SerializeField] private float _walkSpeed = 2f;             // 이동속도 (임시) 이후 외부에서 받아올것
    [SerializeField] private float _attackRange = 1.5f;         // 공격 사거리 (임시)
    [Header("Tracking")]
    [SerializeField] private float _detectionRange = 100f;    // 탐지 범위(맵 전체를 덮을 수 있게
    // 타겟 발견시 속도는 기본과 같음
    #endregion

    #region 내부변수
    private CUnitBase _targetEnemy;                     // 현재 목표 타겟
    private Vector3 _targetPos;                          // 타겟 위치

    private SkeletonAnimation _skeletonAnim;
    private CHero PlayerHero;   // 상태 제어용 참조 사용시 CHero참조
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
            float sqrAttackRange = _attackRange * _attackRange;

            if(sqrDistance <= sqrAttackRange)
            {
                StopAndAttack();
            }
            else
            {
                MoveToTarget();
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
        float minSqrDistance = _detectionRange * _detectionRange;

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

        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkSpeed * Time.deltaTime);

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
