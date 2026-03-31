using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CEnemyManager Instance;

    public List<CUnitBase> ActiveEnemies = new List<CUnitBase>();

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

    public void RegisterEnemy(CUnitBase enemy)
    {
        if(!ActiveEnemies.Contains(enemy))
        {
            ActiveEnemies.Add(enemy);
        }
    }
    public void UnregisterEnemy(CUnitBase enemy)
    {
        if(ActiveEnemies.Contains(enemy))
        {
            ActiveEnemies.Remove(enemy);
        }
    }
}
