using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManagerDummy : MonoBehaviour
{
    public static HeroManagerDummy Instance;
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
            Debug.Log($"HeroManager {hero.name} 등록");
        }
		OnHeroActived?.Invoke(hero);
	}

    public void UnregisterHero(CUnitBase hero)
    {
        if(_activeHero.Contains(hero))
        {
            _activeHero.Remove(hero);
            Debug.Log($"HeroManager {hero.name} 제거");
        }
    }
}
