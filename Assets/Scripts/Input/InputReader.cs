using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu]
public class InputReader : ScriptableObject, IGameplayActions
{
    public event Action<Vector2> MovementEvent;

    private Controls _controls;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
        }

        _controls.Gameplay.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementEvent?.Invoke(context.ReadValue<Vector2>());
    }
}
