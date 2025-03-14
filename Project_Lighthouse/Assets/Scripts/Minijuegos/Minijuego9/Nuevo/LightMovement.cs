using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    #region Private Variables
    private Camera cam;
    private List<Vector3> movePoints = new List<Vector3>();
    private Rigidbody rb;
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
    }

    void MouseRaycast()
    {
        Ray ray = cam.ScreenPointToRay( Input.mousePosition );
        RaycastHit hit;

        if(Physics.Raycast( ray, out hit, Mathf.Infinity, lightFloor))
        {
            movePoints.Add(hit.point);
            StartCoroutine(MoveToPoints());
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
