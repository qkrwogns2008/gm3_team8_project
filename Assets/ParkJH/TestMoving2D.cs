using UnityEngine;

public class TestMoving2D : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    void Update()
    {
        // WASD 또는 화살표 입력을 받습니다 (-1.0 ~ 1.0)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 이동 방향 벡터 생성 및 정규화 (대각선 이동 속도 일정하게)
        Vector3 moveDir = new Vector3(h, v, 0).normalized;

        // 프레임 독립적인 이동 처리
        transform.Translate(moveDir * _moveSpeed * Time.deltaTime);
    }
}