using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu]
public class InputReader : ScriptableObject, IGameplayActions, IUIActions
{
    public Controls Controls { get; private set; }

    public Action<Vector2> MovementEvent;
    public Action<Vector2> LookEvent;
    public Action<bool> ShootEvent;
    public Action<bool> JumpEvent;
    public Action OpenMenuEvent;
    public Action CloseMenuEvent;
    public Action ReturnMenuEvent;

    private void OnEnable()
    {
        if (Controls == null)
        {
            Controls = new Controls();
            Controls.Gameplay.SetCallbacks(this);
            Controls.UI.SetCallbacks(this);
        }
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

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (context.performed) OpenMenuEvent?.Invoke();
    }

    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (context.performed) CloseMenuEvent?.Invoke();
    }

    public void OnReturnMenu(InputAction.CallbackContext context)
    {
        if (context.performed) ReturnMenuEvent?.Invoke();
    }
}
