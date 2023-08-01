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
using Unity.Services.Matchmaker.Models;

public class ServerGameManager : IDisposable
{
    private string _serverIP;
    private int _serverPort;
    private int _queryPort;
    private NetworkServer _networkServer;
    private MultiplayAllocationService _multiplayAllocationService;
    private MatchplayBackfiller _backfiller;
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

        try
        {
            MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

            if (matchmakerPayload != null)
            {
                await StartBackfill(matchmakerPayload);
                _networkServer.OnUserJoined += UserJoined;
                _networkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogWarning("Matchmaker payload timed out");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        if (!_networkServer.OpenConnection(_serverIP, _serverPort))
        {
            Debug.LogWarning("Network server did not start as expected.");
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        _backfiller = new MatchplayBackfiller($"{_serverIP}:{_serverPort}", payload.QueueName, payload.MatchProperties, 20);

        if (_backfiller.NeedsPlayers())
        {
            await _backfiller.BeginBackfilling();
        }
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakingPayloadTask = _multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakingPayloadTask, Task.Delay(20000)) == matchmakingPayloadTask)
        {
            return matchmakingPayloadTask.Result;
        }

        return null;
    }

    private void UserJoined(UserData user)
    {
        _backfiller.AddPlayerToMatch(user);
        _multiplayAllocationService.AddPlayer();

        if (!_backfiller.NeedsPlayers() && _backfiller.IsBackfilling)
        {
            _ = _backfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = _backfiller.RemovePlayerFromMatch(user.userAuthId);
        _multiplayAllocationService.RemovePlayer();

        if (playerCount <= 0)
        {
            CloseServer();
            return;
        }

        if (_backfiller.NeedsPlayers() && !_backfiller.IsBackfilling)
        {
            _ = _backfiller.BeginBackfilling();
        }
    }

    private async void CloseServer()
    {
        await _backfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    public void Dispose()
    {
        _networkServer.OnUserJoined -= UserJoined;
        _networkServer.OnUserLeft -= UserLeft;

        _backfiller?.Dispose();
        _multiplayAllocationService?.Dispose();
        _networkServer?.Dispose();
    }
}
