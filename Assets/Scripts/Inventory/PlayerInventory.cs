using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public List<InventoryContainer> Containers { get; private set; }

    public bool AddItem(ItemData itemData)
    {
        InventoryContainer container = Containers.Find(c => c.Data.ItemCategory == itemData.ItemCategory);
        if (container.HasAvailableCapacity(itemData))
        {
            container.AddItem(itemData);
            return true;
        }

        return false;
    }
}