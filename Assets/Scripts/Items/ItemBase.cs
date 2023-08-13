using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieProject.Core;

public class ItemBase : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    [Header("Setup")]
    [SerializeField] private ParticleSystem mParticleSystem;
    [SerializeField] private float timeToHide = 3;
    [SerializeField] private GameObject graphicItem;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;

    private Collider[] _colliders;

    private void Awake()
    {
        _colliders = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerInventory player = collision.transform.GetComponent<PlayerInventory>();
        if (player)
        {
            Collect(player);
        }
    }

    protected virtual void Collect(PlayerInventory playerInventory)
    {
        if (!playerInventory.AddItem(_itemData))
        {
            Debug.Log($"Não tem mais espaço para {_itemData.ItemType}");
            return;
        }

        graphicItem.SetActive(false);
        foreach (Collider collider in _colliders) collider.enabled = false;

        if (mParticleSystem != null) mParticleSystem.Play();
        if (audioSource != null) audioSource.Play();

        Invoke(nameof(HideObject), timeToHide);
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }
}