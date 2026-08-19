using System;
using UnityEngine;

public interface IInputService
{
    Vector2 Move { get; }
    Vector2 Aim { get; }

    bool AttackHeld { get; }

    event Action OnAttackPressed;
    event Action OnAttackReleased;

    event Action OnDodgePressed;
    event Action OnInteractPressed;
    event Action OnPausePressed;

    void EnableGameplay();
    void EnableUI();
}