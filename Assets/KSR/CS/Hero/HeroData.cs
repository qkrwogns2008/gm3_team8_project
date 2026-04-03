using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Hero/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("기본 정보")]
    public string heroName;

    public HeroType heroType;

    [Header("식별")]
    public int heroID;

    [Header("능력치")]
    public int attack;
    public int defense;
    public int life;
}

public enum HeroType
{
    Melee,
    Ranged,
    Magic,
    Tank
}