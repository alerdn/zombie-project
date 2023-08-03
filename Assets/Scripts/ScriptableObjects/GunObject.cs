using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gun", menuName = "Gun/GunObject")]
public class GunObject : ScriptableObject
{
    public Sprite Crosshair;
    public bool IsAutomatic, HasCrosshair;
    public float TimeBetweenShot;
    public float AdsZoom;
    public int Ammo;
    public int TotalAmmo;
    public int ShotDamage;
}
