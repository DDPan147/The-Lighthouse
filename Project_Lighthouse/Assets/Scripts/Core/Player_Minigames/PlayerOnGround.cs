using UnityEngine;

public class PlayerOnGround : MonoBehaviour
{
    private bool onGround;
    private bool onWall;
    private Vector3 playerDirection;

    [Header("Collider Settings")]
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.95f;
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float wallLength = 0.2f;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 groundColliderOffset;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 wallColliderOffset;

    [Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask wallLayer;


    private void Update()
    {
        //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
        onGround = Physics.Raycast(transform.position + groundColliderOffset, Vector3.down, groundLength, groundLayer) || 
                        Physics.Raycast(transform.position - groundColliderOffset, Vector3.down, groundLength, groundLayer);

        onWall = CheckWalls();
    }
    public void UpdatePlayerDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            playerDirection = direction.normalized;
        }
    }
    private bool CheckWalls()
    {
        bool frontWall = Physics.Raycast(transform.position + wallColliderOffset, transform.forward, wallLength, wallLayer) ||
                         Physics.Raycast(transform.position - wallColliderOffset, transform.forward, wallLength, wallLayer);
        bool leftWall = Physics.Raycast(transform.position + wallColliderOffset, -transform.right, wallLength, wallLayer);
        bool rightWall = Physics.Raycast(transform.position + wallColliderOffset, transform.right, wallLength, wallLayer);
        
        return frontWall || leftWall || rightWall;
    }

    private void OnDrawGizmos()
    {
        //Draw the ground colliders on screen for debug purposes
        if (onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + groundColliderOffset, transform.position +
                        groundColliderOffset + Vector3.down * groundLength);

        if (onWall) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        if (playerDirection != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position + wallColliderOffset, 
                transform.position + wallColliderOffset + playerDirection * wallLength);
            Gizmos.DrawLine(transform.position - wallColliderOffset, 
                transform.position - wallColliderOffset + playerDirection * wallLength);
        }
    }

    

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }
    public bool GetOnWall() { return onWall; }
}
