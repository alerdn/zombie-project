using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyNameText;
    [SerializeField] private TMP_Text _lobbyPlayersText;

    private LobbyList _lobbyList;
    private Lobby _lobby;

    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        _lobbyList = lobbyList;
        _lobby = lobby;

        _lobbyNameText.text = lobby.Name;
        _lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        _lobbyList.JoinAsync(_lobby);
    }
}
