using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ZombieProject/Item/New Heal Item")]
public class ItemHealData : ItemData
{
    public int HealAmount;

    public override void Use()
    {
        Debug.Log($"Player healed with {HealAmount}");
    }
}
