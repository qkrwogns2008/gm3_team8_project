using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<EffectBase, Queue<EffectBase>> _effectPools = new Dictionary<EffectBase, Queue<EffectBase>>(); // EffectBase용 Pool
	private Dictionary<MissileBase, Queue<MissileBase>> _missilePools = new Dictionary<MissileBase, Queue<MissileBase>>(); // MissileBase용 Pool

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

	#region EffectBase Push / POP
	// 오버로딩 : EffectBase 타입으로 오브젝트 Pop
	public EffectBase Pop(EffectBase prefab, Vector3 position, Quaternion rotation)
	{
		if (!_effectPools.ContainsKey(prefab))
		{
			_effectPools.Add(prefab, new Queue<EffectBase>());
		}

		EffectBase obj = null;

		if (_effectPools[prefab].Count > 0)
		{
			obj = _effectPools[prefab].Dequeue();
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.gameObject.SetActive(true);
		}
		else
		{
			obj = Instantiate(prefab, position, rotation);
		}
		return obj;
	}

	// 오버로딩 : EffectBase 타입으로 오브젝트 Push
	public void Push(EffectBase prefab, EffectBase obj)
	{
		if (obj == null)
		{
			return;
		}
		obj.gameObject.SetActive(false);
		if (!_effectPools.ContainsKey(prefab))
		{
			_effectPools.Add(prefab, new Queue<EffectBase>());
		}
		_effectPools[prefab].Enqueue(obj);
	}
	#endregion

	#region MissileBase Push / POP
	// 오버로딩 : MissileBase 타입으로 오브젝트 Pop
	public MissileBase Pop(MissileBase prefab, Vector3 position, Quaternion rotation)
	{
		if (!_missilePools.ContainsKey(prefab))
		{
			_missilePools.Add(prefab, new Queue<MissileBase>());
		}

		MissileBase obj = null;

		if (_missilePools[prefab].Count > 0)
		{
			obj = _missilePools[prefab].Dequeue();
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.gameObject.SetActive(true);
		}
		else
		{
			obj = Instantiate(prefab, position, rotation);
		}
		return obj;
	}

	// 오버로딩 : MissileBase 타입으로 오브젝트 Push
	public void Push(MissileBase prefab, MissileBase obj)
	{
		if (obj == null)
		{
			return;
		}
		obj.gameObject.SetActive(false);
		if (!_missilePools.ContainsKey(prefab))
		{
			_missilePools.Add(prefab, new Queue<MissileBase>());
		}
		_missilePools[prefab].Enqueue(obj);
	}
	#endregion
}
