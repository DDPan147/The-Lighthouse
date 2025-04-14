using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class LightControler : MonoBehaviour
{
    private Vector2 moveDirection;
    private bool start;
    private Camera cam;
    private Vector3 rayDirection;
    public GameObject gm;
    private GameObject activeCloud;
    private Material matActiveCloud;
    private bool canHit;
    void Start()
    {
        gm = GameObject.FindAnyObjectByType<Minijuego9_GameManager>().gameObject;
        cam = Camera.main;
    }


    void Update()
    {
        //transform.Translate(new Vector3(moveDirection.x, moveDirection.y,0), Space.Self);
        rayDirection = cam.transform.position - transform.position;
        RaycastHit hit;

        if(Physics.Raycast(transform.position, rayDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag("Nube") && canHit)
            {
                activeCloud = hit.collider.gameObject;
                //matActiveCloud = activeCloud.GetComponent<Renderer>().material;   Aun no (hacer que pierda opacidad mientras es eliminado)
                activeCloud.transform.DOShakeScale(1f, 0.25f, 50, 45, true, ShakeRandomnessMode.Full).OnComplete(() =>
                {
                    gm.GetComponent<Minijuego9_GameManager>().totalCloudsKilled += activeCloud.GetComponent<Cloud>().points;
                    activeCloud.GetComponent<Cloud>().isDying = true;
                    activeCloud = null;
                }).SetId("Shake");
                canHit = false;
            }
        }
        else
        {
            if(activeCloud != null)
            {
                DOTween.Kill("Shake");
                activeCloud.transform.localScale = activeCloud.GetComponent<Cloud>().initialScale;
            }
            canHit = true;
        }
    }

    /*public void OnMove(InputAction.CallbackContext context)
    {
        if(start)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection *= Time.deltaTime * moveSpeed;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            start = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    */

    
}
