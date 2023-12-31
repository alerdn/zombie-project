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

public class HostGameManager : IDisposable
{
    private Allocation _allocation;
    private string _joinCode;
    private string _lobbyId;
    private NetworkServer _networkServer;
    private NetworkObject _playerPrefab;

    public HostGameManager(NetworkObject playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(Constants.MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _joinCode
                    )
                }
            };

            string playerName = PlayerPrefs.GetString(Constants.PlayerNameKey, "Missing Name");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", Constants.MaxConnections, lobbyOptions);
            _lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HearthbeatLobby(15f));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        _networkServer = new NetworkServer(NetworkManager.Singleton, _playerPrefab);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(Constants.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartHost();
        _networkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(Constants.GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HearthbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        if (string.IsNullOrEmpty(_lobbyId)) return;

        HostSingleton.Instance.StopCoroutine(nameof(HearthbeatLobby));

        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        _lobbyId = string.Empty;

        _networkServer.OnClientLeft -= HandleClientLeft;
        _networkServer?.Dispose();
    }

    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobbyId, authId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
