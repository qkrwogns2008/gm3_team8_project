using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpawnerController : MonoBehaviour
{
    public static CSpawnerController Instance { get; private set; }

    #region 인스펙터
    [Header("스포너 목록")]
    [SerializeField] private List<GameObject> _spawners;

    #endregion
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(CBossSpawner.IsBossMode)
        {
            ToggleSpawners(false);
        }
        else
        {
            ToggleSpawners(true);
        }
    }


    private void ToggleSpawners(bool isActive)
    {
        foreach(var spawner in _spawners)
        {
            if(spawner != null)
            {
                spawner.SetActive(isActive);
            }
        }
    }
    
}
