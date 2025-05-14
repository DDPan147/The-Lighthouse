using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider progressSlider;
    public GameObject handle;
    void Start()
    {
        progressSlider = GetComponent<Slider>();
    }


    public void PotComplete()
    {
        if(progressSlider.value >= 0.995f)
        {
            handle.SetActive(false);
        }
    }
}
