using System.Collections;
using System.Collections.Generic;
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
	[Header("몬스터 리스트")]
	[SerializeField] private List<GameObject> _monsterPrefabs;
	[Header("현 라운드 출현 몬스터(자동 선택)")]
	[SerializeField] private GameObject[] _activeMonsters = new GameObject[3];
	[Header("스폰 구역")]
	[SerializeField] SpawnPointData[] _spawnPoints = new SpawnPointData[3];
	[Header("스폰 설정")]
	[SerializeField] private int _maxMonsterCount = 10;
	[SerializeField] private int _currentMonsterCount = 0;

	#endregion


	private void Start()
	{
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
		if(_monsterPrefabs.Count < 3)
		{
			return;
		}

		List<GameObject> tempList = new List<GameObject>(_monsterPrefabs);

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

		GameObject obj = PoolManager.Instance.Pop(prefab, spawnPos, Quaternion.identity);

		_currentMonsterCount++;

		CEnemyBase enemy = obj.GetComponent<CEnemyBase>();
		if(enemy != null)
		{
			enemy.InitSpawn(this, index);
		}

		return obj;
	}

	public void OnMonsterDeath(int index)
	{
		_currentMonsterCount --;

		StartCoroutine(RespawnTimer(index));
	}

	private IEnumerator RespawnTimer(int index)
	{
		yield return new WaitForSeconds(_spawnPoints[index].respawnTime);

		if(_currentMonsterCount < _maxMonsterCount)
		{
			SpawnMonsterAtPoint(index);
		}
	}

	// 스테이지 종료 혹은 스포너 정지 필요시 호출.
	public void StopSpawning()
	{
		StopAllCoroutines();
	}

    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.blue;
		foreach(var sp in _spawnPoints)
		{
			if(sp.point != null)
			{
				Gizmos.DrawWireSphere(sp.point.position, sp.range);
			}
		}
    }
}
