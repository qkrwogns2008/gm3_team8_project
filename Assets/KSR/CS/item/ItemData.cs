using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;

    public ItemType itemType;

    [Header("식별")]
    public int itemID;

    [Header("기능 수치")]
    public int value;
}

public enum ItemType
{
    Box,
    Card,
    Gold,
    Food,
    Relic,
    Exp
}