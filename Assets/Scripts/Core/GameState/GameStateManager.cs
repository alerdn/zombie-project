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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartInputs();
        StartStateMachine();
    }

    private void StartInputs()
    {
        InputReader.OpenMenuEvent += SwitchStateMenu;
        InputReader.CloseMenuEvent += SwitchStateGameplay;
    }

    #region StateMachine

    private void StartStateMachine()
    {
        _stateMachine = new StateMachine<GameState>();
        _stateMachine.RegisterState(GameState.Gameplay, new GameStateGameplay());
        _stateMachine.RegisterState(GameState.Menu, new GameStateMenu());

        SwitchState(GameState.Gameplay);
    }

    public void SwitchState(GameState state, params object[] objs)
    {
        _stateMachine.SwitchState(state, this, objs);
    }

    #endregion

    #region Gameplay State

    public void SwitchStateMenu()
    {
        SwitchState(GameState.Menu);
        Cursor.lockState = CursorLockMode.Confined;
    }

    #endregion


    #region Menu State

    public void SwitchStateGameplay()
    {
        SwitchState(GameState.Gameplay);
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion
}
