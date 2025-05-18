using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Minijuego3_GameManager : MonoBehaviour
{

    public Camera minigame3Camera;
    private Tuberia[] tuberias;
    public GameObject QueEsEsto;
    private VisualEffect steamVFX;
    [Header("Public Variables")]
    public float rotatePipeSpeed;
    public float steamTimeRate;
    public int minCommentRate;
    public int maxCommentRate;
    private int timesRotated;
    private MinigameComments mc;

    private void Awake()
    {

        //tuberias = GetComponentsInChildren<Tuberia>();
        mc = FindAnyObjectByType<MinigameComments>();
        tuberias = GameObject.FindObjectsByType<Tuberia>(FindObjectsSortMode.None);
        steamVFX = GameObject.Find("Vapor").GetComponent<VisualEffect>();
    }
    void Start()
    {
        InvokeRepeating("RandomSteamGenerator", 5, 2);
        Debug.Log(tuberias.Length);
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            CompleteMinigame();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = minigame3Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray.origin, ray.direction * 10, out hit) && context.performed)
        {
            if (hit.collider.gameObject.CompareTag("Tuberia") && hit.collider.gameObject.GetComponent<Tuberia>() != null)
            {
                if(hit.collider.gameObject.GetComponent<Tuberia>().canRotate)
                {
                    hit.collider.gameObject.GetComponent<Tuberia>().canRotate = false;
                    float rotation = hit.collider.gameObject.transform.eulerAngles.z;
                    rotation += 90;
                    //hit.collider.gameObject.transform.eulerAngles += new Vector3(0, 0, 90);
                    hit.collider.gameObject.transform.DORotate(new Vector3(0, 0, rotation), rotatePipeSpeed, RotateMode.Fast).OnComplete(() =>
                    {
                        hit.collider.gameObject.GetComponent<Tuberia>().canRotate = true;
                        CheckPipes();
                        timesRotated++;
                        if (timesRotated >= Random.Range(minCommentRate, maxCommentRate))
                        { 
                            PipeAndSteamComments(0, 2); 
                        }
                    });
                }
                
                
            }
        }
    }

    public void CheckPipes()
    {
        int correctPipes = 0;
        for (int i = 0; i < tuberias.Length; i++)
        {
            if (tuberias[i].isConnected)
            {
                correctPipes++;
            }
        }
        if (correctPipes == tuberias.Length)
        {
            //Ha ganado el minijuego
            mc.DisplayComment(6);
            Invoke(nameof(CompleteMinigame), 5);
        }
    }

    public void RandomSteamGenerator()
    {
        int random = Random.Range(0, 10);
        Debug.Log(random);
        if(random == 4)
        {
            StartCoroutine(SteamInYoFace());
        }
    }

    public IEnumerator SteamInYoFace()
    {
        steamVFX.Play();
        PipeAndSteamComments(3, 5);
        yield return new WaitForSeconds(steamTimeRate);
        steamVFX.Stop();
    }

    public void CompleteMinigame()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(2);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
        Debug.Log("Todo correcto");
        QueEsEsto.transform.DOMove(Camera.main.transform.position, 0.5f);
    }

    #region CommentsEvents
    public void PipeAndSteamComments(int i, int j)
    {
        mc.DisplayErrorComment(Random.Range(i, j + 1));
    }
    #endregion

}
