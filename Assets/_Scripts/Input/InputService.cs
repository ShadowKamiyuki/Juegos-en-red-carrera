using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IInputService, IDisposable
{
    private readonly InputSystemActions controls;

    public Vector2 Move => controls.Gameplay.Move.ReadValue<Vector2>();

    public Vector2 Aim => controls.Gameplay.Aim.ReadValue<Vector2>();

    public bool AttackHeld => controls.Gameplay.Attack.IsPressed();

    public event Action OnAttackPressed;
    public event Action OnAttackReleased;

    public event Action OnDodgePressed;
    public event Action OnInteractPressed;
    public event Action OnPausePressed;

    public InputService()
    {
        controls = new InputSystemActions();

        controls.Gameplay.Attack.started += AttackStarted;
        controls.Gameplay.Attack.canceled += AttackCanceled;

        controls.Gameplay.Dodge.performed += DodgePerformed;
        controls.Gameplay.Interact.performed += InteractPerformed;
        controls.Gameplay.Pause.performed += PausePerformed;

        EnableGameplay();
    }

    public void EnableGameplay()
    {
        controls.UI.Disable();
        controls.Gameplay.Enable();
    }

    public void EnableUI()
    {
        controls.Gameplay.Disable();
        controls.UI.Enable();
    }

    public void Dispose()
    {
        controls.Gameplay.Attack.started -= AttackStarted;
        controls.Gameplay.Attack.canceled -= AttackCanceled;

        controls.Gameplay.Dodge.performed -= DodgePerformed;
        controls.Gameplay.Interact.performed -= InteractPerformed;
        controls.Gameplay.Pause.performed -= PausePerformed;

        controls.Dispose();
    }

    private void AttackStarted(InputAction.CallbackContext _) => OnAttackPressed?.Invoke();

    private void AttackCanceled(InputAction.CallbackContext _) => OnAttackReleased?.Invoke();

    private void DodgePerformed(InputAction.CallbackContext _) => OnDodgePressed?.Invoke();

    private void InteractPerformed(InputAction.CallbackContext _) => OnInteractPressed?.Invoke();

    private void PausePerformed(InputAction.CallbackContext _) => OnPausePressed?.Invoke();
}