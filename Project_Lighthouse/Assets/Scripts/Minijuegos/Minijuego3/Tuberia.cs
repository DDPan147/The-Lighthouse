using DG.Tweening;
using UnityEngine;

public class Tuberia : MonoBehaviour
{
    public bool isPart1Connected, isPart2Connected, isConnected;
    public bool canRotate = true;

    void Update()
    {
        if(isPart1Connected && isPart2Connected)
        {
            isConnected = true;
        }
        else
        {
            isConnected = false;
        }
    }

    void InitialSuffle()
    {
        int randomRotation1 = Random.Range(0, 3);
        switch (randomRotation1)
        {
            case 0:
                randomRotation1 = 0;
                break;
            case 1:
                randomRotation1 = 90;
                break;
            case 2:
                randomRotation1 = 180;
                break;
            case 3:
                randomRotation1 = 270;
                break;
        }
        int randomRotation2 = Random.Range(0, 3);
        switch (randomRotation2)
        {
            case 0:
                randomRotation2 = 0;
                break;
            case 1:
                randomRotation2 = 90;
                break;
            case 2:
                randomRotation2 = 180;
                break;
            case 3:
                randomRotation2 = 270;
                break;
        }
        Sequence SuffleSequence = DOTween.Sequence();
        SuffleSequence.Append(transform.DORotate(new Vector3(0, 0, randomRotation1), 1f, RotateMode.Fast));
        SuffleSequence.Append(transform.DORotate(new Vector3(0, 0, randomRotation2), 1f, RotateMode.Fast));
    }   //No se habla de esta función

    void InitialRotate()
    {
        Sequence RotateSequence = DOTween.Sequence();
        RotateSequence.Append(transform.DORotate(new Vector3(0, 0, transform.eulerAngles.z + 180), 1f, RotateMode.Fast));
        RotateSequence.Append(transform.DORotate(new Vector3(0, 0, transform.eulerAngles.z - 180), 1f, RotateMode.Fast));
    }

}
