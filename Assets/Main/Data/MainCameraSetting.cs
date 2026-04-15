using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MainCameraSetting : MonoBehaviour
{
    public Transform target;      // 카메라 기준 Transform
    public Vector3 offset = new Vector3(0, 0 , -350); // 타겟과 카메라 사이의 거리
    public float smoothTime = 0.1f; // 따라가는 부드러움 정도 (1에 가까울 수록 빠름)

    private void LateUpdate()
    {
        if (target == null)
        {
            SetCameraTarget();
            return;
        }
        Vector3 camPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, camPosition, smoothTime);
    }

    // 외부에서 추적 대상을 바꿀 때 사용
    public Transform SetCameraTarget()
    {
        int i = CDataManager.Instance.UserData.Hero_Camera_Array;

        EHeroID targetHeroID = (EHeroID)CDataManager.Instance.UserData.Hero_Array[i];
        if (targetHeroID == EHeroID.None)
        {
            Debug.Log("카메라 타겟이 없습니다.");
            return target;
        }
        CHero[] allHeroes = Object.FindObjectsByType<CHero>(FindObjectsSortMode.None);

        foreach (CHero hero in allHeroes)
        {
        
            if (hero.HeroID == targetHeroID)
            {
                target = hero.transform;
                Debug.Log($"카메라 타겟 변경 완료: {targetHeroID}");
                return target;
            }
            
        }

        return target;
    }


}