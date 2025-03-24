using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("MinigameManager")]
    public MinigameData[] minigames;
    public Camera mainCamera;
    public GameObject eventSystem;
    public AudioListener audioListener;

    public static bool minigameActive;

    [Header("MissionManager")]
    public Mission[] missions;
    [SerializeField] private GameObject playerMissionPopup;
    [SerializeField] private TMP_Text playerMissionText;
    [SerializeField] private GameObject GUIMissionHolder;
    [SerializeField] private TMP_Text GUIMissionText;


    
    
    void Start()
    {
        mainCamera = Camera.main;
        eventSystem = FindFirstObjectByType<EventSystem>().gameObject;
        audioListener = FindFirstObjectByType<AudioListener>();
    }

    void Update()
    {
        
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
        FindFirstObjectByType<Player>().UnassignActiveMinigameSwitch();

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

        //Lanzar diálogo

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
            playerMissionText.text = missions[missionIndex].missionName;
            //Activate Text (intro animation)
            playerMissionText.GetComponentInParent<Image>().DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            playerMissionText.DOFade(1, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => StartCoroutine(FadeOut(missions[missionIndex].missionDesc)));
            //Play Effect On Top Of Player's Head
            //Fade Out Top Of Player's Head
            //Deactivate Text (animation)

            
        }

        void PlayEffect()
        {

        }

        IEnumerator FadeOut(string _missionDesc)
        {
            yield return new WaitForSeconds(2);
            playerMissionText.GetComponentInParent<Image>().DOFade(0, 1).SetEase(Ease.InQuad);
            playerMissionText.DOFade(0, 1).SetEase(Ease.InQuad).OnComplete(() => ShowNewMissionOnGUI(_missionDesc));
        }

     
    }
    public void ShowNewMissionOnGUI(string _missionDesc)
    {
        //Replace Text with New Mission Name
        GUIMissionText.text = _missionDesc;
        GUIMissionText.color = Color.white;
        //Play Animation Of New Mission
        GUIMissionText.gameObject.transform.DOMoveX(GUIMissionText.transform.parent.position.x, 2, false).SetEase(Ease.OutBack);

        //Display Marker Of New Mission Location (optional)
    }

    public void CompleteMission(string _missionName)
    {
        int missionIndex = Array.FindIndex(missions, mission => mission.missionName == _missionName);
        //Mark Mission as Completed
        missions[missionIndex].isCompleted = true;
        RemoveMissionFromGUI();

        //Trigger Dialogue of whatever?
    }

    public void RemoveMissionFromGUI()
    {
        //Strikethrough Text Animation

        GUIMissionText.color = Color.green;
        //Dissappear Animation
        GUIMissionText.gameObject.transform.DOLocalMoveX(400, 2, false).SetEase(Ease.InBack);
    }
    #endregion

}
