using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLootItem : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private float _flyDuration = 0.8f; // 날아가는 시간
	[SerializeField] private AnimationCurve _flyCuve; // 포물선 커브
	#endregion

	#region 내부변수
	private GameObject _originPrefab;
	#endregion

	public void SetupLootFly(GameObject origin, Vector3 startPos, RectTransform target, Camera mainCam)
	{
		_originPrefab = origin;
		transform.position = startPos;

		Vector3 targetWorldPos = mainCam.ScreenToWorldPoint(new Vector3(
			target.position.x,
			target.position.y,
			mainCam.nearClipPlane + 10f));

		targetWorldPos.z = 0f;

		StartCoroutine(CoFly(startPos, targetWorldPos));
	}

	private IEnumerator CoFly(Vector3 start, Vector3 end)
	{
		float elapsed = 0f;

		Vector3 midPoint = start + Vector3.up * 2f + (Vector3)Random.insideUnitCircle * 1.5f;

		while(elapsed < _flyDuration)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / _flyDuration;

			Vector3 m1 = Vector3.Lerp(start, midPoint, t);
			Vector3 m2 = Vector3.Lerp(midPoint, end, t);
			transform.position = Vector3.Lerp(m1, m2, t);

			transform.localScale = Vector3.one * Mathf.Lerp(1.2f, 0.4f, t);

			yield return null;
		}
		PoolManager.Instance.Push(_originPrefab, gameObject);
	}
}
