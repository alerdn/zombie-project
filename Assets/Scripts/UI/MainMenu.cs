using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeField;
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;

    private bool _isMatchmaking;
    private bool _isCanceling;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
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

            _findMatchButtonText.text = "Find Match";
            _queueStatusText.text = string.Empty;
            return;
        }

        ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
        _findMatchButtonText.text = "Cancel";
        _queueStatusText.text = "Searching...";
        _isMatchmaking = true;
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
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
    }
}
