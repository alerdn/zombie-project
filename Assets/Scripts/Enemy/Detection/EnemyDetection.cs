using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DetectionType
{
    SOUND,
    SIGHT
}

public class EnemyDetection : NetworkBehaviour
{
    public event Action<Player> OnSpotPlayer;

    public bool HasSpottedPlayer { get; private set; }

    [field: SerializeField] public DetectionType DetectionType { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            HasSpottedPlayer = HandleNoticeSomething(player);
            if (HasSpottedPlayer)
            {
                OnSpotPlayer?.Invoke(player);
            }
        }
    }

    private bool HandleNoticeSomething(Player player)
    {
        return DetectionType switch
        {
            DetectionType.SOUND => HandleSoundDetection(player),
            DetectionType.SIGHT => HandleSightDetection(player),
            _ => true,
        };
    }

    private bool HandleSoundDetection(Player player)
    {
        Debug.Log($"ouvindo {player.IsShooting.Value}");
        return player.IsShooting.Value;
    }

    private bool HandleSightDetection(Player player)
    {
        if (Physics.Raycast(transform.position, (transform.position - player.transform.position) * -1, out RaycastHit hit))
        {
            return hit.collider.CompareTag("Player");
        }
        else
        {
            return false;
        }
    }
}
