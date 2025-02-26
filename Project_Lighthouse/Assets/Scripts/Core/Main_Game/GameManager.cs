using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public MinigameData[] minigames;
    public Camera mainCamera;
    public GameObject eventSystem;
    public AudioListener audioListener;


    public static bool minigameActive;
    
    void Start()
    {
        mainCamera = Camera.main;
        eventSystem = FindFirstObjectByType<EventSystem>().gameObject;
        audioListener = FindFirstObjectByType<AudioListener>();
    }

    void Update()
    {
        
    }

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
    }

}
