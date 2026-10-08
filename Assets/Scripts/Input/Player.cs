using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Color team1Color = Color.blue;
    [SerializeField] private Color team2Color = Color.red;

    [SerializeField] private GameObject indicator;

    [Header("Power Up PvP")]
    [SerializeField] private float powerUpDuration = 5f;

    [Networked] public int Team { get; set; }
    [Networked] public NetworkBool IsAlive { get; set; }

    // POWER UP PVP
    [Networked] public NetworkBool CanPvp { get; set; }
    [Networked] private float PowerUpEndTime { get; set; }


    // DASH POWER UP
    [Header("Dash")]
    [SerializeField] private float dashDuration = 10f;
    [SerializeField] private float dashDistance = 3f;

    [Networked] public NetworkBool HasDash { get; set; }
    [Networked] private float DashEndTime { get; set; }

    [Header("Dash Visual")]
    [SerializeField] private TrailRenderer dashTrail;


    private int lastTeam = -1;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }

        if (Object.HasStateAuthority)
        {
            IsAlive = true;

            // DASH
            HasDash = false;
            DashEndTime = 0f;
        }

        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        UpdateColor();
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsAlive)
            return;

        if (GetInput(out NetworkInputData data))
        {
            Vector2 movement = new Vector2(
                data.Direction.x,
                data.Direction.y
            );

            // Movimiento normal
            transform.Translate(
                movement * speed * Runner.DeltaTime
            );


            if (Object.HasStateAuthority && HasDash)
            {
                if (Runner.SimulationTime >= DashEndTime)
                {
                    HasDash = false;

                    Debug.Log("Dash Power Up terminado");
                }
                else if (data.Buttons.IsSet(NetworkInputData.DashButton))
                {
                    Dash(movement);
                }
            }
        }


        if (CanPvp && Runner.SimulationTime >= PowerUpEndTime)
        {
            CanPvp = false;

            Debug.Log("Power Up terminado");
        }
    }

    private void Dash(Vector2 movement)
    {
        if (movement == Vector2.zero)
            return;

        movement.Normalize();

        transform.position += (Vector3)(movement * dashDistance);

        Debug.Log("DASH!");
    }

    public void ActivateDash()
    {
        if (!HasStateAuthority)
            return;

        HasDash = true;

        DashEndTime = Runner.SimulationTime + dashDuration;

        Debug.Log("Dash Power Up activado durante " + dashDuration + " segundos");
    }


    private void Update()
    {
        if (lastTeam != Team)
        {
            UpdateColor();
        }

        if (!IsAlive)
        {
            spriteRenderer.color = Color.gray;
        }

        // Efecto visual del Dash
        if (dashTrail != null)
        {
            dashTrail.emitting = HasDash;
        }
    }


    // =========================
    // POWER UP PVP ORIGINAL
    // =========================

    public void ActivatePowerUp()
    {
        if (!HasStateAuthority)
            return;

        CanPvp = true;
        PowerUpEndTime = Runner.SimulationTime + powerUpDuration;

        Debug.Log("Power Up activado");
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

        Debug.Log("Jugador " + Object.InputAuthority + " murió.");
    }
}