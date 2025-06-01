using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class Minigame8_GameManager : MonoBehaviour
{
    public Camera minigame8Camera;
    public GameObject canica, candado, marco;
    public bool canRotate = true;
    private int timesRotate;
    public int minCommentRate, maxCommentRate;
    [HideInInspector]public MinigameComments mc;
    [HideInInspector]public float rotationX, rotationY, rotationZ;
    [Header("ArrowSprites")]
    public Image leftArrow, rightArrow;
    public Sprite leftArrowSelectedSprite, rightArrowSelectedSprite;
    private Sprite leftArrowSprite, rightArrowSprite;
    private void Awake()
    {
        rotationX = candado.transform.localEulerAngles.x;
        rotationY = candado.transform.localEulerAngles.y;
        mc = GetComponent<MinigameComments>();
        leftArrowSprite = leftArrow.sprite;
        rightArrowSprite = rightArrow.sprite;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKeyDown(KeyCode.RightControl))
        {
            Win();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = minigame8Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hit) && canRotate)
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.gameObject.CompareTag("Tuberia"))
                {
                    leftArrow.sprite = leftArrowSelectedSprite;
                    canRotate = false;
                    rotationZ += 90;
                    candado.transform.DOLocalRotate(new Vector3(rotationX,rotationY, rotationZ), 0.5f, RotateMode.Fast).OnComplete(() =>
                    {
                        timesRotate++;
                        canica.GetComponent<Pelota>().MoveMarble();
                        StartCoroutine(CanRotate());
                    });
                }
                if (hit.collider.gameObject.CompareTag("Tuberia_Entrada"))
                {
                    rightArrow.sprite = rightArrowSelectedSprite;
                    canRotate = false;
                    rotationZ += -90;
                    candado.transform.DOLocalRotate(new Vector3(rotationX,rotationY, rotationZ), 0.5f, RotateMode.Fast).OnComplete(() =>
                    {
                        timesRotate++;
                        canica.GetComponent<Pelota>().MoveMarble();
                        StartCoroutine(CanRotate());
                    });
                }
                if(timesRotate >= Random.Range(minCommentRate, maxCommentRate))
                {
                    RotateComments(0, 4);
                }
            }
        }

        if (context.canceled)
        {
            StartCoroutine(ReturnNormalSprites());
        }

    }

    public IEnumerator ReturnNormalSprites()
    {
        yield return new WaitForSeconds(0.5f);
        leftArrow.sprite = leftArrowSprite;
        rightArrow.sprite = rightArrowSprite;
    }

    public void Win()
    {
        Debug.Log("Has Ganado");
        

        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            FindAnyObjectByType<GameManager>().MinigameCompleted(7);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }

    public IEnumerator CanRotate()
    {
        yield return new WaitForSeconds(0.9f);
        canRotate = true;
    }

    public void RotateComments(int i, int j)
    {
        mc.DisplayErrorComment(Random.Range(i, j + 1));
    }
}
