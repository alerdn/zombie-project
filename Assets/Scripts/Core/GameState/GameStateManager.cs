using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    Gameplay,
    Menu
}

public class GameStateManager : NetworkBehaviour
{
    public InputReader InputReader;

    private StateMachine<GameState> _stateMachine;

    #region StateMachine

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartStateMachine();
    }

    private void StartStateMachine()
    {
        _stateMachine = new StateMachine<GameState>();
        _stateMachine.RegisterState(GameState.Gameplay, new GameStateGameplay());

        SwitchState(GameState.Gameplay);
    }

    public void SwitchState(GameState state, params object[] objs)
    {
        _stateMachine.SwitchState(state, this, objs);
    }

    #endregion
}
