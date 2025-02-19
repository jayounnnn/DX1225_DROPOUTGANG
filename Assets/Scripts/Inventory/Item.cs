using UnityEngine;

public enum ItemType
{
    Torch,
    Consumable,
    QuestItem
}

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
}
