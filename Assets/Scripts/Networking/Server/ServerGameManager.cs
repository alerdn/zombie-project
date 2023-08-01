using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;

public class ServerGameManager : IDisposable
{
    private string _serverIP;
    private int _serverPort;
    private int _queryPort;
    private NetworkServer _networkServer;
    private MultiplayAllocationService _multiplayAllocationService;
    private const string GameSceneName = "SCN_Game_Prototype";

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager)
    {
        _serverIP = serverIP;
        _serverPort = serverPort;
        _queryPort = queryPort;
        _networkServer = new NetworkServer(manager);
        _multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartGameServerAsync()
    {
        await _multiplayAllocationService.BeginServerCheck();

        if (!_networkServer.OpenConnection(_serverIP, _serverPort))
        {
            Debug.LogWarning("Network server did not start as expected.");
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    public void Dispose()
    {
        _multiplayAllocationService?.Dispose();
        _networkServer?.Dispose();
    }
}
