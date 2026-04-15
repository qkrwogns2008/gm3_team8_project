using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTargetScanner : MonoBehaviour
{
	#region 인스펙터
	[Header("참조")]
	[SerializeField] private MainCameraSetting _cameraSetting;

	[Header("스캔 설정")]
	[SerializeField] private float _defaultScanRange = 15f;
	#endregion

	public CUnitBase ScanTargetFromList()
	{
		// 카메라 주인공 찾기
		Transform leader = _cameraSetting.target;
		if(leader == null)
		{
			return null;
		}

		// 리스트가 비어있는지 확인
		if(CEnemyManager.Instance == null || CEnemyManager.Instance.ActiveEnemies.Count == 0 )
		{
			return null;
		}

		CUnitBase closestEnemy = null;
		float minSqrDistance = _defaultScanRange * _defaultScanRange;

		// 리스트 순회
		foreach(CUnitBase enemy in CEnemyManager.Instance.ActiveEnemies)
		{
			if(enemy == null || !enemy.gameObject.activeSelf || enemy.IsUnitDead)
			{
				continue;
			}

			float sqrDist = (enemy.transform.position - leader.position).sqrMagnitude;

			if(sqrDist < minSqrDistance)
			{
				minSqrDistance = sqrDist;
				closestEnemy = enemy;
			}
		}

		return closestEnemy;
	}
}
