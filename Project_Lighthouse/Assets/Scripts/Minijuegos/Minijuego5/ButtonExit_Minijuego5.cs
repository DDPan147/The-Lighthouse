using UnityEngine;
using UnityEngine.UI;

public class ButtonExit_Minijuego5 : MonoBehaviour
{
    private Button button;
    private GameManager globalGameManager;

    private void Start()
    {
        button = GetComponent<Button>();
        globalGameManager = FindAnyObjectByType<GameManager>();
        button.onClick.AddListener(() => globalGameManager.CloseMinigame(4));
    }
}
