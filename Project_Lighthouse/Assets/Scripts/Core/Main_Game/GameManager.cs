using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool minigameActiveTest;
    public bool cutsceneActiveTest;
    public GameObject luna;


    public Material highlightMat;
    [Header("MinigameManager")]
    public MinigameData[] minigames;

    public UnityEvent[] OnMinigameEnded = new UnityEvent[9];
    private Camera mainCamera;
    private GameObject eventSystem;
    private AudioListener audioListener;

    public static bool minigameActive;
    

    [Space(30)]
    [Header("MissionManager")]
    public Mission[] missions;
    [SerializeField] private GameObject playerMissionPopup;
    [SerializeField] private TMP_Text playerMissionText;
    [SerializeField] private GameObject GUIMissionHolder;
    [SerializeField] private TMP_Text GUIMissionText;

    [Space(30)]
    [Header("Cutscenes")]

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private CinemachineCamera cutsceneCamera;
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private Image curtain;
    private Player player;

    //public UnityEvent[] OnCutsceneStarted;
    public UnityEvent[] OnCutsceneEnded;
    [SerializeField] private PlayableAsset[] cutscenes;

    public GameObject UpperBand;
    public GameObject LowerBand;

    public static bool cutsceneActive;

    [HideInInspector]public UnityEvent OnEveryCutsceneStart;
    [HideInInspector]public UnityEvent OnEveryCutsceneEnd;
    /*[System.Serializable]
    public struct CutsceneSetup
    {
        public int cutsceneIndex;
        public SplineContainer playerSpline;
        public SplineContainer lunaSpline;
        public SplineContainer cameraSpline;
    }*/

    [Space(30)]
    [Header("DaySystem")]
    public UnityEvent[] OnDayStart = new UnityEvent[4];
    public static int dayCount;

    void Start()
    {
        mainCamera = Camera.main;
        eventSystem = FindAnyObjectByType<EventSystem>().gameObject;
        audioListener = FindAnyObjectByType<AudioListener>();
        player = FindAnyObjectByType<Player>();
    }

    public void StartUp()
    {
        mainMenu.SetActive(false);
        dayCount = 1;
        StartDay(dayCount);
        player.ToggleCollider();
    }

    void Update()
    {
        minigameActiveTest = minigameActive;
        cutsceneActiveTest = cutsceneActive;
    }
    #region MinigameManager
    public void LoadMinigame(int index)
    {
        //Cargar Escena de minijuego
        SceneManager.LoadScene(minigames[index].sceneName, LoadSceneMode.Additive);

        //Desactivar todo en la escena principal
            //Movimiento del jugador
            //Efectos de particulas
            //etc... (posiblemente con una variable global en el game manager a la que todos accedan para poder moverse
        minigameActive = true;
        FindAnyObjectByType<Player>().UnassignActiveMinigameSwitch();

        //Desactivar camara principal
        mainCamera.enabled = false;
        eventSystem.SetActive(false);
        audioListener.enabled = false;

    }

    public void MinigameCompleted(int index)
    {
        //Activar Evento Final en escena (feedback visual de que el minijuego se ha completado

        //Descargar (Unload) escena de minijuego
        SceneManager.UnloadSceneAsync(minigames[index].sceneName);

        //Marcar minijuego como completado
        minigames[index].isCompleted = true;

        //Activar todo en la escena principal de nuevo
        minigameActive = false;

        //Cambiar a la camara del juego
        mainCamera.enabled = true;
        eventSystem.SetActive(true);
        audioListener.enabled = true;

        //Activar evento de minijuego completado
        OnMinigameEnded[index]?.Invoke();
    }

    public void CloseMinigame(int index)
    {
        //Activar Evento Final en escena (feedback visual de que el minijuego se ha completado

        //Descargar (Unload) escena de minijuego
        SceneManager.UnloadSceneAsync(minigames[index].sceneName);

        //Activar todo en la escena principal de nuevo
        minigameActive = false;

        //Cambiar a la camara del juego
        mainCamera.enabled = true;
        eventSystem.SetActive(true);
        audioListener.enabled = true;
    }
    #endregion
    #region MissionManager
    public void UnlockNewMission(string _missionName)
    {
        int missionIndex = Array.FindIndex(missions, mission => mission.missionName == _missionName);
        if (missions[missionIndex] != null)
        {
            missions[missionIndex].isDiscovered = true;

            //Show Mission On Top Of Player's Head
            //Replace Text with new mission name
            playerMissionText.text = missions[missionIndex].missionDesc;
            //Activate Text (intro animation)
            //playerMissionText.GetComponentInParent<Image>().DOFade(1, 0.5f).SetEase(Ease.OutQuad).SetId("Mission");
            playerMissionText.DOFade(1, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => StartCoroutine(FadeOut(missions[missionIndex].missionDesc))).SetId("Mission");
            //Play Effect On Top Of Player's Head
            //Fade Out Top Of Player's Head
            //Deactivate Text (animation)

            
        }

        IEnumerator FadeOut(string _missionDesc)
        {
            yield return new WaitForSeconds(2);
            //playerMissionText.GetComponentInParent<Image>().DOFade(0, 1).SetEase(Ease.InQuad);
            playerMissionText.DOFade(0, 1).SetEase(Ease.InQuad).OnComplete(() => ShowNewMissionOnGUI(_missionDesc)).SetId("Mission");
        }

     
    }
    public void ShowNewMissionOnGUI(string _missionDesc)
    {
        
        //Replace Text with New Mission Name
        GUIMissionText.text = _missionDesc;
        GUIMissionText.color = Color.white;
        //Play Animation Of New Mission
        GUIMissionText.gameObject.transform.DOMoveX(GUIMissionText.transform.parent.position.x, 2, false).SetEase(Ease.OutBack).SetId("Mission");

        //Display Marker Of New Mission Location (optional)
    }
    
    public void CompleteMission(string _missionName)
    {
        DOTween.Complete("Mission");
        int missionIndex = Array.FindIndex(missions, mission => mission.missionName == _missionName);
        //Mark Mission as Completed
        missions[missionIndex].isCompleted = true;
        RemoveMissionFromGUI();
    }

    public void CompleteMissionWithInterrupt(string _missionName)
    {
        DOTween.Complete("Mission");
        int missionIndex = Array.FindIndex(missions, mission => mission.missionName == _missionName);
        //Mark Mission as Completed
        missions[missionIndex].isCompleted = true;
        RemoveMissionFromGUIInterrupt();
    }

    public Mission GetMission(string _missionName)
    {
        return Array.Find(missions, mission => mission.missionName == _missionName);
    }
    public void RemoveMissionFromGUI()
    {
        //Strikethrough Text Animation

        GUIMissionText.color = Color.green;
        //Dissappear Animation
        GUIMissionText.gameObject.transform.DOLocalMoveX(400, 2, false).SetEase(Ease.InBack);
    }

    public void RemoveMissionFromGUIInterrupt()
    {
        GUIMissionText.color = Color.green;
        //Dissappear Animation
        GUIMissionText.gameObject.transform.DOLocalMoveX(400, 2, false).SetEase(Ease.InBack).OnComplete(() =>
        {
            UnlockNewMission("Tutorial2");
            StartCoroutine(MissionInteract());
        });

    }

    public void UnlockTutorial1()
    {
        StartCoroutine(CoroutineTutorial1());

    }
    IEnumerator CoroutineTutorial1()
    {
        yield return new WaitForSeconds(2);
        UnlockNewMission("Tutorial1");
        StartCoroutine(MissionMove());
    }
    IEnumerator MissionMove()
    {
        yield return new WaitForSeconds(4);
        do
        {
            Debug.Log("Me estoy reproduciendo: Move");
            yield return null;
        } while (Player.moveVector == 0);
        //CompleteMission("Tutorial1");
        CompleteMissionWithInterrupt("Tutorial1");
    }
    IEnumerator MissionInteract()
    {
        yield return new WaitForSeconds(4);
        do
        {
            Debug.Log("Me estoy reproduciendo: Interact");
            yield return null;
        } while (!Player.interact);
        CompleteMission("Tutorial2");

    }
    #endregion
    #region Cutscenes
    /*public void CurtainFadeOut(float time)
    {
        curtain.DOFade(1, time);
    }
    public void CurtainFadeIn(float time)
    {
        curtain.DOFade(0, time);
    }*/
    public void SetMinigameAvailable(int index)
    {
        minigames[index].isAvailable = true;
    }
    public void CutsceneStart(int index)
    {
        cutsceneActive = true;
        cutsceneCamera.Priority = 5;
        timelineDirector.Play(cutscenes[index]);
        player.ToggleAnimator();
        curtain.gameObject.GetComponent<Animator>().enabled = true;
        SetBlackBands();


    }
    public void CutsceneEnd(int index)
    {
        cutsceneActive = false;
        cutsceneCamera.Priority = 0;
        player.UntoggleAnimator();
        curtain.gameObject.GetComponent<Animator>().enabled = false;
        UnsetBlackBands();
        OnCutsceneEnded[index]?.Invoke();
        Debug.Log("Ha acabado la cinemática " + index);
    }

    void SetBlackBands()
    {
        UpperBand.transform.DOLocalMoveY(240, 2, false).SetEase(Ease.InOutQuart);
        LowerBand.transform.DOLocalMoveY(-240, 2, false).SetEase(Ease.InOutQuart);
    }
    void UnsetBlackBands()
    {
        UpperBand.transform.DOLocalMoveY(275, 2, false).SetEase(Ease.InOutQuart);
        LowerBand.transform.DOLocalMoveY(-275, 2, false).SetEase(Ease.InOutQuart);
    }
    public void LunaMoverGrito()
    {
        //Voy a tener que desactivar todo lol
        luna.transform.position = new Vector3(1.45f, 13.47f, -4.86f);
        luna.GetComponent<MeshRenderer>().enabled = false;
    }
    #endregion
    #region Day System
    public void DelayedCurtainFadeIn(float delay)
    {
        curtain.DOFade(0, 2).SetDelay(delay);
    }
    public void StartDay(int index)
    {
        Debug.Log("Voy a empezar el día " + (index - 1));
        OnDayStart[index-1]?.Invoke();
    }

    public void EndDay()
    {
        //Launch DAY 2 Screen
        //Make It Dissappear
        dayCount++;
        Debug.Log("Current Day Count: " + dayCount);
        if(dayCount < 5)
        {
            StartDay(dayCount);
        }
        
    }
    #endregion
    public void CreditsRoll()
    {
        //Your code here
        SceneManager.LoadScene("ProvEnding");
    }

}
