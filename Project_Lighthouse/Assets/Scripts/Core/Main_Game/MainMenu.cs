using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameManager gm;
    public Image[] images;

    public void StartFadeIn()
    {
        foreach(Image im in images)
        {
            im.DOFade(1, 2);
        }
    }
    public void Play()
    {
        gm.StartUp();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
