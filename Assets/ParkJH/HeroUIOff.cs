using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroUIOff : MonoBehaviour
{
    private void OnDisable()
    {
        if (CDataManager.Instance.UserData.IsHeroArrayChanged)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            CDataManager.Instance.SaveUserData(); // 변경된 데이터 저장
            SceneManager.LoadScene(currentScene.name);
            
        }
    }
}
