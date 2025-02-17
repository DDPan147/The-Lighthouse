using UnityEngine;

public class Tuberia : MonoBehaviour
{
    public bool isPart1Connected, isPart2Connected, isConnected;
    public float initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation.z;
    }
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

}
