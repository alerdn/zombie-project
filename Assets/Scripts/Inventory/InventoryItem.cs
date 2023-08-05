using UnityEngine;

public class InventoryItem
{
    public ItemData Data { get; private set; }
    public int CurrentQuantity;

    public InventoryItem(ItemData data, int currentQuantity)
    {
        Data = data;
        CurrentQuantity = currentQuantity;
    }
}