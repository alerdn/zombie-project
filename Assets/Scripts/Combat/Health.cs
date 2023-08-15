using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public event Action<int> OnHit;

    [SerializeField] private GameObject _deatheEffect;

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public int CurrentHealth { get; private set; }

    private bool _isDead;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealthServerRpc(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealthServerRpc(healValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ModifyHealthServerRpc(int value)
    {
        if (_isDead) return;

        int newHealth = CurrentHealth + value;
        CurrentHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
        SetHealthClientRpc(CurrentHealth);

        if (CurrentHealth == 0)
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
        CurrentHealth = currentHealth;
        OnHit?.Invoke(CurrentHealth);
    }

    // Apenas o servidor pode dizer ao client se alguém morreu
    [ClientRpc]
    private void KillClientRpc()
    {
        Instantiate(_deatheEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
