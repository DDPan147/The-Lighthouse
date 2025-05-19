using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameManager gm;

    public void Play()
    {
        gm.StartUp();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
