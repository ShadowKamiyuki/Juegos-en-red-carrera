using UnityEngine;

public class Player : MonoBehaviour, ICameraTarget
{
    // add stats, health, energy

    private PlayerMovement movement;
    private PlayerCombat combat;
    private PlayerInteraction interaction;

    public Transform Transform => transform;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        interaction = GetComponent<PlayerInteraction>();
    }

    public void Move(Vector2 direction)
    {
        movement.SetMoveDirection(direction);
    }

    public void Attack()
    {
        //combat.TryAttack();
    }

    public void Dodge()
    {
        movement.TryDodge();
    }

    public void Interact()
    {
        interaction.TryInteract();
    }
}