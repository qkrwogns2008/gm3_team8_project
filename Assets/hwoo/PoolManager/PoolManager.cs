using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 프리팹 새로 만들기
        if (!_pools.ContainsKey(prefab))
        {
            _pools.Add(prefab, new Queue<GameObject>());
        }

        GameObject obj = null;

        if (_pools[prefab].Count > 0)
        {
            obj = _pools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
        }
        return obj;
    }

    public void Push(GameObject prefab, GameObject obj)
    {
        if(obj == null)
        {
            return;
        }
        obj.SetActive(false);
        if(!_pools.ContainsKey(prefab))
        {
            _pools.Add(prefab, new Queue<GameObject>());
        }
        _pools[prefab].Enqueue(obj);
    }
}
