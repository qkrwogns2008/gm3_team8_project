using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CDropItem : MonoBehaviour
{
	#region 인스펙터
	[Header("설정")]
	[SerializeField] private GameObject _originPrefab;
	[SerializeField] private float _bounceHeight = 1.5f;
	[SerializeField] private float _flySpeed = 10f;
	#endregion

	private Vector3 _uiTargetWorldPos;

    private void OnEnable()
    {
		_uiTargetWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, 10f));

		StartCoroutine(Co_ItemRoutine());
    }
	private IEnumerator Co_ItemRoutine()
	{
		Vector3 startPos = transform.position;

		// 튕기기
		float elapsed = 0f;
		float duration = 0.5f;
		Vector3 bouncePos = startPos + new Vector3(Random.Range(-1f, 1f), 0f, 0f);

		while(elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / duration;

			float yOffset = Mathf.Sin(t * Mathf.PI) * _bounceHeight;
			transform.position = Vector3.Lerp(startPos, bouncePos, t) + new Vector3(0, yOffset, 0);

			yield return null;
		}

		// 잠시 대기
		yield return new WaitForSeconds(0.3f);

		// UI로 날아가기
		while(Vector3.Distance(transform.position, _uiTargetWorldPos)> 0.5f)
		{
			transform.position = Vector3.MoveTowards(transform.position, _uiTargetWorldPos, _flySpeed * Time.deltaTime);
			_flySpeed += 0.5f;

			yield return null;
		}

		if(_originPrefab != null)
		{
			PoolManager.Instance.Push(_originPrefab, gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
