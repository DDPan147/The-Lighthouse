using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cutSceneTime;
    private float currentTime;
    private CinemachineSplineDolly cinemachineSplineDolly;
    void Start()
    {
        cinemachineSplineDolly = GetComponent<CinemachineSplineDolly>();
    }

    void Update()
    {
        CameraMove();
    }


    public void CameraMove()
    {
        currentTime += Time.deltaTime;
        cinemachineSplineDolly.CameraPosition = currentTime/cutSceneTime;
    }
}
