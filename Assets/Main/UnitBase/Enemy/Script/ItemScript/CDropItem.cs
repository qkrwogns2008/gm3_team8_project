using System.Collections;
using UnityEngine;

public class CDropItem : MonoBehaviour
{
	#region 인스펙터
	[Header("설정")]
	[SerializeField] private GameObject _originPrefab;
	[SerializeField] private float _bounceHeight = 1.5f;
	[SerializeField] private float _flySpeed = 10f;
	#endregion

	#region 내부변수

	private float _initialFlySpeed;

    #endregion

    private void Awake()
    {
		_initialFlySpeed = _flySpeed;
    }

	public void Init(GameObject origin)
	{
		_originPrefab = origin;
	}
    private void OnEnable()
    {
		//transform.localScale = Vector3.one;
		_flySpeed = _initialFlySpeed;

		StartCoroutine(Co_ItemRoutine());
    }
	private IEnumerator Co_ItemRoutine()
	{
		Vector3 startPos = transform.position;

		// 주변으로 튕기기
		float elapsed = 0f;
		float duration = 0.5f;

		Vector3 bouncePos = startPos + new Vector3(Random.Range(-1.2f, 1.2f), 0f, 0f);

		while (elapsed < duration)
		{ 
			elapsed += Time.deltaTime;
			float t = elapsed / duration;

			float yOffset = Mathf.Sin(t * Mathf.PI) * _bounceHeight;
			transform.position = Vector3.Lerp(startPos, bouncePos, t);

			yield return null;
		}

		// 잠깐 멈추기
		yield return new WaitForSeconds(0.2f);


		while(true)
		{
			Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
			targetWorldPos.z = 0f;

			float dist = Vector3.Distance(transform.position, targetWorldPos);

			// 목적지에 가까우면 종료
			if(dist < 0.2f)
			{
				break;
			}

			// 목적지로 이동
			transform.position = Vector3.MoveTowards(
				transform.position,
				targetWorldPos,
				_flySpeed * Time.deltaTime
				);
			_flySpeed += 0.8f;

			yield return null;
		}
		// 풀 반납
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
