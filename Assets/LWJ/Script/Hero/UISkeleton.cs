using Spine.Unity;
using UnityEngine;

public class UISkeleton : MonoBehaviour
{
	[SerializeField] private SkeletonAnimation SkeletonAni;
	[SerializeField] private AnimationReferenceAsset[] clip;

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // 예시. << 이 코드는 아니고 버튼 클릭을 감지해야 함.
		{
			PlayRandomAnimation();
		}
	}

	private void PlayRandomAnimation()
	{
		if (SkeletonAni == null)
		{
			Debug.LogWarning($"{name} 인스펙터 null");
			return;
		}

		if (clip == null || clip.Length == 0)
		{
			return;
		}

		int index = Random.Range(0, clip.Length);
		SkeletonAni.AnimationState.SetAnimation(0, clip[index], false);
		SkeletonAni.AnimationState.AddAnimation(0, "Idle", true, 0);
	}
}