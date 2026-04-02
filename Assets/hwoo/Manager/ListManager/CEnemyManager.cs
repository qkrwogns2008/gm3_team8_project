using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CEnemyManager Instance;

    private List<CUnitBase> _activeEnemies = new List<CUnitBase>();

    public IReadOnlyList<CUnitBase> ActiveEnemies => _activeEnemies;

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
        if(!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
        }
    }
    public void UnregisterEnemy(CUnitBase enemy)
    {
        if(_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }
    }
}
