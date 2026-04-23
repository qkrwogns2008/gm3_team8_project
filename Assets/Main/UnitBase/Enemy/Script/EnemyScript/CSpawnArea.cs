using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CSpawnArea : MonoBehaviour
{
	#region 인스펙터

	[System.Serializable]
	struct SpawnPointData
	{
		public Transform point;
		public float range;
		public float respawnTime;
	}

	[Header("테마")]
	[SerializeField] private string _themeName;
	[Header("몬스터 리스트SO")]
	[SerializeField] private SpawnTemeSO _currentTheme;
	[Header("현 라운드 출현 몬스터(자동 선택)")]
	[SerializeField] private GameObject[] _activeMonsters = new GameObject[3];
	[Header("스폰 구역")]
	[SerializeField] SpawnPointData[] _spawnPoints = new SpawnPointData[3];
	[Header("스폰 설정")]
	[SerializeField] private int _maxMonsterCount = 10;
	[SerializeField] private int _currentMonsterCount = 0;

	#endregion

	private List<GameObject> _spawnedEnemy = new List<GameObject>();
	private Coroutine[] _respawnCoroutines = new Coroutine[3];

	private void OnEnable()
	{
		if(CBossSpawner.IsBossMode)
		{
			gameObject.SetActive(false);
			return;
		}
		if (_spawnPoints.Length != 3)
		{
			Debug.Log("Spawn Point가 정확히 3개가 아님");
			return;
		}
		// 몬스터 3종 선별
		SelectRoundMonsters();

		for (int i = 0; i < _maxMonsterCount; i++)
		{
			SpawnMonsterAtPoint(i % 3);
		}
    }

	// 몬스터 3마리 뽑기
	void SelectRoundMonsters()
	{
		List<GameObject> sourceList = _currentTheme.monsterPrefabs;

		if(sourceList.Count < 3)
		{
			return;
		}

		List<GameObject> tempList = new List<GameObject>(sourceList);

		for(int i = 0; i < 3; i++)
		{
			int randIndex = Random.Range(0, tempList.Count);
            _activeMonsters[i] = tempList[randIndex];
			tempList.RemoveAt(randIndex);
		}
		Debug.Log("포인트별 몬스터 배정 완료");
	}


	GameObject SpawnMonsterAtPoint(int index)
	{
		GameObject prefab = _activeMonsters[index];
		SpawnPointData data = _spawnPoints[index];

		Vector2 randomOffset = Random.insideUnitCircle * data.range;
		Vector3 spawnPos = data.point.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

		Debug.Log($"1");

		GameObject obj = PoolManager.Instance.Pop(prefab, spawnPos, Quaternion.identity);

        Debug.Log($"2");

        _currentMonsterCount++;

		if(!_spawnedEnemy.Contains(obj))
		{
			_spawnedEnemy.Add(obj);
		}

		CEnemyBase enemy = obj.GetComponent<CEnemyBase>();
		if(enemy != null)
		{
			enemy.InitSpawn(this, index);
		}

		return obj;
	}

	public void OnMonsterDeath(int index, GameObject monsterObj)
	{
		_currentMonsterCount --;

		if(_spawnedEnemy.Contains(monsterObj))
		{
			_spawnedEnemy.Remove(monsterObj);
		}

		StartCoroutine(RespawnTimer(index));
	}

	public void OnMonsterDeath(int index)
	{
		_currentMonsterCount--;
		StartCoroutine(RespawnTimer(index));
	}

	private IEnumerator RespawnTimer(int index)
	{
		yield return new WaitForSeconds(_spawnPoints[index].respawnTime);

		if(_currentMonsterCount < _maxMonsterCount)
		{
			Debug.Log("3");
			SpawnMonsterAtPoint(index);
			Debug.Log("4");
		}
		Debug.Log("5");
	}

	// 스테이지 종료 혹은 스포너 정지 필요시 호출.
	public void StopSpawning()
	{
		for(int i = 0; i < _respawnCoroutines.Length; i++)
		{
			if (_respawnCoroutines[i] != null)
			{
				StopCoroutine(_respawnCoroutines[i]);
				_respawnCoroutines[i] = null;
			}
		}
	}
	#region 스테이지 이동 관련
	
	// 모든 몬스터 지우고 스폰 중단
	public void ClearAllMonsters()
	{
		// 리스폰 타이머 코루틴 모두 정지
		StopSpawning();

		// 리스트 역순 순회 모든 몬스터 Pool로 반환
		for(int i = _spawnedEnemy.Count - 1; i >= 0; i--)
		{
			GameObject obj = _spawnedEnemy[i];
			if(obj != null)
			{
				CEnemyBase enemy = obj.GetComponent<CEnemyBase>();
				if(enemy != null && enemy.OriginPrefab != null)
				{
					PoolManager.Instance.Push(enemy.OriginPrefab, obj);
				}
				else
				{
					obj.SetActive(false);
				}
			}
		}

		_spawnedEnemy.Clear();
		_currentMonsterCount = 0;

	}

	// 스포너 재실행
	public void ReStartStage()
	{
		ClearAllMonsters();

		SelectRoundMonsters();

		for(int i = 0; i < _maxMonsterCount; i++)
		{
			Debug.Log("6");
			SpawnMonsterAtPoint(i % 3);
		}
		Debug.Log("7");
	}
	
	// 테마 변경시 SO를 주입하고 재시작 할 때 사용
	public void ChangeTheme(SpawnTemeSO newTheme, string newName)
	{
		_currentTheme = newTheme;
		_themeName = newName;
		ReStartStage();
	}
	#endregion
	#region 기즈모
	private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (var sp in _spawnPoints)
        {
            if (sp.point != null)
            {
                Gizmos.DrawWireSphere(sp.point.position, sp.range);
            }
        }
    }
    #endregion

}
