using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public enum EnemyState
{
    WANDERING,
    PERSUING
}

public class EnemyBase : NetworkBehaviour
{
    public EnemyMovement MovementComponent { get; private set; }

    private StateMachine<EnemyState> _stateMachine;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        InitComponents();
        StartStateMachine();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (_stateMachine.CurrentState != null) _stateMachine.CurrentState.OnStateStay();
    }

    private void InitComponents()
    {
        MovementComponent = GetComponent<EnemyMovement>();
        MovementComponent.Init();
    }

    #region StateMachine

    private void StartStateMachine()
    {
        _stateMachine = new StateMachine<EnemyState>();
        _stateMachine.RegisterState(EnemyState.WANDERING, new EnemyStateWandering());
        _stateMachine.RegisterState(EnemyState.PERSUING, new EnemyStatePersuing());

        SwitchState(EnemyState.WANDERING);
    }

    public void SwitchState(EnemyState state)
    {
        _stateMachine.SwitchState(state, this);
    }

    #endregion
}
