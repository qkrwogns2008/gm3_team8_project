using UnityEngine;

public class GameOver : MonoBehaviour
{
	#region ¿ŒΩ∫∆Â≈Õ
	[SerializeField] private Canvas _gameoverCanvas;
	[SerializeField] private CHeroManager _cheroManager;
	#endregion

	private void OnEnable()
	{
		if (_cheroManager == null)
		{
			if (CHeroManager.Instance != null)
			{
				_cheroManager = CHeroManager.Instance;
			}
			else
			{
				Debug.LogWarning($"[{name}] HeroManager null");
				return;
			}
		}
		_cheroManager.OnAllHeroDead += ActiveDefeatUI;
	}

	private void OnDisable()
	{
		if (_cheroManager == null)
		{
			if (CHeroManager.Instance != null)
			{
				_cheroManager = CHeroManager.Instance;
			}
			else
			{
				Debug.LogWarning($"[{name}] HeroManager null");
				return;
			}
		}
		_cheroManager.OnAllHeroDead -= ActiveDefeatUI;
	}

	public void ActiveDefeatUI()
	{
		if (_gameoverCanvas == null)
		{
			Debug.LogWarning("gameoverCanvas null");
			return;
		}

		_gameoverCanvas.gameObject.SetActive(true);
	}
}