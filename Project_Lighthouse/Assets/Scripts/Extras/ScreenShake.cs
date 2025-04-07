using DG.Tweening;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            
        }
    }

    /*Vector3 ShakeCamera(Vector3 position)
    {
        DOTween.Shake(() => position, () => )
    }
    */
}
