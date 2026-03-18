using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAutoEnemyMove : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] float _walkspeed = 2f;             // 이동속도 (임시) 이후 외부에서 받아올것
    [SerializeField] private float _walkRadius = 20f;   // 주변 돌아다니는 범위
    [SerializeField] private float _walkTimer = 3f;     // 대기시간
    #endregion
    #region 내부변수
    private Vector3 homePosition;                       // 처음 스폰한 위치
    private Vector3 targetPos;                          // 타겟 위치
    private bool isMoving = false;                      // 이동 상태 확인
    private float timer = 0f;                           // 대기시간 타이머

    private SpriteRenderer sprite;
    #endregion

    void Start()
    {
        // 현재 위치 세팅
        homePosition = transform.position;
        sprite = GetComponent<SpriteRenderer>();
        // 이동할 위치 선택
        SetNewTarget();
    }
    void Update()
    {
        if(isMoving)
        {
            MoveToTarget();
        }
        else
        {
            WaitAtPosiiton();
        }
    }
    
    // 이동 위치 탐색
    /// <summary>
    /// 플레이어가 주변에 들어올시 타겟방향 수정 필요
    /// </summary>
    void MoveToTarget()
    {
        LookAtTarget();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _walkspeed * Time.deltaTime);
        
        // 목표지점.x - 현재 위치.x(절대값) < 0.005f 경우 정지
        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.005f)
        {
            isMoving = false;
            timer = 0f;
        }
    }
    
    // 이동 위치 바라보기
    /// <summary>
    /// 기본 캐릭터 방향이 오른쪽일 경우 그대로
    /// 왼쪽을 바라볼 경우 반대로 뒤집기
    /// </summary>
    void LookAtTarget()
    {
        // 오른쪽
        if(targetPos.x > transform.position.x)
        {
            if(sprite != null)
            {
                // 뒤집기
                sprite.flipX = true;
            }
        }
        // 왼쪽
        else if (targetPos.x < transform.position.x)
        {
            if(sprite != null)
            {
                // 그대로
                sprite.flipX = false;
            }
        }

    }
    
    // 이동 후 대기
    void WaitAtPosiiton()
    {
        timer = Time.deltaTime;
        if (timer >= _walkTimer)
        {
            SetNewTarget();
        }
    }

    // 새로운 위치 탐색
    void SetNewTarget()
    {
        float randX = Random.Range(-_walkRadius, _walkRadius);
        targetPos = new Vector3(homePosition.x + randX, homePosition.y, homePosition.z);
        isMoving = true;
    }

}
