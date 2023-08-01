using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Netcode;
using UnityEngine;

public class ServerSingleton : PersistentSingleton<HostSingleton>
{
    public ServerGameManager GameManager { get; private set; }

    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
        );
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
