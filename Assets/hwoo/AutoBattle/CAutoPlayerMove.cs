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
    [Header("SpineAnim")]
    [SerializeField] private string _idleAnim = "Idle";
    [SerializeField] private string _moveAnim = "Move";
    #endregion
    #region 내부변수
    private Transform _targetEnemy;                     // 현재 목표 타겟
    private Vector3 _targetPos;                          // 타겟 위치
    private bool _isMoving = false;                      // 이동 상태 확인

    private SkeletonAnimation _skeletonAnim;
    private HeroBaseDummy PlayerHero;   // 상태 제어용 참조 사용시 CHero참조
    #endregion

    private void Awake()
    {
        PlayerHero = GetComponent<HeroBaseDummy>();
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

        FindClosestEnemy();
        
        if(_targetEnemy != null)
        {
            _targetPos = _targetEnemy.position;
            _targetPos.z = 0f;

            float distanceToEnemy = Vector2.Distance(transform.position, _targetPos);

            CheckTarget();

            FindClosestEnemy();

            if(distanceToEnemy <= _attackRange)
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
        CUnitBase enemy = _targetEnemy.GetComponent<CUnitBase>();

        
        if(enemy == null || enemy.IsUnitDead || !_targetEnemy.gameObject.activeSelf)
        {
            _targetEnemy = null;
        }
    }

    void FindClosestEnemy()
    {
        if(CEnemyManager.Instance == null || CEnemyManager.Instance.ActiveEnemies.Count == 0)
        {
            _targetEnemy = null;
            return;
        }

        Transform closest = null;
        float minDistance = _detectionRange;

        foreach(Transform enemy in CEnemyManager.Instance.ActiveEnemies)
        {
            if(enemy == null || !enemy.gameObject.activeSelf)
            {
                continue;
            }
            CUnitBase enemyUnit = enemy.GetComponent<CUnitBase>();
            if(enemyUnit!= null && enemyUnit.IsUnitDead)
            {
                Debug.Log($"대상 : {enemy.name} 상태 : Die");
                continue;
                
            }

            float distance = Vector2.Distance(transform.position, enemy.position);

            if(distance < minDistance)
            {
                Debug.Log($"대상 : {enemy.name} 상태 : 범위 밖");
                minDistance = distance;
                closest = enemy;
            }
        }

        _targetEnemy = closest;
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

        /// </summary>
        /// 공격 로직 실행
        /// </summary>
        CUnitBase targetUnit = _targetEnemy.GetComponent<CUnitBase>();
        PlayerHero.TryAttack(targetUnit);
    }
}
