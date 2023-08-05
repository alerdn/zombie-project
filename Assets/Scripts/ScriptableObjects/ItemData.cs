using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ZombieProject/Item/New Item")]
public class ItemData : ScriptableObject
{
    public ItemCategory ItemCategory;
    public ItemType ItemType;
    public Sprite Sprite;
    public bool IsStackable;
}
