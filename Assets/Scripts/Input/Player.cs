using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Color team1Color = Color.blue;
    [SerializeField] private Color team2Color = Color.red;

    [Networked] public int Team { get; set; }
    [Networked] public NetworkBool IsAlive { get; set; }

    private int lastTeam = -1;


    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            IsAlive = true;
        }

        UpdateColor();
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsAlive)
            return;

        if (GetInput(out NetworkInputData data))
        {
            Vector2 movement = new Vector2(data.Direction.x, data.Direction.y);

            transform.Translate(movement * speed * Runner.DeltaTime);
        }
    }


    private void Update()
    {
        if (lastTeam != Team)
        {
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        lastTeam = Team;

        if (spriteRenderer == null)
            return;

        if (Team == 1)
        {
            spriteRenderer.color = team1Color;
        }
        else if (Team == 2)
        {
            spriteRenderer.color = team2Color;
        }
    }

    public void Kill()
    {
        if (!HasStateAuthority)
            return;

        if (!IsAlive)
            return;

        IsAlive = false;

        Debug.Log(
            "Jugador " +
            Object.InputAuthority +
            " murió."
        );
        spriteRenderer.color = Color.gray;

    }
}
