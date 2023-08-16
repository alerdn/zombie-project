using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public event Action<float> OnHit;

    [SerializeField] private GameObject _deatheEffect;

    [field: SerializeField] public float MaxHealth { get; private set; } = 100;
    public float CurrentHealth { get; private set; }

    private bool _isDead;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damageValue)
    {
        ModifyHealthServerRpc(-damageValue);
    }

    public void RestoreHealth(float healValue)
    {
        ModifyHealthServerRpc(healValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ModifyHealthServerRpc(float value)
    {
        if (_isDead) return;

        float newHealth = CurrentHealth + value;
        CurrentHealth = Mathf.Clamp(newHealth, 0f, MaxHealth);
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
    private void SetHealthClientRpc(float currentHealth)
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
