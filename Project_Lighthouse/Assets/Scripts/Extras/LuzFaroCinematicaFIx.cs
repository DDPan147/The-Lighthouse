using DG.Tweening;
using UnityEngine;

public class LuzFaroCinematicaFIx : MonoBehaviour
{
    public Light[] lights;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReducirLentamente()
    {
        foreach (var light in lights)
        {
            DOTween.To(() => light.intensity, x => light.intensity = x, 0, 30);
        }
        
    }
}
