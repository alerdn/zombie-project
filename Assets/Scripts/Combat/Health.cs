using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public event Action<Health> OnDie;

    [SerializeField] private GameObject _deatheEffect;

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public int _currentHealth;

    private bool _isDead;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _currentHealth = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealthServerRpc(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealthServerRpc(healValue);
    }

    [ServerRpc]
    private void ModifyHealthServerRpc(int value)
    {
        if (_isDead) return;

        int newHealth = _currentHealth + value;
        _currentHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
        SetHealthClientRpc(_currentHealth);

        if (_currentHealth == 0)
        {
            _isDead = true;
            KillClientRpc();
            Destroy(gameObject);
        }
    }

    // Deixar que o servidor calcule a nova health e os clientes apenas settem
    [ClientRpc]
    private void SetHealthClientRpc(int currentHealth)
    {
        _currentHealth = currentHealth;
    }

    // Apenas o servidor pode dizer ao client se alguém morreu
    [ClientRpc]
    private void KillClientRpc()
    {
        Instantiate(_deatheEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
