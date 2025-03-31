using UnityEngine;

public class Selectable_MG2 : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    [HideInInspector] public Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    [HideInInspector] public Vector3 origPosition;
    private Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;
    void Start()
    {
        GameObject limitColliders = GameObject.Find("LimitColliders");
        minLimitX = limitColliders.transform.GetChild(3);
        minLimitZ = limitColliders.transform.GetChild(1);
        maxLimitX = limitColliders.transform.GetChild(2);
        maxLimitZ = limitColliders.transform.GetChild(0);
        origPosition = transform.position;
    }


    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        LimitarMovimiento();
    }

    void LimitarMovimiento()
    {
        if (transform.position.x < minLimitX.position.x)
        {
            transform.position = new Vector3(minLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > maxLimitX.position.x)
        {
            transform.position = new Vector3(maxLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z < minLimitZ.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minLimitZ.position.z);
        }
        else if (transform.position.z > maxLimitZ.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxLimitZ.position.z);
        }
    }
}
