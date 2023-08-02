using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeField;
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;

    private bool _isMatchmaking;
    private bool _isCanceling;
    private float _timeInQueue;
    private bool _isBusy;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (_isMatchmaking)
        {
            _timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(_timeInQueue);
            _queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
    }

    public async void FindMatch()
    {
        if (_isCanceling) return;

        if (_isMatchmaking)
        {
            _queueStatusText.text = "Cancelling...";
            _isCanceling = true;

            await ClientSingleton.Instance.GameManager.CancelMatchmaking();

            _isCanceling = false;
            _isMatchmaking = false;
            _isBusy = false;

            _findMatchButtonText.text = "Find Match";
            _queueStatusText.text = string.Empty;
            _queueTimerText.text = string.Empty;
            return;
        }

        if (_isBusy) return;

        ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
        _findMatchButtonText.text = "Cancel";
        _queueStatusText.text = "Searching...";
        _timeInQueue = 0f;
        _isMatchmaking = true;
        _isBusy = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                _queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                _queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                _queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                _queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                _queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        if (_isBusy) return;
        _isBusy = true;

        await HostSingleton.Instance.GameManager.StartHostAsync();
        _isBusy = false;
    }

    public async void StartClient()
    {
        if (_isBusy) return;
        _isBusy = true;

        await ClientSingleton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
        _isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (_isBusy) return;

        _isBusy = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        _isBusy = false;
    }
}
