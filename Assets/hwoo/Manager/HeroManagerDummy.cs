using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroManagerDummy : MonoBehaviour
{
    public static HeroManagerDummy Instance;

    public List<CUnitBase> ActiveHero = new List<CUnitBase>();

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
        if(!ActiveHero.Contains(hero))
        {
            ActiveHero.Add(hero);
            Debug.Log($"HeroManager {hero.name} 등록");
        }
    }

    public void UnregisterHero(CUnitBase hero)
    {
        if(ActiveHero.Contains(hero))
        {
            ActiveHero.Remove(hero);
            Debug.Log($"HeroManager {hero.name} 제거");
        }
    }
}
