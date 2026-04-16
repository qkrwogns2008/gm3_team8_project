using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTargetScanner : MonoBehaviour
{
	#region 인스펙터
	[Header("참조")]
	[SerializeField] private MainCameraSetting _cameraSetting;

	[Header("HeroSO")]
	[SerializeField] private HeroDataSO _targetDataSO;
	#endregion

	public CUnitBase ScanTargetFromList()
	{
        // 카메라 주인공 찾기
        Transform leader = _cameraSetting.target;
		if(leader == null)
		{
            return null;
		}

		// SO에서 탐지범위 가져오기
		float scanRange = _targetDataSO.DetectionRange;

		// 리스트가 비어있는지 확인
		if(CEnemyManager.Instance == null || CEnemyManager.Instance.ActiveEnemies.Count == 0 )
		{
			return null;
		}

		CUnitBase closestEnemy = null;
		float minSqrDistance = scanRange * scanRange;

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
        if (closestEnemy != null)
        {
            Debug.Log($"<color=green>2. 스캐너: 적 발견! -> {closestEnemy.name}</color>");
        }
        else
        {
            Debug.Log("<color=yellow>주의: 주변에 적이 없음 (Range 확인 필요)</color>");
        }
        return closestEnemy;
	}
}
