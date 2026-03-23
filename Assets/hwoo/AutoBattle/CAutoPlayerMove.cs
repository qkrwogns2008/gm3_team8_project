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
    [SerializeField] private LayerMask _enemyLayer;        // 탐지할 레이어
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
    #endregion

    void Start()
    {
        _skeletonAnim = GetComponent<SkeletonAnimation>();
        // 첫 번째 적 탐색
        FindClosesEnemy();
    }
    void Update()
    {
        FindClosesEnemy();

        if(_targetEnemy != null)
        {
            _targetPos = _targetEnemy.position;

            // 거리 계산
            float distanceToEnemy = Vector3.Distance(transform.position, _targetPos);

            // 가까우면 공격
            if (distanceToEnemy <= _attackRange)
            {
                StopAttack();
            }
            // 멀면 이동
            else
            {
                _isMoving = true;
                MoveToTarget();
            }
        }
        else
        {
            //대기
            _isMoving = false;
        }
    }

    void FindClosesEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _detectionRange, _enemyLayer);

        if(enemies.Length>0)
        {
            Transform closest = null;
            float minDistance = Mathf.Infinity;

            
            foreach(Collider enemy in enemies)
            {
                // 가까운 대상 거리 계산
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = enemy.transform;
                }
            }
            // 타겟 설정
            _targetEnemy = closest;
        }
        else
        {
            Debug.Log("타겟 없음");
            _targetEnemy = null;
        }
    }

    // 이동 위치 탐색
    void MoveToTarget()
    {
        LookAtTarget();
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkSpeed * Time.deltaTime);

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

    void StopAttack()
    {
        _isMoving = false;
        LookAtTarget();

        /// </summary>
        /// 공격 로직 실행
        /// </summary>
    }
}
