using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement_Minigame_7 : MonoBehaviour
{
    // --- Referencias ---
    private Rigidbody rb;
    private PlayerOnGround ground;

    // --- Movimiento general ---
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 desiredVelocity;
    private Vector2 inputDirection;
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
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;
    [SerializeField][Tooltip("Magnitude of the force of bounce")] private float bounceForce = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ground = GetComponent<PlayerOnGround>();
    }

    // Update is called once per frame
    void Update()
    {
        onGround = ground.GetOnGround();
        onWall = ground.GetOnWall();

        velocity = rb.linearVelocity;

        desiredVelocity = new Vector3(directionX, 0f, directionZ) * Mathf.Max(maxSpeed - friction, 0f);
    }
    private void FixedUpdate()
    {
        if (onGround)
        {
            RunWithAcceleration();
        }
        else
        {
            RunWithoutAcceleration();
        }

    }
    public void OnMoveCallback(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
        directionX = inputDirection.x; // Movimiento horizontal
        directionZ = inputDirection.y;
        isMoving = inputDirection != Vector2.zero;

        if (isMoving)
        {
            ground.UpdatePlayerDirection(transform.position);
        }
    }
    private void RunWithAcceleration()
    {
        acceleration = onGround ? maxAcceleration : 0f;
        deceleration = onGround ? maxDecceleration : 0f;
        turnSpeed = onGround ? maxTurnSpeed : 0f;

        if (isMoving)
        {
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
            if (Mathf.Sign(directionZ) != Mathf.Sign(velocity.z))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }
        ApplyMovement();
    }

    private void RunWithoutAcceleration()
    {
        rb.linearVelocity = new Vector3(0f,0f,0f);
    }

    private void ApplyMovement()
    {
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        rb.linearVelocity = velocity;
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
