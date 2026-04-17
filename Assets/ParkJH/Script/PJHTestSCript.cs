using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PJHTestSCript : MonoBehaviour
{
    void Start()
    {
            StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeInOutRoutine()
    {
        yield return new WaitForSeconds(1f); if (MainNotification.Instance != null)
        {
            MainNotification.Instance.StartMainNotification("안녕");
        }
        else
        {
            Debug.LogError("MainNotification 인스턴스를 찾을 수 없습니다!");
        }

    }

}