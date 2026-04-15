using UnityEngine;

public class MainCameraSetting : MonoBehaviour
{
    public Transform target;      // 카메라 기준 Transform
    public Vector3 offset = new Vector3(0, 0 , -350); // 타겟과 카메라 사이의 거리
    public float smoothTime = 0.008f; // 따라가는 부드러움 정도 (낮을수록 빠름)

    private Vector3 _currentVelocity = Vector3.zero;
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 camPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, camPosition, smoothTime);
    }

    // 외부에서 추적 대상을 바꿀 때 사용
    public void SetCameraTarget(Transform newTarget)
    {
        target = newTarget;
    }


}