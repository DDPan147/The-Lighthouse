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
    [HideInInspector]public Minijuego9_GameManager gm;
    [HideInInspector]public CloudSpawn cloudSpawn;
    private GameObject activeCloud;
    private Material matActiveCloud;
    private bool canHit;
    private int maxCloudKill;
    void Start()
    {
        gm = GameObject.FindAnyObjectByType<Minijuego9_GameManager>();
        cloudSpawn = GameObject.FindAnyObjectByType<CloudSpawn>();
        cam = Camera.main;
        maxCloudKill = gm.cloudNumberToKill;
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
                    gm.totalCloudsKilled += activeCloud.GetComponent<Cloud>().points;
                    activeCloud.GetComponent<Cloud>().isDying = true;
                    activeCloud = null;
                    if(gm.totalCloudsKilled == 1)
                    {
                        gm.mc.DisplayComment(0);
                    }
                    else if(gm.totalCloudsKilled == gm.cloudNumberToKill / 4)
                    {
                        gm.mc.DisplayComment(1);
                    }
                    else if(gm.totalCloudsKilled == gm.cloudNumberToKill / 2)
                    {
                        gm.mc.DisplayComment(2);
                    }
                    else if(gm.totalCloudsKilled == gm.cloudNumberToKill * 3 / 4)
                    {
                        gm.mc.DisplayComment(3);
                    }
                    else if(gm.totalCloudsKilled >= gm.cloudNumberToKill)
                    {
                        cloudSpawn.CancelInvoke("SpawnCloud");
                        StartCoroutine(gm.FinishMinigame9Sequence());
                    }
                }).SetId("Shake");
                canHit = false;
            }
            if (hit.collider.gameObject.CompareTag("NubeMala") && canHit)
            {
                activeCloud = hit.collider.gameObject;
                //matActiveCloud = activeCloud.GetComponent<Renderer>().material;   Aun no (hacer que pierda opacidad mientras es eliminado)
                activeCloud.transform.DOShakeScale(1f, 0.25f, 50, 45, true, ShakeRandomnessMode.Full).OnComplete(() =>
                {
                    gm.totalCloudsKilled += activeCloud.GetComponent<Cloud>().points;
                    activeCloud.GetComponent<Cloud>().isDying = true;
                    activeCloud = null;
                    BadCloudsComments();
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
    public void BadCloudsComments()
    {
        gm.mc.DisplayErrorComment(Random.Range(4, 7));
    }

}
