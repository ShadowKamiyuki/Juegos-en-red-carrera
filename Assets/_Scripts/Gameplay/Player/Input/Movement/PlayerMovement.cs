using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Dodge")]
    [SerializeField] private float dodgeForce = 12f;
    [SerializeField] private float dodgeDuration = 0.15f;
    [SerializeField] private float dodgeCooldown = 0.35f;

    private Rigidbody rb;

    private Vector3 moveDirection;

    private bool isDodging;
    private float dodgeTimer;
    private float nextDodgeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = new Vector3(direction.x, 0, direction.y);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleDodge();
    }

    private void HandleMovement()
    {
        if (isDodging)
            return;

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void HandleRotation()
    {
        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    public bool TryDodge()
    {
        if (Time.time < nextDodgeTime)
            return false;

        if (moveDirection.sqrMagnitude < 0.001f)
            return false;

        nextDodgeTime = Time.time + dodgeCooldown;

        isDodging = true;
        dodgeTimer = dodgeDuration;

        return true;
    }

    private void HandleDodge()
    {
        if (!isDodging)
            return;

        rb.AddForce(moveDirection * dodgeForce, ForceMode.VelocityChange);

        dodgeTimer -= Time.fixedDeltaTime;

        if (dodgeTimer <= 0f)
            isDodging = false;
    }
}