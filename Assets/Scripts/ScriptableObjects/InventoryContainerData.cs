using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ZombieProject/Inventory/New Inventory Container")]
public class InventoryContainerData : ScriptableObject
{
    public ItemCategory ItemCategory;
    public int MaxCapacity;
}
