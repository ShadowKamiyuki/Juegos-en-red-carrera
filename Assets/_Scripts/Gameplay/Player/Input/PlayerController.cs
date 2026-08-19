using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IInputService input;

    private Player player;

    public void Construct(IInputService input)
    {
        this.input = input;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        input.OnAttackPressed += player.Attack;
        input.OnDodgePressed += player.Dodge;
        input.OnInteractPressed += player.Interact;
    }

    private void OnDisable()
    {
        input.OnAttackPressed -= player.Attack;
        input.OnDodgePressed -= player.Dodge;
        input.OnInteractPressed -= player.Interact;
    }

    private void Update()
    {
        player.Move(input.Move);
    }
}