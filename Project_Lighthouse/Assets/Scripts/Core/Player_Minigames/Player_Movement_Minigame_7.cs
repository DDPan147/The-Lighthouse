using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement_Minigame_7 : MonoBehaviour
{
    // --- Referencias ---
    private Rigidbody rb;
    private PlayerOnGround ground;
    private Collider grabCollider;
    public GameObject abueloPrefab;
    public Animator animator;

    // --- Movimiento general ---
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 desiredVelocity;
    public Vector2 inputDirection;
    private bool isMoving = false;
    private bool onGround;
    private bool onWall;

    // --- Direccion de movimiento ---
    private float directionX;
    private float directionZ;

    // --- Cambios de velocidad ---
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    // --- Parametros de movimiento ---
    [Header("Movement Stats")]
    [SerializeField, Range(0f, 5f)][Tooltip("Maximum movement speed")] public float maxSpeed = 5f;
    [SerializeField, Range(0f, 20f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 20f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;
    [SerializeField][Tooltip("Magnitude of the force of bounce")] private float bounceForce = 5f;

    [HideInInspector] public bool isMovementBlocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ground = GetComponent<PlayerOnGround>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isMovementBlocked) return;

        onGround = ground.GetOnGround();
        onWall = ground.GetOnWall();

        velocity = rb.linearVelocity;

        Vector3 input = new Vector3(directionX, 0f, directionZ).normalized;
        desiredVelocity = input * maxSpeed;
    }

    private void FixedUpdate()
    {
        if (isMovementBlocked) return;

        if (onGround)
        {
            ApplyGroundMovement();
        }
        else
        {
            
        }
    }

    public void OnMoveCallback(InputAction.CallbackContext context)
    {
        if (isMovementBlocked) return;

        inputDirection = context.ReadValue<Vector2>();
        directionX = inputDirection.x;
        directionZ = inputDirection.y;
        isMoving = inputDirection != Vector2.zero;

        animator.SetBool("Idle", !isMoving);
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            ground.UpdatePlayerDirection(transform.position);
            Vector3 moveDirection = new Vector3(directionX, 0f, directionZ);
            if (moveDirection != Vector3.zero)
            {
                abueloPrefab.transform.rotation = Quaternion.LookRotation(-moveDirection);
            }
        }
    }

    private void ApplyGroundMovement()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = desiredVelocity - currentVelocity;
        velocityChange.y = 0f; // keep gravity effect untouched

        float appliedAcceleration = isMoving ? maxAcceleration : maxDecceleration;

        Vector3 clampedVelocityChange = Vector3.ClampMagnitude(velocityChange, appliedAcceleration * Time.fixedDeltaTime);
        rb.AddForce(clampedVelocityChange, ForceMode.VelocityChange);
    }

  

    private void OnCollisionEnter(Collision collision)
    {
        if (onWall)
        {
            ApplyWallBounce(collision);
        }
    }

    private void ApplyWallBounce(Collision collision)
    {
        Vector3 bounceDirection = collision.contacts[0].normal;
        rb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
    }
}
