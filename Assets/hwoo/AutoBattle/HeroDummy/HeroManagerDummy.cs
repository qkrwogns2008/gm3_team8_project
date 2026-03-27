using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManagerDummy : MonoBehaviour
{
    public static HeroManagerDummy Instance;

    public List<Transform> ActiveHero = new List<Transform>();

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
    public void RegisterHero(Transform hero)
    {
        if(!ActiveHero.Contains(hero))
        {
            ActiveHero.Add(hero);
            Debug.Log($"HeroManager {hero.name} 등록");
        }
    }

    public void UnregiserHero(Transform hero)
    {
        if(ActiveHero.Contains(hero))
        {
            ActiveHero.Remove(hero);
            Debug.Log($"HeroManager {hero.name} 제거");
        }
    }
}
