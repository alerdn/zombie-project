using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryContainer
{
    [field: SerializeField] public InventoryContainerData Data { get; private set; }
    public List<InventoryItem> Items { get; private set; } = new();

    public int CurrentCapacity => Items.Count;

    public void AddItem(ItemData item, int amount = 1)
    {
        InventoryItem itemInventory = Items.Find(itemInventory => itemInventory.Data.ItemType == item.ItemType);
        if (!item.IsStackable || (item.IsStackable && itemInventory == null))
        {
            Items.Add(new InventoryItem(item, 1));
        }
        else
        {
            itemInventory.CurrentQuantity += amount;
        }
    }

    public bool HasAvailableCapacity(ItemData itemData)
    {
        InventoryItem item = Items.Find(item => item.Data.ItemType == itemData.ItemType);

        // Se o item não é empilhável ou é empilhável, mas ainda não está no inventário
        // É necessário que tenha um espaço livre
        if (!itemData.IsStackable || (itemData.IsStackable && item == null))
        {
            return CurrentCapacity < Data.MaxCapacity;
        }

        // Se é empilhável e já está no inventário
        return true;
    }
}
