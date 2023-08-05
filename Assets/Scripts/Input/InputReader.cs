using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu]
public class InputReader : ScriptableObject, IGameplayActions, IUIActions
{
    public event Action<Vector2> MovementEvent;
    public event Action<Vector2> LookEvent;
    public event Action<bool> ShootEvent;
    public event Action<bool> JumpEvent;
    public event Action ToggleInventoryEvent;

    private Controls _controls;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
            _controls.UI.SetCallbacks(this);
        }

        _controls.Gameplay.Enable();
        _controls.UI.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShootEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            ShootEvent?.Invoke(false);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            JumpEvent?.Invoke(false);
        }
    }

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed) ToggleInventoryEvent?.Invoke();
    }
}
