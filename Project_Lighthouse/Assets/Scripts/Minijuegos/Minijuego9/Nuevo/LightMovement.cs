using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    #region Private Variables
    private Camera cam;
    private List<Vector3> movePoints = new List<Vector3>();
    private Rigidbody rb;

    private Vector3 targetPosition;
    [SerializeField]private float speed;
    [SerializeField][Range(0, 1)]private float umbralDistance;
    #endregion

    public LayerMask lightFloor;
    [Tooltip("Delay entre el raton y el movimiento del objeto")]public float mouseDelay;

    
    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MouseRaycast();
        MoveSmooth();
    }

    void MouseRaycast()
    {
        Ray ray = cam.ScreenPointToRay( Input.mousePosition );
        RaycastHit hit;

        if(Physics.Raycast( ray, out hit, Mathf.Infinity, lightFloor))
        {
            /*movePoints.Add(hit.point);
            StartCoroutine(MoveToPoints());*/
            targetPosition = hit.point;
        }
    }

    public void MoveSmooth()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if(distance > umbralDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
        
    }
    public IEnumerator MoveToPoints()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        rb.MovePosition(movePoints[0]);
        /*Vector3 direction = movePoints[0] - transform.position;
        rb.linearVelocity += direction;*/
        movePoints.RemoveAt(0);
    }
}
