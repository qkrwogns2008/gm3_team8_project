using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHeroManager : MonoBehaviour
{
    public static CHeroManager Instance;
	public event Action<CUnitBase> OnHeroActived;

    private List<CUnitBase> _activeHero = new List<CUnitBase>();

    public IReadOnlyList<CUnitBase> ActiveHero => _activeHero;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RegisterHero(CUnitBase hero)
    {
        if(!_activeHero.Contains(hero))
        {
            _activeHero.Add(hero);
            Debug.Log($"CHeroManager {hero.name} 등록");
        }
		OnHeroActived?.Invoke(hero);
	}

    public void UnregisterHero(CUnitBase hero)
    {
        if(_activeHero.Contains(hero))
        {
            _activeHero.Remove(hero);
            Debug.Log($"CHeroManager {hero.name} 제거");
        }
    }

	/// <summary>
	/// 모든 영웅의 스탯 상태를 갱신함.
	/// </summary>
	public void RefreshUpgradeStatAllHero()
	{
		for (int i = 0; i < _activeHero.Count; i++)
		{
			CHero hero = _activeHero[i] as CHero;
			if (hero != null)
			{
				hero.RefreshUpgrade();
			}
		}
	}

	/// <summary>
	/// 특정 ID의 영웅 스탯 상태를 갱신함.
	/// </summary>
	/// <param name="id"></param>
	public void RefreshUpgradeStat(EHeroID id)
	{
		for (int i = 0; i < _activeHero.Count; i++)
		{
			CHero hero = _activeHero[i] as CHero;
			if (hero != null)
			{
				if (hero.HeroID == id)
				{
					hero.RefreshUpgrade();
					return;
				}
			}
		}
	}
}
