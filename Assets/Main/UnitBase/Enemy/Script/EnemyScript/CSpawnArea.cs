using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
	[SerializeField] private float _spawnInterval = 5.0f;
	[SerializeField] private int _maxMonsterCount = 10;
	[SerializeField] private int _currentMonsterCount = 0;

	#endregion

	private Coroutine _spawnRoutine;

    private void Start()
    {
        if(_spawnPoints.Length != 3)
		{
			Debug.Log("Spawn Point가 정확히 3개가 아님");
			return;
		}
        SelectRoundMonsters();

		_spawnRoutine = StartCoroutine(SpawnMonsterRoutine());
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

	private IEnumerator SpawnMonsterRoutine()
	{
		yield return new WaitForSeconds(1.0f);

		while (true)
		{
			SpawnRandomMonster();
			yield return new WaitForSeconds(_spawnInterval);
		}
	}


	// 3개의 포인트중 하나 골라 랜덤으로 소환
	void SpawnRandomMonster()
	{
		if(_currentMonsterCount >= _maxMonsterCount)
		{
			return;
		}

		int index = Random.Range(0, 3);

		GameObject prefab = _activeMonsters[index];
		SpawnPointData data = _spawnPoints[index];

		Vector2 randomOffset = Random.insideUnitCircle * data.range;
		Vector3 spawnPos = data.point.position + new Vector3(randomOffset.x, randomOffset.y, 0);

		GameObject obj = PoolManager.Instance.Pop(prefab, spawnPos, Quaternion.identity);

		_currentMonsterCount++;
	}

	public void OnMonsterDeath()
	{
		_currentMonsterCount --;
	}

	// 스테이지 종료 혹은 스포너 정지 필요시 호출.
	public void StopSpawning()
	{
		if(_spawnRoutine != null)
		{
			StopCoroutine(_spawnRoutine);
			_spawnRoutine = null;
		}
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
