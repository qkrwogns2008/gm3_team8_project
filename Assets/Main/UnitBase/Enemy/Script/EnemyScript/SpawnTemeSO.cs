using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnTheme", menuName = "ScriptableObjects/SpawnTheme(SO)")]
public class SpawnTemeSO : ScriptableObject
{
    public string themeName;
    public List<GameObject> monsterPrefabs;
}
