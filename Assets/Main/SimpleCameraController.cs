using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 10f;       // 이동 속도
    [SerializeField] private float _boostMultiplier = 2.5f; // Shift 누를 때 부스트

    void Update()
    {
        // 1. 이동 로직 (WSAD)
        float currentSpeed = _moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= _boostMultiplier;

        float h = Input.GetAxisRaw("Horizontal"); // A, D
        float v = Input.GetAxisRaw("Vertical");   // W, S

        Vector3 moveDir = (transform.up * v) + (transform.right * h);
        transform.position += moveDir.normalized * currentSpeed * Time.deltaTime;

        // 2. 고도 조절 (Q: 하강, E: 상승)
        if (Input.GetKey(KeyCode.E)) transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) transform.position -= Vector3.forward * currentSpeed * Time.deltaTime;
    }
}