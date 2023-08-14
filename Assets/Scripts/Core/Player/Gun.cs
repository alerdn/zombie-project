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

    private GameObject _bulletPrefab;

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

        _bulletPrefab = _gunObject.Bullet;
    }

    public virtual void Shoot(Vector3 viewPointPosition, Vector3 viewPointForward)
    {
        // Apenas instancia gráfico
        if (Physics.Raycast(viewPointPosition, viewPointForward, out RaycastHit hit))
        {
            ShootGraphic(hit.point);
        }

        // Efetua cálculos no servidor
        ShootServerRpc(viewPointPosition, viewPointForward);
    }

    [ServerRpc]
    public virtual void ShootServerRpc(Vector3 viewPointPosition, Vector3 viewPointForward)
    {
        if (Physics.Raycast(viewPointPosition, viewPointForward, out RaycastHit hit))
        {
            ShootGraphicClientRpc(hit.point);

            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Health>().TakeDamage(ShotDamage);
            }
        }
    }

    private void ShootGraphic(Vector3 hitPoint)
    {
        GameObject bulletInstance = Instantiate(_bulletPrefab, hitPoint, Quaternion.identity);
        Destroy(bulletInstance, 2f);
    }

    [ClientRpc]
    private void ShootGraphicClientRpc(Vector3 hitPoint)
    {
        ShootGraphic(hitPoint);
    }
}
