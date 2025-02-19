using UnityEngine;

public enum ItemType
{
    Torch,
    Consumable,
    QuestItem,
    Battery
}

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    [HideInInspector]
    public float batteryLife = 100f;
}
