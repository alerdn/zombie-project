using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ZombieProject.Core;

public class Gun : NetworkBehaviour
{
    public Sprite Crosshair;
    public bool IsAutomatic, HasCrosshair;
    public float TimeBetweenShot;
    public float AdsZoom;
    public int Ammo;
    public int TotalAmmo;
    public int ShotDamage;
    public AudioSource ShotAudio;
    public GameObject MuzzleFlash;

    [SerializeField] private GunObject _gunObject;

    private void Start()
    {
        TimeBetweenShot = _gunObject.TimeBetweenShot;
        IsAutomatic = _gunObject.IsAutomatic;
        HasCrosshair = _gunObject.HasCrosshair;
        Crosshair = _gunObject.Crosshair;
        AdsZoom = _gunObject.AdsZoom;
        Ammo = _gunObject.TotalAmmo;
        TotalAmmo = _gunObject.TotalAmmo;
        ShotDamage = _gunObject.ShotDamage;
    }

    public virtual void Shoot(Camera _cam, GameObject bullet)
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject bulletInstance = Instantiate(bullet, hit.point, Quaternion.identity);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<Health>().TakeDamage(ShotDamage);
            }
            Destroy(bulletInstance, 2f);
        }


    }
}
