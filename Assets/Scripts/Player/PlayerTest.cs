using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : NetworkBehaviour
{
    public GameObject prefab;

    private bool hasSpawned;

    private void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !hasSpawned)
        {
            hasSpawned = true;
            SpawnThingServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnThingServerRpc()
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = transform.position;

        SpawnThingClientRpc();
    }

    [ClientRpc]
    private void SpawnThingClientRpc()
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = transform.position;
    }
}
